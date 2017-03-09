#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (14:41)
// ///
// ///
#endregion

using System;

namespace CodeTools.MSBuild.Helpers.VisualStudio
{
    public class NetFramework
    {
        public string MsbuildTag { get; }
        public string NugetFolder { get; }

        public static NetFramework v35 = new NetFramework("v3.5", "net35");
        public static NetFramework v40 = new NetFramework("v4.0", "net40");
        public static NetFramework v45 = new NetFramework("v4.5", "net45");
        public static NetFramework v452 = new NetFramework("v4.5.2", "net452");
        public static NetFramework v461 = new NetFramework("v4.6.1", "net461");
        public static NetFramework v462 = new NetFramework("v4.6.2", "net462");

        public NetFramework(string msbuildTag, string nugetFolder)
        {
            if (msbuildTag == null) throw new ArgumentNullException(nameof(msbuildTag));
            if (nugetFolder == null) throw new ArgumentNullException(nameof(nugetFolder));
            MsbuildTag = msbuildTag;
            NugetFolder = nugetFolder;
        }

        public static bool AreEqual(NetFramework lhs, NetFramework rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            if (null == lhs || null == rhs)
            {
                return false;
            }
            return lhs.MsbuildTag == rhs.MsbuildTag;
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as NetFramework);
        }

        public override int GetHashCode()
        {
            return MsbuildTag.GetHashCode();
        }

        public override string ToString()
        {
            return $".Net framework {MsbuildTag}";
        }
    }
}