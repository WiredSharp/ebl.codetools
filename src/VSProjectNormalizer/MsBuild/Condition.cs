#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-08 (15:04)
// ///
// ///
#endregion

using System;
using System.Xml.Linq;

namespace VSProjectNormalizer.MsBuild
{
    internal static class Condition
    {
        public static XAttribute Equal(string lhs, string rhs)
        {
            return NewCondition(lhs, "==", rhs);
        }

        public static XAttribute NotEqual(string lhs, string rhs)
        {
            return NewCondition(lhs, "!=", rhs);
        }

        public static XAttribute TagDefined(string tag)
        {
            return TagNotEqual(tag, "");
        }

        public static XAttribute TagEqual(string tag, string value)
        {
            return Equal($"$({tag})", value);
        }

        public static XAttribute TagNotDefined(string tag)
        {
            return TagEqual(tag, "");
        }

        public static XAttribute TagNotEqual(string tag, string value)
        {
            return NotEqual($"$({tag})", value);
        }

        public static XAttribute And(string lhs, string rhs)
        {
            return new XAttribute("Condition", $"({lhs}) AND ({rhs})");
        }

        public static XAttribute And(XAttribute lhs, XAttribute rhs)
        {
            if (lhs == null) throw new ArgumentNullException(nameof(lhs));
            if (rhs == null) throw new ArgumentNullException(nameof(rhs));
            return new XAttribute("Condition", $"({lhs.Value}) AND ({rhs.Value})");
        }

        private static XAttribute NewCondition(string lhs, string op, string rhs)
        {
            return new XAttribute("Condition", $"'{lhs}'{op}'{rhs}'");
        }
    }
}