using System;
using System.Xml;

/*
 * Slightly modiefied version of http://stackoverflow.com/a/509340
 */

namespace Solita.LanguageEditor.Common
{
    public static class XmlDocumentExtensions
    {
        public static XmlNode CreateXPath(this XmlDocument doc, string xpath)
        {
            XmlNode node = doc;
            foreach (var part in xpath.Substring(1).Split('/'))
            {
                var nodes = node.SelectNodes(part);
                if (nodes.Count > 1)
                    throw new Exception("Xpath '" + xpath + "' was found multiple times!");

                if (nodes.Count == 1)
                {
                    node = nodes[0];
                    continue;
                }

                if (part.StartsWith("@"))
                {
                    var anode = doc.CreateAttribute(part.Substring(1));
                    node.Attributes.Append(anode);
                    node = anode;
                }
                else
                {
                    string elName, attrib = null;
                    if (part.Contains("["))
                    {
                        part.SplitOnce("[", out elName, out attrib);
                        if (!attrib.EndsWith("]")) throw new Exception("Unsupported XPath (missing ]): " + part);
                        attrib = attrib.Substring(0, attrib.Length - 1);
                    }
                    else elName = part;

                    XmlNode next = doc.CreateElement(elName);
                    node.AppendChild(next);
                    node = next;

                    if (attrib != null)
                    {
                        if (!attrib.StartsWith("@")) throw new Exception("Unsupported XPath attrib (missing @): " + part);
                        string name, value;
                        attrib.Substring(1).SplitOnce("='", out name, out value);
                        if (string.IsNullOrEmpty(value) || !value.EndsWith("'")) throw new Exception("Unsupported XPath attrib: " + part);
                        value = value.Substring(0, value.Length - 1);
                        var anode = doc.CreateAttribute(name);
                        anode.Value = value;
                        node.Attributes.Append(anode);
                    }
                }
            }
            return node;
        }

        private static void SplitOnce(this string value, string separator, out string part1, out string part2)
        {
            if (value != null)
            {
                int idx = value.IndexOf(separator);
                if (idx >= 0)
                {
                    part1 = value.Substring(0, idx);
                    part2 = value.Substring(idx + separator.Length);
                }
                else
                {
                    part1 = value;
                    part2 = null;
                }
            }
            else
            {
                part1 = "";
                part2 = null;
            }
        }
    }
}