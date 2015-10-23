using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using Solita.LocalizationEditor.UI.Common;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Solita.LocalizationEditor.UI.Helpers
{
    public class XmlLanguageFileHelper
    {
        public const string LanguagesRootXPath = "/languages";
        private const string LanguageXPath = LanguagesRootXPath + "/language";
        private const string TranslationXPath = LanguageXPath + "[@id='{0}']{1}";
        private readonly XmlDocument _xmlDoc;

        public XmlDocument XmlDoc
        {
            get
            {
                if (_xmlDoc == null) { throw new InvalidOperationException("member _xmlDoc is null."); }
                return _xmlDoc;
            }
        }

        public XmlLanguageFileHelper()
        {
            _xmlDoc = new XmlDocument();
        }

        public static string FindExistingTranslation(XmlNode xml, string lang, string key)
        {
            var xpath = string.Format(TranslationXPath, lang, key);
            var node = xml.SelectSingleNode(xpath);

            return node == null ? null : node.InnerText;
        }

        public void EnsureLanguagesXPath()
        {
            XmlDoc.CreateXPath(LanguagesRootXPath);
        }

        public void SetTranslation(string key, string lang, string value)
        {
            var xPath = string.Format(TranslationXPath, lang, key);
            var node = XmlDoc.CreateXPath(xPath);
            node.InnerText = value;
        }

        public void SetTranslation(XmlDocument xmlDoc, string key, string lang, string value)
        {
            var xPath = string.Format(TranslationXPath, lang, key);
            var node = xmlDoc.CreateXPath(xPath);
            node.InnerText = value;
        }

        public void AddLanguageNames()
        {
            var nodes = XmlDoc.SelectNodes(LanguageXPath);
            if (nodes == null) { return; }

            foreach (XmlNode node in nodes)
            {
                var id = node.Attributes["id"].InnerText;
                var attribute = XmlDoc.CreateAttribute("name");
                attribute.InnerText = CultureInfo.GetCultureInfo(id).NativeName;
                node.Attributes.Append(attribute);
            }
        }
    }
}