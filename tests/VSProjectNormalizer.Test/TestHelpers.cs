using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
    internal static class TestHelpers
    {
        public static void AssertIsUniqueAndEqualsTo(XElement root, string localName, string expectedValue)
        {
            IEnumerable<XElement> matchingNodes = FindNodes(root, localName);
            Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
            Assert.AreEqual(1, matchingNodes.Count(), "only one " + localName + " node should remain");
            Assert.AreEqual(expectedValue, matchingNodes.First().Value, "unexpected value for " + localName);
        }

        public static void AssertEqualsTo(XElement root, string localName, string expectedValue)
        {
            IEnumerable<XElement> matchingNodes = FindNodes(root, localName);
            Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
            Assert.IsTrue(matchingNodes.Any(n => n.Value == expectedValue), "unexpected value for " + localName);
        }

        public static IEnumerable<XElement> FindNodes(XElement root, string localName)
        {
            XNamespace defaultNamespace = root.GetDefaultNamespace();
            return root.Descendants(defaultNamespace.GetName(localName));
        }

        public static XElement Normalize(FileInfo projectFile, Settings settings)
        {
            var normalizer = new VSProjectNormalizer(settings);
            string normalized = normalizer.Normalize(projectFile);
            File.WriteAllText(
                              Path.Combine(Path.GetDirectoryName(projectFile.FullName), Path.GetFileNameWithoutExtension(projectFile.Name)) +
                              ".normalized.xml",
                              normalized);
            XElement root = XElement.Parse(normalized);
            return root;
        }
    }
}