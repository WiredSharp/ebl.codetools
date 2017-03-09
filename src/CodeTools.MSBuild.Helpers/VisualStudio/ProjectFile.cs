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
using System.Xml;
using System.Xml.Linq;

namespace CodeTools.MSBuild.Helpers.VisualStudio
{
    public class ProjectFile
    {
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

        protected readonly XElement _root;
        protected readonly XNamespace _ns;

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
            _root = XElement.Parse(projectFileContent);
            _ns = _root.GetDefaultNamespace();
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

        public void WriteTo(XmlWriter writer)
        {
            _root.WriteTo(writer);
        }

        protected IEnumerable<XElement> GetElements(string localName)
        {
            return _root.Descendants(Name(localName));
        }

        protected XName Name(string localName)
        {
            return _ns.GetName(localName);
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