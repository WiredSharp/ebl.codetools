using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
    internal static class TestHelpers
    {
        public static void AssertIsUniqueAndEqualsTo(this XElement root, string localName, string expectedValue)
        {
            XElement[] matchingNodes = FindNodes(root, localName);
            Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
            Assert.AreEqual(1, matchingNodes.Count(), "only one " + localName + " node should remain");
            Assert.AreEqual(expectedValue, matchingNodes.First().Value, "unexpected value for " + localName);
        }

        public static void AssertEqualsTo(this XElement root, string localName, string expectedValue)
        {
            XElement[] matchingNodes = FindNodes(root, localName);
            Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
            Assert.IsTrue(matchingNodes.Any(n => n.Value == expectedValue), "unexpected value for " + localName);
        }

        /// <summary>
        /// Asserts that all <param name="expectedValue"></param> match a node with local name <param name="localName"></param>.
        /// node count must match expected values count
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="expectedValue">The expected value.</param>
        public static void AssertExactMatch(this XElement root, string localName, params string[] expectedValue)
        {
            XElement[] matchingNodes = AssertMatch(root, localName, expectedValue);
            Assert.AreEqual(expectedValue.Length, matchingNodes.Length, $"unexpected '{localName}' nodes count");
        }

        /// <summary>
        /// Asserts that all <param name="expectedValue"></param> match a node with local name <param name="localName"></param>.
        /// they may be more node than expected values
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <returns></returns>
        private static XElement[] AssertMatch(XElement root, string localName, params string[] expectedValue)
        {
            XElement[] matchingNodes = FindNodes(root, localName);
            foreach (string value in expectedValue)
            {
                Assert.IsTrue(matchingNodes.Any(node => node.Value == value), $"{value} has not been found, available values: {String.Join(Environment.NewLine, matchingNodes.Select(n => n.Value))}");
            }
            return matchingNodes;
        }

        public static XElement[] FindNodes(this XElement root, string localName)
        {
            XNamespace defaultNamespace = root.GetDefaultNamespace();
            return root.Descendants(defaultNamespace.GetName(localName)).ToArray();
        }

        public static XElement Normalize(this FileInfo projectFile, Settings settings)
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