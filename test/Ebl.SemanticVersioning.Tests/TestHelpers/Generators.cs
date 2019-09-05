using NUnit.Framework;
using SemanticVersioning.Tools.Core;

namespace SemanticVersioning.Tools.Tests.TestHelpers
{
    internal static class Generators
    {
        public static TestCaseData NewTestCase(string version, bool addParsed = false)
        {
            if (addParsed)
            return new TestCaseData(version, SemanticVersion.Parse(version)); // {TestName = version};
            else
            return new TestCaseData(version); // {TestName = version};
        }

        public static TestCaseData NewTestCase(string version1, string version2)
        {
            return new TestCaseData(SemanticVersion.Parse(version1), SemanticVersion.Parse(version2));// { TestName = $"Test_{version1}_{version2}" };
        }
    }
}