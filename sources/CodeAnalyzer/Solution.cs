using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeAnalyzer
{
    public class Solution
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public IReadOnlyDictionary<string,Project> Projects { get; private set; }
        public static Solution Parse(string file)
        {
            var solution = new Solution
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(file)
                ,Path = file
                ,Projects = ReadProjects(file)
            };
            return solution;
        }

        private static IReadOnlyDictionary<string, Project> ReadProjects(string file)
        {
            if (String.IsNullOrEmpty(file)) throw new ArgumentException("file");
            string folder = System.IO.Path.GetDirectoryName(file);
            var projects = new Dictionary<string, Project>();
            foreach (string[] items in File.ReadAllLines(file)
                .Where(l => l.StartsWith("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"))
                .Select(line => line.Split(',')))
            {
                string projectName = items[0].Split('=')[1].Trim().Trim('"');
                string projectPath = PathHelpers.Combine(folder, items[1].Trim().Trim('"'));
                try
                {
                    projects.Add(projectName, Project.Parse(projectPath));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(System.IO.Path.GetFileNameWithoutExtension(file) + ": " + projectName + ": " + ex.Message + " (" + projectPath + ")");
                    projects.Add(projectName, new Project(projectPath));
                }
                
            }
            return projects;
        }

        public override string ToString()
        {
            return "Solution " + Name;
        }
    }
}