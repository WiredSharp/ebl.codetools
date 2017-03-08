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

namespace VSProjectNormalizer.MsBuild
{
    internal static class Elements
    {
        internal static XNamespace MSBUILD_NS = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        public static XElement Choose(XElement firstChild, params XElement[] childs)
        {
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement("Choose", firstChild, childs);
        }

        public static XElement When(XAttribute condition, XElement firstChild, params XElement[] childs)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement("When", condition, firstChild, childs);
        }

        public static XElement Otherwise(XElement firstChild, params XElement[] childs)
        {
            if (firstChild == null) throw new ArgumentNullException(nameof(firstChild));
            return NewXElement("Otherwise", firstChild, childs);
        }

        public static XElement PropertyGroup(params object[] childs)
        {
            return NewXElement("PropertyGroup", childs);
        }

        public static XElement Property(string name, params object[] childs)
        {
            return NewXElement(name, childs);
        }

        private static XElement NewXElement(string localName, params object[] childs)
        {
            return new XElement(MSBUILD_NS.GetName(localName), childs);
        }
    }
}