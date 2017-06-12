using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CodeTools.Helpers.Core
{
	public class XmlFileWrapper
	{
		private readonly XDocument _doc;

		protected XElement Root => _doc.Root;

		private readonly XNamespace _ns;

		protected XmlFileWrapper(FileInfo file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));
			using (var stream = File.OpenRead(file.FullName))
			using (var reader = XmlReader.Create(stream))
				_doc = XDocument.Load(reader);
			_ns = Root.GetDefaultNamespace();
		}

		protected XmlFileWrapper(string fileContent)
		{
			_doc = XDocument.Parse(fileContent);
			_ns = Root.GetDefaultNamespace();
		}

		protected IEnumerable<XElement> GetElements(string localName)
		{
			return Root.Descendants(Name(localName));
		}

		public void RemoveNodes(params string[] tags)
		{
			XElement[] toRemove = Root.Descendants().Where(e => tags.Any(t => e.Name.LocalName == t)).ToArray();
			foreach (XElement element in toRemove)
			{
				element.Remove();
			}
		}

		protected XName Name(string localName)
		{
			return _ns.GetName(localName);
		}

		public void WriteTo(XmlWriter writer)
		{
			_doc.WriteTo(writer);
		}

	    public void WriteTo(Stream output)
	    {
	        using (var writer = XmlWriter.Create(output))
	        {
	            _doc.WriteTo(writer);
	        }
	    }

	    public void WriteTo(FileInfo file)
	    {
	        if (file == null) throw new ArgumentNullException(nameof(file));
	        using (var writer = File.Create(file.FullName))
	        {
	            WriteTo(writer);
	        }
	    }

		public string ToString(SaveOptions saveOptions)
		{
			return Root.ToString(saveOptions);
		}

		public override string ToString()
		{
			return _doc.ToString();
		}

		public string ToXml()
		{
			return XmlExtensions.ToXml(_doc);
		}
	}
}