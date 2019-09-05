using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SemanticVersioning
{
	public struct SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
	{
		public const string RegularExpression = @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?<prerelease>\-[0-9A-Za-z\-\.]+|)(?<metadata>\+[0-9A-Za-z\-\.]+|)";

		private static readonly Regex SemverRegex = new Regex($"^{RegularExpression}$", RegexOptions.Compiled);

		public readonly int Major;

		public readonly int Minor;

		public readonly int Patch;

		public readonly string Metadata;

		public readonly string PreRelease;

		private readonly string _display;

		public SemanticVersion(int major, int minor, int patch, string metadata, string preRelease)
			 : this()
		{
			if (major < 0) throw new ArgumentOutOfRangeException(nameof(major));
			if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor));
			if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch));

			Major = major;
			Minor = minor;
			Patch = patch;

			var display = new StringBuilder($"{Major}.{Minor}.{Patch}");

			if (!String.IsNullOrWhiteSpace(preRelease))
			{
				if (preRelease.StartsWith("-"))
				{
					PreRelease = preRelease;
				}
				else
				{
					PreRelease = $"-{PreRelease}";
				}
				display.Append(PreRelease);
			}

			if (!String.IsNullOrWhiteSpace(metadata))
			{
				if (metadata.StartsWith("+"))
				{
					Metadata = metadata;
				}
				else
				{
					Metadata = $"+{Metadata}";
				}
				display.Append(Metadata);
			}

			_display = display.ToString();
		}

		public SemanticVersion(int major, int minor, int patch)
			 : this(major, minor, patch, null, null)
		{
		}

		public int CompareTo(object other)
		{
			if (other is SemanticVersion semanticVersion)
			{
				return CompareTo(semanticVersion);
			}
			else
			{
				throw new ArgumentException($"argument is not a {nameof(SemanticVersion)}", nameof(other));
			}
		}

		public int CompareTo(SemanticVersion other)
		{
			int compareTo = Major.CompareTo(other.Major);
			if (compareTo != 0) return compareTo;
			compareTo = Minor.CompareTo(other.Minor);
			if (compareTo != 0) return compareTo;
			compareTo = Patch.CompareTo(other.Patch);
			if (compareTo != 0) return compareTo;
			return ComparePreRelease(PreRelease, other.PreRelease);
		}

		public override bool Equals(object other)
		{
			if (other is SemanticVersion semanticVersion)
			{
				return Equals(semanticVersion);
			}
			return false;
		}

		public bool Equals(SemanticVersion semanticVersion)
		{
			bool versionFieldsAreEqual = Major.Equals(semanticVersion.Major)
							  && Minor.Equals(semanticVersion.Minor)
							  && Patch.Equals(semanticVersion.Patch);
			if (!versionFieldsAreEqual) return false;
			return String.Equals(Metadata, semanticVersion.Metadata, StringComparison.Ordinal)
					  && String.Equals(PreRelease, semanticVersion.PreRelease, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return Major.GetHashCode() ^ Minor.GetHashCode() ^ Patch.GetHashCode();
		}

		public override string ToString()
		{
			return _display;
		}

		public static bool operator <=(SemanticVersion lhs, SemanticVersion rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}

		public static bool operator >=(SemanticVersion lhs, SemanticVersion rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}

		public static bool operator <(SemanticVersion lhs, SemanticVersion rhs)
		{
			return lhs.CompareTo(rhs) < 0;
		}

		public static bool operator >(SemanticVersion lhs, SemanticVersion rhs)
		{
			return lhs.CompareTo(rhs) > 0;
		}

		public static bool operator ==(SemanticVersion lhs, SemanticVersion rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SemanticVersion lhs, SemanticVersion rhs)
		{
			return !(lhs == rhs);
		}

		public static SemanticVersion Parse(string version)
		{
			Match match = SemverRegex.Match(version);
			if (!match.Success)
			{
				throw new ArgumentException("not a valid semantic version", nameof(version));
			}
			return new SemanticVersion(Int32.Parse(match.Groups["major"].Value)
			, Int32.Parse(match.Groups["minor"].Value)
			, Int32.Parse(match.Groups["patch"].Value)
			, match.Groups["metadata"].Value
			, match.Groups["prerelease"].Value);
		}

		/// <summary>
		///  Precedence for two pre-release versions with the same major, minor, and patch version
		/// MUST be determined by comparing each dot separated identifier from left to right until a difference is found as follows:
		/// identifiers consisting of only digits are compared numerically and identifiers with letters or hyphens are compared lexically in ASCII sort order.
		/// Numeric identifiers always have lower precedence than non-numeric identifiers.
		/// A larger set of pre-release fields has a higher precedence than a smaller set
		/// </summary>
		/// <param name="preRelease"></param>
		/// <param name="otherPreRelease"></param>
		/// <returns></returns>
		private static int ComparePreRelease(string preRelease, string otherPreRelease)
		{
			if (ReferenceEquals(preRelease, otherPreRelease)) return 0;
			if (null == preRelease) return 1;
			if (null == otherPreRelease) return -1;
			return ComparePreReleaseFields(preRelease.Split('.'), otherPreRelease.Split('.'));
		}

		private static int ComparePreReleaseFields(string[] fields, string[] otherFields)
		{
			int compareTo = fields.Length.CompareTo(otherFields.Length);
			if (compareTo != 0) return compareTo;
			if (fields.Length > otherFields.Length) return 1;
			for (int i = 0; i < fields.Length; i++)
			{
				if (String.Equals(fields[i], otherFields[i], StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				bool leftIsNumeric = Int64.TryParse(fields[i], out long leftNumericValue);
				bool rightIsNumeric = Int64.TryParse(otherFields[i], out long rightNumericValue);
				if (leftIsNumeric && !rightIsNumeric)
				{
					return -1;
				}
				if (!leftIsNumeric && rightIsNumeric)
				{
					return 1;
				}
				if (leftIsNumeric && rightIsNumeric)
				{
					compareTo = leftNumericValue.CompareTo(rightNumericValue);
					if (compareTo == 0)
					{
						continue;
					}
					else
					{
						return compareTo;
					}
				}
				return String.CompareOrdinal(fields[i], otherFields[i]);
			}
			return 0;
		}
	}
}