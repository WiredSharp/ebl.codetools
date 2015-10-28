using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeTools.Core
{
    internal static class PathHelpers
    {
        public static string Combine(string folder, string path)
        {
            return ResolveRelativePath(System.IO.Path.Combine(folder, path));
        }

        public static string ResolveRelativePath(string path)
        {
            if (path.Contains(".."))
            {
                var pathItems = path.Split('\\');
                var newPath = new List<string>();
                for (int i = 0; i < pathItems.Length; i++)
                {
                    if (pathItems[i] == "..")
                    {
                        newPath.RemoveAt(newPath.Count - 1);
                    }
                    else
                    {
                        newPath.Add(pathItems[i]);
                    }
                }
                path = Regex.Replace(System.IO.Path.Combine(newPath.ToArray()), @":([^\\\\])", @":\$1");
            }
            path = path.Replace(".\\", "");
            return path;
        }
    }
}