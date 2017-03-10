#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-08 (15:33)
// ///
// ///
#endregion

using System;
using System.Xml.Linq;

namespace CodeTools.MSBuild.Helpers.VisualStudio
{
    public static class Elements
    {
        private const string LINK_TAG = "Link";
        private const string IMPORT_TAG = "Import";
        private const string PROJECT_ATTRIBUTE_TAG = "Project";
        private const string PROPERTY_GROUP_TAG = "PropertyGroup";
        private const string OTHERWISE_TAG = "Otherwise";
        private const string WHEN_TAG = "When";
        private const string CHOOSE_TAG = "Choose";

        internal static XNamespace MSBUILD_NS = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        public static XElement Choose(XElement firstChild, params XElement[] childs)
        {
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement(CHOOSE_TAG, firstChild, childs);
        }

        public static XElement When(XAttribute condition, XElement firstChild, params XElement[] childs)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement(WHEN_TAG, condition, firstChild, childs);
        }

        public static XElement Otherwise(XElement firstChild, params XElement[] childs)
        {
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement(OTHERWISE_TAG, firstChild, childs);
        }

        public static XElement PropertyGroup(params object[] childs)
        {
            return NewXElement(PROPERTY_GROUP_TAG, childs);
        }

        public static XElement Property(string name, string value, bool checkDefined = false)
        {
            XElement property = NewXElement(name, value);
            if (checkDefined)
            {
                property.Add(Condition.TagDefined(name));
            }
            return property;
        }

        public static XElement Property(string name, string value, XAttribute condition)
        {
            return NewXElement(name, condition, value);
        }

        public static XElement Link(string linkPath)
        {
            return NewXElement(LINK_TAG, linkPath);
        }

        public static XElement Import(string importPath, XAttribute condition)
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

        public static XElement Import(string importPath, bool checkExists = true)
        {
            if (checkExists)
            {
                return Import(importPath, Condition.Exists(importPath));
            }
            else
            {
                return Import(importPath);
            }
        }

        private static XElement NewXElement(string localName, params object[] childs)
        {
            return new XElement(Name(localName), childs);
        }

        private static XName Name(string localName)
        {
            return MSBUILD_NS.GetName(localName);
        }
    }
}