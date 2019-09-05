#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-14 (17:45)
// ///
// ///
#endregion

using System.Xml.Linq;
using CodeTools.Test.Common;
using NUnit.Framework;

namespace CodeTools.VisualStudio.Tools.Test
{
    [TestFixture]
    public class NugetSpecerTest
    {
        [Test]
        public void i_can_generate_a_valid_nuspec_file()
        {
            var specer = new NugetSpecer();
            string generated = specer.GenerateNuSpec();
            TestContext.WriteLine(generated);
            XDocument xml = XDocument.Parse(generated);
            Assert.IsNotNull(xml.Root, "root node not found");
            Assert.AreEqual("package", xml.Root.Name.LocalName, "unexpected root tag");
            HasUniqueNode(xml, "id");
            HasUniqueNode(xml, "version");
            HasUniqueNode(xml, "authors");
            HasUniqueNode(xml, "description");
        }

        private static void HasUniqueNode(XDocument xml, string localName)
        {
            Assert.AreEqual(1, xml.Root.FindNodes(localName).Length, $"unexpected {localName} element count");
        }
    }
}