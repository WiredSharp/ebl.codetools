using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace CodeTools.Test.Common
{
    internal static class TestHelpers
    {
        public static FileInfo GetTestFileInfo(this string projectFile)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, projectFile));
        }

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

        /// <summary>
        /// Finds the nodes by local name, considering node is in default namespace.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="localName">Name of the local.</param>
        /// <returns></returns>
        public static XElement[] FindNodes(this XElement root, string localName)
        {
            XNamespace defaultNamespace = root.GetDefaultNamespace();
            return root.Descendants(defaultNamespace.GetName(localName)).ToArray();
        }
    }
}