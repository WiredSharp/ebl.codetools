using System.Collections.Generic;
using NUnit.Framework;
using SemanticVersioning.Tools.Core;
using SemanticVersioning.Tools.Tests.TestHelpers;

namespace SemanticVersioning.Tools.Tests
{
	[TestFixture]
	public class SemanticVersionComparisonTest
	{
		[Test]
		[TestCaseSource(nameof(ComparedVersions))]
		public void is_greater_than(SemanticVersion greater, SemanticVersion lower)
		{
			Assert.Greater(greater, lower, $"'{greater}' should be greater than '{lower}'");
		}

		[Test]
		[TestCaseSource(nameof(ComparedVersions))]
		[TestCaseSource(nameof(EqualVersions))]
		public void is_greater_or_equal_to(SemanticVersion greater, SemanticVersion lower)
		{
			Assert.GreaterOrEqual(greater, lower, $"'{greater}' should be greater or equal to '{lower}'");
		}

		[Test]
		[TestCaseSource(nameof(ComparedVersions))]
		public void is_less_than(SemanticVersion greater, SemanticVersion lower)
		{
			Assert.Less(lower, greater, $"'{lower}' should be less than '{greater}'");
		}

		[Test]
		[TestCaseSource(nameof(ComparedVersions))]
		[TestCaseSource(nameof(EqualVersions))]
		public void is_less_or_equal_to(SemanticVersion greater, SemanticVersion lower)
		{
			Assert.GreaterOrEqual(greater, lower, $"'{greater}' should be greater or equal to '{lower}'");
		}

		[Test]
		[TestCaseSource(nameof(EqualVersions))]
		public void are_equal(SemanticVersion greater, SemanticVersion lower)
		{
			Assert.AreEqual(greater, lower, $"'{greater}' should be equal to '{lower}'");
		}

		private static IEnumerable<TestCaseData> ComparedVersions
		{
			get
			{
				yield return Generators.NewTestCase("1.0.1", "1.0.0");
				yield return Generators.NewTestCase("1.1.1", "1.0.0");
				yield return Generators.NewTestCase("1.1.1", "1.0.3");
				yield return Generators.NewTestCase("2.1.1", "1.5.3");
				yield return Generators.NewTestCase("2.1.1+123", "1.5.3");
				yield return Generators.NewTestCase("2.1.1-123", "1.5.3");
				yield return Generators.NewTestCase("2.1.1-123+abc", "1.5.3");
				yield return Generators.NewTestCase("1.5.3", "1.5.3-qew2134");
				yield return Generators.NewTestCase("1.5.3-alpha.1", "1.5.3-alpha");
				yield return Generators.NewTestCase("1.5.3-alpha.beta", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("1.5.3-beta.2", "1.5.3-beta");
				yield return Generators.NewTestCase("1.5.3-rc.1", "1.5.3-beta.11");
				yield return Generators.NewTestCase("1.5.3", "1.5.3-rc.1");
				yield return Generators.NewTestCase("2.5.3", "1.5.3-qew2134");
				yield return Generators.NewTestCase("2.5.3-alpha.1", "1.5.3-alpha");
				yield return Generators.NewTestCase("2.5.3-alpha.beta", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("2.5.3-beta.2", "1.5.3-beta");
				yield return Generators.NewTestCase("2.5.3-rc.1", "1.5.3-beta.11");
				yield return Generators.NewTestCase("2.5.3", "1.5.3-rc.1");
				yield return Generators.NewTestCase("1.8.3", "1.5.3-qew2134");
				yield return Generators.NewTestCase("1.8.3-alpha.1", "1.5.3-alpha");
				yield return Generators.NewTestCase("1.8.3-alpha.beta", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("1.8.3-beta.2", "1.5.3-beta");
				yield return Generators.NewTestCase("1.8.3-rc.1", "1.5.3-beta.11");
				yield return Generators.NewTestCase("1.8.3", "1.5.3-rc.1");
				yield return Generators.NewTestCase("1.5.13", "1.5.3-qew2134");
				yield return Generators.NewTestCase("1.5.13-alpha.1", "1.5.3-alpha");
				yield return Generators.NewTestCase("1.5.13-alpha.beta", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("1.5.13-beta.2", "1.5.3-beta");
				yield return Generators.NewTestCase("1.5.13-rc.1", "1.5.3-beta.11");
				yield return Generators.NewTestCase("1.5.13", "1.5.3-rc.1");
				yield return Generators.NewTestCase("21.5.3-qew2134", "1.5.3");
				yield return Generators.NewTestCase("12.5.3-alpha", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("12.5.3-alpha.1", "1.5.3-alpha.beta");
				yield return Generators.NewTestCase("12.5.3-beta", "1.5.3-beta.2");
				yield return Generators.NewTestCase("12.5.3-beta.11", "1.5.3-rc.1");
				yield return Generators.NewTestCase("12.5.3-rc.1", "1.5.3");
				yield return Generators.NewTestCase("1.51.3-qew2134", "1.5.3");
				yield return Generators.NewTestCase("1.51.3-alpha", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("1.51.3-alpha.1", "1.5.3-alpha.beta");
				yield return Generators.NewTestCase("1.51.3-beta", "1.5.3-beta.2");
				yield return Generators.NewTestCase("1.51.3-beta.11", "1.5.3-rc.1");
				yield return Generators.NewTestCase("1.51.3-rc.1", "1.5.3");
				yield return Generators.NewTestCase("1.5.13-qew2134", "1.5.3");
				yield return Generators.NewTestCase("1.5.13-alpha", "1.5.3-alpha.1");
				yield return Generators.NewTestCase("1.5.113-alpha.1", "1.5.3-alpha.beta");
				yield return Generators.NewTestCase("1.5.13-beta", "1.5.3-beta.2");
				yield return Generators.NewTestCase("1.5.13-beta.11", "1.5.3-rc.1");
				yield return Generators.NewTestCase("1.5.13-rc.1", "1.5.3");
			}
		}

		private static IEnumerable<TestCaseData> EqualVersions
		{
			get
			{
				yield return Generators.NewTestCase("1.0.1", "1.0.1");
				yield return Generators.NewTestCase("2.1.1+123", "2.1.1+123");
				yield return Generators.NewTestCase("2.1.23-123", "2.1.23-123");
				yield return Generators.NewTestCase("2.12.1+abc", "2.12.1+abc");
				yield return Generators.NewTestCase("2.531.1-123+abc", "2.531.1-123+abc");
			}
		}
	}
}