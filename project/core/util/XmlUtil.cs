using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XmlUtil
	{
		public static XmlDocument CreateDocument(string xml)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			return document;
		}

		public static XmlElement CreateDocumentElement(string xml)
		{
			return CreateDocument(xml).DocumentElement;
		}

		public static XmlElement GetFirstElement(XmlDocument doc, string name)
		{
			XmlNodeList list = doc.GetElementsByTagName(name);
			if (list == null)
			{ 
				return null;
			}
			return (XmlElement)list.Item(0);
		}

		public static XmlElement GetSingleElement(XmlDocument doc, string name)
		{
			XmlNodeList list = doc.GetElementsByTagName(name);
			if (list == null)
			{ 
				return null;
			}
			if (list.Count > 1) 
			{
				throw new CruiseControlException(string.Format("Expected single element '{0}', got multiple ({1})", name, list.Count));
			}
			return (XmlElement)list.Item(0);
		}		

		public static string GetSingleElementValue(XmlDocument doc, string name)
		{
			return GetSingleElementValue(doc, name, "");
		}

		public static string GetSingleElementValue(XmlDocument doc, string name, string defaultValue)
		{
			XmlElement element = GetSingleElement(doc, name);
			if (element == null)
			{
				return defaultValue;
			}
			else
			{
				return element.InnerText;
			}
		}

		public static string GenerateOuterXml(string xmlContent)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(xmlContent);
			return document.OuterXml;
		}		

		public static string SelectValue(XmlNode node, string xpath, string defaultValue)
		{
			if (node == null || node.InnerXml == null || node.InnerXml == String.Empty)
			{
				return defaultValue;
			}
			node = node.SelectSingleNode(xpath);
			if (node == null)
			{
				throw new ArgumentException("No node found at: "+xpath);
			}
			return node.InnerText;
		}

		public static string SelectValue(XmlDocument document, string xpath, string defaultValue)
		{
			XmlNode node = document.SelectSingleNode(xpath);
			return SelectValue(node, xpath, defaultValue);
		}

		public static string SelectRequiredValue(XmlDocument document, string xpath)
		{			
			XmlNode node = document.SelectSingleNode(xpath);
			if (node == null || node.InnerXml == null || node.InnerXml == String.Empty)
			{
				throw new CruiseControlException("Document missing required value at xpath: " + xpath);
			}
			return node.InnerText;
		}

		public static string EncodeCDATA(string text)
		{
			Regex CDATACloseTag = new Regex(@"\]\]>");
			return CDATACloseTag.Replace(text, @"] ]>");
		}

		public static string StringSerialize(object o)
		{
			XmlSerializer serializer = new XmlSerializer(o.GetType());
			StringWriter writer1 = new StringWriter();
			serializer.Serialize(writer1, o);

			StringReader reader = new StringReader(writer1.ToString());
			StringWriter writer2 = new StringWriter();

			// This is because .NET's XML Serialization is a but bunk and puts a dodgy first line in the xml
			reader.ReadLine();
			writer2.WriteLine(@"<?xml version=""1.0""?>");
			writer2.Write(reader.ReadToEnd());

			return writer2.ToString();
		}
	}
}
