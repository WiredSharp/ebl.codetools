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
using System.Xml.Linq;
using CodeTools.Helpers.Core;

namespace CodeTools.MSBuild.Helpers.VisualStudio
{
    public class ProjectFile: XmlFileWrapper
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
        protected const string LINK_TAG = "Link";
        protected const string PROJECT_ATTRIBUTE_TAG = "Project";
        protected const string OTHERWISE_TAG = "Otherwise";
        protected const string WHEN_TAG = "When";
        protected const string CHOOSE_TAG = "Choose";

        protected const string INCLUDE_ATTRIBUTE_TAG = "Include";

        public static XNamespace MSBUILD_NS = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        public static ProjectFile Create()
        {
            return new ProjectFile(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003""/>");
        }

        public static ProjectFile Parse(string projectFileContent)
        {
            return new ProjectFile(projectFileContent);
        }

        public static ProjectFile Load(FileInfo projectFile)
        {
            if (projectFile == null) throw new ArgumentNullException(nameof(projectFile));
            return new ProjectFile(File.ReadAllText(projectFile.FullName));
        }

        protected ProjectFile(string projectFileContent)
			:base(projectFileContent)
        {
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
            return NewXElement(LINK_TAG, linkPath);
        }

        public XElement NewProperty(string propertyTag, string value, XAttribute condition)
        {
            return NewXElement(propertyTag, condition, value);
        }

        public XElement NewProperty(string propertyTag, string value, bool checkDefined)
        {
            XElement property = NewXElement(propertyTag, value);
            if (checkDefined)
            {
                property.Add(Condition.TagNotDefined(propertyTag));
            }
            return property;
        }

        public XElement NewPropertyGroup(params XElement[] properties)
        {
            return NewXElement(PROPERTY_GROUP_TAG, properties);
        }

        public XElement NewPropertyGroup(XAttribute condition, params XElement[] properties)
        {
            return NewXElement(PROPERTY_GROUP_TAG, condition, properties);
        }

        public XElement Choose(XElement firstChild, params XElement[] childs)
        {
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement(CHOOSE_TAG, firstChild, childs);
        }

        public XElement Otherwise(XElement firstChild, params XElement[] childs)
        {
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement(OTHERWISE_TAG, firstChild, childs);
        }

        public XElement When(XAttribute condition, XElement firstChild, params XElement[] childs)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement(WHEN_TAG, condition, firstChild, childs);
        }

        public XElement NewImport(string importPath, XAttribute condition)
        {
            if (condition != null)
            {
                return NewXElement(IMPORT_TAG, new XAttribute(PROJECT_ATTRIBUTE_TAG, importPath), condition);
            }
            else
            {
                return NewXElement(IMPORT_TAG);
            }
        }

        public XElement NewImport(string importPath, bool checkExists = true)
        {
            if (checkExists)
            {
                return NewImport(importPath, Condition.Exists(importPath));
            }
            else
            {
                return NewImport(importPath, null);
            }
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
    }
}