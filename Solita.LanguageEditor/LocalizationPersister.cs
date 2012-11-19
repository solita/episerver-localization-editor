using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;
using Solita.LanguageEditor.Common;
using Solita.LanguageEditor.Definitions;
using Solita.LanguageEditor.Models;

namespace Solita.LanguageEditor
{
    public class LocalizationPersister
    {
        private const string TranslationFile = "Localizations.xml";
        private const string LanguageXPath = "/languages/language";
        private const string TranslationXPath = LanguageXPath + "[@id='{0}']{1}";

        private readonly string _translationFilePath;

        public LocalizationPersister()
        {
            var folderPath = VirtualPathUtility.AppendTrailingSlash(Settings.AutoPopulated.LangFolderVirtualPath);
            _translationFilePath = VirtualPathUtility.Combine(folderPath, TranslationFile);
        }

        public LanguageEditorViewModel GetLocalizations()
        {
            var xml = LoadXml(_translationFilePath);

            var enabledLanguageIds =
                ServiceLocator.Current.GetInstance<ILanguageBranchRepository>()
                              .ListEnabled()
                              .Select(branch => branch.LanguageID)
                              .ToList();

            var model = new LanguageEditorViewModel { Languages = enabledLanguageIds };

            foreach (var field in GetLocalizationFields())
            {
                var attribute = (LocalizationAttribute)Attribute.GetCustomAttribute(field, typeof(LocalizationAttribute));
                var key = (string)field.GetValue(null);

                var translation = model.AddTranslation(key, attribute.Description, attribute.Category, attribute.Order, attribute.DefaultValue);
                foreach (var lang in model.Languages)
                {
                    var value = FindExistingTranslation(xml, lang, key);
                    translation.AddTranslation(lang, value ?? string.Empty);
                }
            }

            return model;
        }

        private static IEnumerable<FieldInfo> GetLocalizationFields()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   from field in type.GetFields(BindingFlags.Static | BindingFlags.Public)
                   where field.GetCustomAttributes(typeof (LocalizationAttribute), true).Any()
                   select field;
        }

        private static XmlDocument LoadXml(string filePath)
        {
            var file = (UnifiedFile) HostingEnvironment.VirtualPathProvider.GetFile(filePath);
            if (file == null)
            {
                return new XmlDocument();
            }

            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                var document = new XmlDocument();
                document.Load(stream);
                return document;
            }
        }

        private static string FindExistingTranslation(XmlNode xml, string lang, string key)
        {
            var xpath = string.Format(TranslationXPath, lang, key);
            var node = xml.SelectSingleNode(xpath);

            return node == null ? null : node.InnerText;
        }
   
        public void SaveLocalizations(LanguageEditorViewModel model)
        {
            var xml = new XmlDocument();
            foreach(var translation in model.Translations)
            {
                foreach (var dictionary in translation.Translations)
                {
                    SetTranslation(xml, translation.Key, dictionary.Key, dictionary.Value);
                }
            }

            AddLanguageNames(xml);
            SaveXml(xml, _translationFilePath);
            
            LocalizationProviderInitiator.ReInitProvider();
        }

        private static void SetTranslation(XmlDocument xml, string key, string lang, string value)
        {
            var xPath = string.Format(TranslationXPath, lang, key);
            var node = xml.CreateXPath(xPath);
            node.InnerText = value;
        }

        private static void AddLanguageNames(XmlDocument xml)
        {
            var nodes = xml.SelectNodes(LanguageXPath);
            if (nodes == null)
                return;

            foreach (XmlNode node in nodes)
            {
                var id = node.Attributes["id"].InnerText;
                var attribute = xml.CreateAttribute("name");
                attribute.InnerText = CultureInfo.GetCultureInfo(id).NativeName;
                node.Attributes.Append(attribute);
            }
        }

        private static void SaveXml(XmlDocument xml, string filePath)
        {
            var path = filePath.Substring(0, filePath.LastIndexOf("/", StringComparison.Ordinal) + 1);
            var unifiedDirectory = (UnifiedDirectory) HostingEnvironment.VirtualPathProvider.GetDirectory(path);

            var file = HostingEnvironment.VirtualPathProvider.FileExists(filePath)
                           ? (UnifiedFile) HostingEnvironment.VirtualPathProvider.GetFile(filePath)
                           : unifiedDirectory.CreateFile(filePath);

            using (var stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write))
            {
                xml.Save(stream);
            }
        }
    }
}