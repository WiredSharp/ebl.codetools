using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticVersioning
{
	public static class SemanticVersionExtensions
	{
		/// <summary>
		/// create a new patch version
		/// </summary>
		/// <param name="version"></param>
		/// <param name="update"></param>
		/// <returns></returns>
		public static SemanticVersion Bump(this SemanticVersion version, int update = 1)
		{
			return Bump(version, VersionField.Patch, update);
		}

		/// <summary>
		/// create a new version by updating field <paramref name="field"/>
		/// </summary>
		/// <param name="version"></param>
		/// <param name="field"></param>
		/// <param name="update"></param>
		/// <returns></returns>
		public static SemanticVersion Bump(this SemanticVersion version, VersionField field, int update)
		{
			switch (field)
			{
				case VersionField.Major:
					return new SemanticVersion(version.Major+update, version.Minor, version.Patch);
				case VersionField.Minor:
					return new SemanticVersion(version.Major, version.Minor + update, version.Patch);
				case VersionField.Patch:
					return new SemanticVersion(version.Major, version.Minor, version.Patch + update);
				default:
					throw new ArgumentOutOfRangeException(nameof(field), $"{field}: field not supported");
			}
		}


		/// <summary>
		/// create a new version by updating field <paramref name="field"/>
		/// </summary>
		/// <param name="version"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public static int GetField(this SemanticVersion version, VersionField field)
		{
			switch (field)
			{
				case VersionField.Major:
					return version.Major;
				case VersionField.Minor:
					return version.Minor;
				case VersionField.Patch:
					return version.Patch;
				default:
					throw new ArgumentOutOfRangeException(nameof(field), $"{field}: field not supported");
			}
		}
	}
}
