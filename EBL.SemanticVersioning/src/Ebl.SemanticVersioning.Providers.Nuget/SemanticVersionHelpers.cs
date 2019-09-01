// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System;

namespace SemanticVersioning
{
	internal static class SemanticVersionHelpers
	{
		public static SemanticVersion Parse(
			string majorValue
			, string minorValue
			, string patchValue
			, string preRelease = null
			, string metadata = null
			)
		{
			if (Int32.TryParse(majorValue, out int major))
			{
				if (Int32.TryParse(minorValue, out int minor))
				{
					if (Int32.TryParse(patchValue, out int patch))
					{
						return new SemanticVersion(major, minor, patch, metadata, preRelease);
					}
					else
					{
						throw new ArgumentException($"{patchValue}: invalid minor field value", nameof(patchValue));
					}
				}
				else
				{
					throw new ArgumentException($"{minorValue}: invalid minor field value", nameof(minorValue));
				}
			}
			else
			{
				throw new ArgumentException($"{majorValue}: invalid major field value", nameof(majorValue));
			}
		}
	}
}