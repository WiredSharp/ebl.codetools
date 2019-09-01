using System;
using System.Collections.Generic;
using NUnit.Framework;
using SemanticVersioning.Tools.Core;
using SemanticVersioning.Tools.Tests.TestHelpers;

namespace SemanticVersioning.Tools.Tests
{
    [TestFixture]
    public class SemanticVersionTest
    {
        [Test]
        [TestCaseSource(nameof(SemanticVersions))]
        public void i_can_parse_version(string rawVersion, SemanticVersion version)
        {
            SemanticVersion parsedVersion = SemanticVersion.Parse(rawVersion);
            Assert.AreEqual(version, parsedVersion, "parsed version does not match");
        }

        [Test]
        [TestCaseSource(nameof(SemanticVersions))]
        public void tostring_returns_expected_value(string displayed, SemanticVersion version)
        {
            Assert.AreEqual(displayed, version.ToString(), "unexpected value displayed");
        }

        [Test]
        [TestCaseSource(nameof(InvalidSemanticVersions))]
        public void i_cannot_parse_version(string version)
        {
            Assert.Throws<ArgumentException>(() => SemanticVersion.Parse(version));
        }

        private static IEnumerable<TestCaseData> SemanticVersions
        {
            get
            {
                yield return Generators.NewTestCase("1.0.0", addParsed:true);
                yield return Generators.NewTestCase("13.231.532+1235", addParsed: true);
                yield return Generators.NewTestCase("13.231.532-alpha", addParsed: true);
                yield return Generators.NewTestCase("13.231.532-alpha.234", addParsed: true);
                yield return Generators.NewTestCase("13.231.532+1234-alpha", addParsed: true);
                yield return Generators.NewTestCase("13.231.532+1234.24-alpha", addParsed: true);
            }
        }

        private static IEnumerable<TestCaseData> InvalidSemanticVersions
        {
            get
            {
                yield return Generators.NewTestCase("1.0");
                yield return Generators.NewTestCase("13.231.532+#1235");
                yield return Generators.NewTestCase("13.231#alpha");
                yield return Generators.NewTestCase("alpha");
                yield return Generators.NewTestCase("0");
                yield return Generators.NewTestCase("1");
                yield return Generators.NewTestCase("13.alpha");
                yield return Generators.NewTestCase("13.231alpha");
                yield return Generators.NewTestCase("13.231#alpha.2");
            }
        }
    }
}