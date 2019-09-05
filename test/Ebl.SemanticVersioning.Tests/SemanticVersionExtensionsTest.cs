using NUnit.Framework;
using SemanticVersioning.Tools.Core;
using System.Collections.Generic;

namespace SemanticVersioning.Tools.Tests
{
	[TestFixture]
	class SemanticVersionExtensionsTest
	{
		[Test]
		[TestCaseSource(nameof(VersionsToBump))]
		public void i_can_bump_version(VersionField field, int update)
		{
			SemanticVersion version = new SemanticVersion(1, 2, 3);
			SemanticVersion bumped = version.Bump(field, update);
			switch (field)
			{
				case VersionField.Major:
					Assert.AreEqual(update + version.Major, bumped.Major, "unexpected major field value");
					break;
				case VersionField.Minor:
					Assert.AreEqual(update + version.Minor, bumped.Minor, "unexpected minor field value");
					break;
				case VersionField.Patch:
					Assert.AreEqual(update + version.Patch, bumped.Patch, "unexpected patch field value");
					break;
				default:
					Assert.Inconclusive($"{field}: field not handled, update tests !!");
					break;	
			}
		}

		[Test]
		public void i_can_get_field_value([Values(
			VersionField.Major, 
			VersionField.Minor,
			VersionField.Patch)]VersionField field)
		{
			SemanticVersion version = new SemanticVersion(1, 2, 3);
			int actualField = -1;
			switch (field)
			{
				case VersionField.Major:
					actualField = version.Major;
					break;
				case VersionField.Minor:
					actualField = version.Minor;
					break;
				case VersionField.Patch:
					actualField = version.Patch;
					break;
				default:
					Assert.Inconclusive($"{field}: field not handled, update tests !!");
					break;
			}
			Assert.AreEqual(actualField, version.GetField(field), $"unexpected {field} field value");
		}

		private static IEnumerable<TestCaseData> VersionsToBump
		{
			get
			{
				yield return new TestCaseData(VersionField.Major, -1);
				yield return new TestCaseData(VersionField.Major, 0);
				yield return new TestCaseData(VersionField.Major, 1);
				yield return new TestCaseData(VersionField.Minor, -1);
				yield return new TestCaseData(VersionField.Minor, 0);
				yield return new TestCaseData(VersionField.Minor, 1);
				yield return new TestCaseData(VersionField.Patch, -1);
				yield return new TestCaseData(VersionField.Patch, 0);
				yield return new TestCaseData(VersionField.Patch, 1);
			}
		}
	}
}
