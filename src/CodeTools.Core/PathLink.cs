using System;
using System.IO;

namespace CodeTools.Core
{
	public struct PathLink
    {
        public readonly FileInfo Path;

        public bool Valid => Path != null && Path.Exists;

        public PathLink(string path)
            :this(new FileInfo(path))
        {
            
        }

        public PathLink(FileInfo path)
            : this()
        {
            Path = path;
        }

        public static implicit operator string(PathLink path)
        {
            return path.Path?.FullName;
        }
        public override string ToString()
        {
            return Path?.FullName ?? String.Empty;
        }
    }
}