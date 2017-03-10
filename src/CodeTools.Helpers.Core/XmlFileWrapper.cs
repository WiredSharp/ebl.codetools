using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace CodeTools.Helpers.Core
{
    public class XmlFileWrapper
    {
        protected readonly XElement _root;
        protected readonly XNamespace _ns;

        protected XmlFileWrapper(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            _root = XElement.Parse(File.ReadAllText(file.FullName));
            _ns = _root.GetDefaultNamespace();
        }

        protected XmlFileWrapper(string fileContent)
        {
            _root = XElement.Parse(fileContent);
            _ns = _root.GetDefaultNamespace();
        }

        protected IEnumerable<XElement> GetElements(string localName)
        {
            return _root.Descendants(Name(localName));
        }

        protected XName Name(string localName)
        {
            return _ns.GetName(localName);
        }

        public void WriteTo(XmlWriter writer)
        {
            _root.WriteTo(writer);
        }

        public string ToString(SaveOptions saveOptions)
        {
            return _root.ToString(saveOptions);
        }

        public override string ToString()
        {
            return _root.ToString();
        }
    }
}