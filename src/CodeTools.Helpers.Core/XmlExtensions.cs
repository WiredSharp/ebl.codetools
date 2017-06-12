#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-14 (12:54)
// ///
// ///
#endregion

using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace CodeTools.Helpers.Core
{
    public static class XmlExtensions
    {
        public static XElement TryAdd(this XElement parent, string localName)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            return TryAdd(parent, parent.GetDefaultNamespace().GetName(localName));
        }

        public static XElement TryAdd(this XElement parent, XName name)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            var target = parent.Element(name);
            if (target != null)
            {
                return target;
            }
            target = new XElement(name);
            parent.Add(target);
            return target;
        }

        public static string ToXml(this XDocument document)
        {
            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(document.Declaration.Encoding);
            }
            catch
            {
                encoding = Encoding.UTF8;
            }
            using (var writer = new StringWriterWithEncoding(encoding))
            {
                document.Save(writer);
                writer.Flush();
                return writer.ToString();
            }
        }

        private class StringWriterWithEncoding : StringWriter
        {
            public StringWriterWithEncoding(Encoding encoding)
            {
                Encoding = encoding;
            }

            public override Encoding Encoding { get; }
        }
    }
}