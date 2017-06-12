#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-15 (09:52)
// ///
// ///
#endregion

using System;
using System.Text;

namespace CodeTools.Helpers.Core
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            var output = new StringBuilder(new string(Char.ToLower(input[0]), 1));
            if (input.Length > 1)
            {
                output.Append(input.Substring(1));
            }
            return output.ToString();
        }
    }
}