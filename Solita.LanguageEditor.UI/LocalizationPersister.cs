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
using Solita.LanguageEditor.Definitions;
using Solita.LanguageEditor.UI.Common;
using Solita.LanguageEditor.UI.Models;

namespace Solita.LanguageEditor.UI
{
    public class LocalizationPersister
    {
        private const string TranslationFile = "Localizations.xml";
        private const string LanguagesXPath = "/languages";
        private const string LanguageXPath = LanguagesXPath + "/language";
        private const string TranslationXPath = LanguageXPath + "[@id='{0}']{1}";

        private readonly string _translationFilePath;

        public LocalizationPersister()
        {
            var folderPath = VirtualPathUtility.AppendTrailingSlash(Settings.AutoPopulated.LangFolderVirtualPath);
            _translationFilePath = VirtualPathUtility.Combine(folderPath, TranslationFile);
        }

        public IList<string> GetEnabledLanguages()
        {
            return ServiceLocator.Current.GetInstance<ILanguageBranchRepository>()
                              .ListEnabled()
                              .Select(branch => branch.LanguageID)
                              .ToList();
        }

        public CategoryList GetLocalizations()
        {
            var xml = LoadXml(_translationFilePath);
            var languages = GetEnabledLanguages();
            var categories = new CategoryList();
              
            foreach (var localization in LocalizationHelpers.GetLocalizationDefinitions())
            {
                var translation = categories.AddTranslation(
                    localization.Key, localization.Description, localization.Category, localization.DefaultValue);

                foreach (var lang in languages)
                {
                    var value = FindExistingTranslation(xml, lang, translation.Key);
                    translation.AddTranslation(lang, value ?? string.Empty);
                }
            }

            return categories;
        }

        public object GetJsonLocalizations(string version)
        {
            var xml = LoadXml(_translationFilePath, version);
            var languages = GetEnabledLanguages();

            var localizations = LocalizationHelpers.GetLocalizationDefinitions().Select(d => new
                {
                    key = d.Key,
                    translations = languages.Select(language => new {language, value = FindExistingTranslation(xml, language, d.Key)})
                });


            return new {localizations};
        }

        public IList<XmlVersionInfo> GetTranslationFileVersions()
        {
            var versions = new List<XmlVersionInfo>();
            if (HostingEnvironment.VirtualPathProvider.FileExists(_translationFilePath))
            {
                var file = (UnifiedFile) HostingEnvironment.VirtualPathProvider.GetFile(_translationFilePath);
                var versioningFile = file as VersioningFile;
                if (versioningFile != null)
                {
                    versions.AddRange(
                        versioningFile.GetVersions()
                                      .Select(v => new XmlVersionInfo(v.Id.ToString(), v.Name, v.Created, v.CreatedBy)));
                }
            }

            return versions;
        }

        private static XmlDocument LoadXml(string filePath)
        {
            return LoadXml(filePath, null);
        }

        private static XmlDocument LoadXml(string filePath, string version)
        {
            if (!HostingEnvironment.VirtualPathProvider.FileExists(filePath))
            {
                return new XmlDocument();
            }

            var file = (UnifiedFile)HostingEnvironment.VirtualPathProvider.GetFile(filePath);
            if (version != null)
            {
                // Will throw an exception if the VPP folder doesn't support versioning, but that's reasonable here.
                file = ((VersioningFile) file).GetVersion(version);
            }

            using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
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
            // Ensure root node is created
            xml.CreateXPath(LanguagesXPath);

            foreach(var translation in model.Categories.SelectMany(category => category.Translations))
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