#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (10:23)
// ///
// ///
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CodeTools.MSBuild.Helpers.VisualStudio
{
    public class ProjectFile
    {
        protected const string WPF_PROJECT_TYPE = "{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}"; // followed by CSHARP_PROJECT_TYPE
        protected const string CSHARP_PROJECT_TYPE = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        protected const string WEB_APPLICATION_PROJECT_TYPE = "{349C5851-65DF-11DA-9384-00065B846F21}";

        protected const string PROJECT_TYPES_TAG = "ProjectTypeGuids";
        protected const string PROJECT_GUID_TAG = "ProjectGuid";
        protected const string PROPERTY_GROUP_TAG = "PropertyGroup";
        protected const string ITEM_GROUP_TAG = "ItemGroup";
        protected const string TARGET_FRAMEWORK_VERSION_TAG = "TargetFrameworkVersion";
        protected const string OUTPUT_PATH_TAG = "OutputPath";
        protected const string IMPORT_TAG = "Import";
        protected const string ASSEMBLY_NAME_TAG = "AssemblyName";
        protected const string CONFIGURATION_TAG = "Configuration";
        protected const string PLATFORM_TAG = "Platform";
        protected const string INTERMEDIATE_OUTPUT_PATH_TAG = "IntermediateOutputPath";
        protected const string COMPILE_TAG = "Compile";
        protected const string LINK_TAG = "Link";

        protected const string INCLUDE_ATTRIBUTE_TAG = "Include";

        protected XElement Root => _document.Root;
        protected readonly XNamespace _ns;
        private readonly XDocument _document;

        public static ProjectFile Parse(string projectFileContent)
        {
            return new ProjectFile(projectFileContent);
        }

        public static ProjectFile Load(FileInfo projectFile)
        {
            if (projectFile == null) throw new ArgumentNullException(nameof(projectFile));
            return Parse(File.ReadAllText(projectFile.FullName));
        }

        protected ProjectFile(string projectFileContent)
        {
            _document = XDocument.Parse(projectFileContent);
            if (_document.Root == null) throw new ArgumentException("invalid xml document", nameof(projectFileContent));
            _ns = Root.GetDefaultNamespace();
        }

        public XElement[] Platform()
        {
            return GetElements(PLATFORM_TAG).ToArray();
        }

        public XElement[] Configuration()
        {
            return GetElements(CONFIGURATION_TAG).ToArray();
        }

        public XElement[] AssemblyName()
        {
            return GetElements(ASSEMBLY_NAME_TAG).ToArray();
        }

        public XElement[] TargetFrameworkVersion()
        {
            return GetElements(TARGET_FRAMEWORK_VERSION_TAG).ToArray();
        }

        public XElement[] ProjectGuid()
        {
            return GetElements(PROJECT_GUID_TAG).ToArray();
        }

        public XElement[] OutputPath()
        {
            return GetElements(OUTPUT_PATH_TAG).ToArray();
        }

        public XElement[] IntermediateOutputPath()
        {
            return GetElements(INTERMEDIATE_OUTPUT_PATH_TAG).ToArray();
        }

        public XElement[] GetImports()
        {
            return GetElements(IMPORT_TAG).ToArray();
        }

        public XElement[] GetProperties(params string[] localNames)
        {
            return GetElements(PROPERTY_GROUP_TAG).Elements().Where(p => localNames.Any(name => name == p.Name.LocalName)).ToArray();
        }

        public XElement[] GetProperties(Func<XElement, bool> filter)
        {
            return GetElements(PROPERTY_GROUP_TAG).Elements().Where(filter).ToArray();
        }

        public XElement[] GetItems(Func<XElement, bool> filter)
        {
            return GetElements(ITEM_GROUP_TAG).Elements().Where(filter).ToArray();
        }

        public XElement[] PropertyGroups()
        {
            return GetElements(PROPERTY_GROUP_TAG).ToArray();
        }

        public XElement[] ItemGroups()
        {
            return GetElements(ITEM_GROUP_TAG).ToArray();
        }

        public XElement NewLink(string linkPath)
        {
            return new XElement(Name(LINK_TAG), linkPath);
        }

        public XElement NewProperty(string propertyTag, string value, bool checkDefined)
        {
            return Elements.Property(propertyTag, value, checkDefined);
        }

        public bool IsWpfProject()
        {
            string projectType = GetProjectType();
            return projectType.StartsWith(WPF_PROJECT_TYPE);
        }

        public bool IsWebProject()
        {
            string projectType = GetProjectType();
            return projectType == null || projectType.Contains(WEB_APPLICATION_PROJECT_TYPE);
        }

        public bool IsCSharpProject()
        {
            string projectType = GetProjectType();
            return projectType == null || projectType.StartsWith(CSHARP_PROJECT_TYPE);
        }

        protected string GetProjectType()
        {
            XElement projectTypeNode = GetElements(PROJECT_TYPES_TAG).FirstOrDefault();
            return projectTypeNode?.Value.Trim().ToUpper();
        }

        protected IEnumerable<XElement> GetElements(string localName)
        {
            return Root.Descendants(Name(localName));
        }

        protected XName Name(string localName)
        {
            return _ns.GetName(localName);
        }

        public string ToString(SaveOptions saveOptions)
        {
            return Root.ToString(saveOptions);
        }

        public override string ToString()
        {
            return _document.ToString();
        }

        public string ToXml()
        {
            return ToXml(_document);
        }

        public void WriteTo(XmlWriter writer)
        {
            Root.WriteTo(writer);
        }

        public void RemoveNodes(params string[] tags)
        {
            XElement[] toRemove = Root.Descendants().Where(e => tags.Any(t => e.Name.LocalName == t)).ToArray();
            foreach (XElement element in toRemove)
            {
                element.Remove();
            }
        }

        /// <summary>
        /// Returns list of nodes between given <param name="regionMark"></param>.
        /// <param name="regionMark"></param> are included in region.
        /// </summary>
        /// <param name="regionMark">The region mark.</param>
        /// <returns></returns>
        public XNode[] GetRegion(XComment regionMark)
        {
            List<XNode> nodes = new List<XNode>();
            bool inRegion = false;
            foreach (XNode node in Root.DescendantNodes())
            {
                bool isRegionMark = AreEquivalent(node, regionMark);
                if (inRegion || isRegionMark)
                {
                    nodes.Add(node);
                    if (isRegionMark)
                    {
                        inRegion = !inRegion;
                    }
                }
            }
            return nodes.ToArray();
        }

        private bool AreEquivalent(XObject lhs, XComment rhs)
        {
            if (rhs == null)
            {
                return false;
            }
            if (lhs.NodeType != rhs.NodeType)
            {
                return false;
            }
            var lhsElement = lhs as XComment;
            return lhsElement != null && lhsElement.Value == rhs.Value;
        }

        protected static string ToXml(XDocument document)
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