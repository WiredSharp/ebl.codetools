using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CodeTools.Core.Projects;

namespace CodeTools.Core
{
	public class Solution
	{
		public string Name { get; private set; }

		public string Path { get; private set; }

		public string Version { get; private set; }

		public IReadOnlyDictionary<string, CSharpProject> CSharpProjects { get; private set; }

		public IReadOnlyDictionary<string, CppProject> CppProjects { get; private set; }

	    protected Solution()
	    {}

		public static Solution Parse(string file)
		{
			var solution = new Solution
			{
				Name = System.IO.Path.GetFileNameWithoutExtension(file)
				,Path = file
			};
			solution.ReadProjects(file);
			return solution;
		}

		private void ReadProjects(string file)
		{
			if (String.IsNullOrEmpty(file)) throw new ArgumentException("file");
			string folder = System.IO.Path.GetDirectoryName(file);
			var cSharpProjects = new Dictionary<string, CSharpProject>();
			var cppProjects = new Dictionary<string, CppProject>();
			foreach (string line in File.ReadAllLines(file))
			{
				if (line.StartsWith("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"))
				{
					string[] items = line.Split(',');
					string projectName = items[0].Split('=')[1].Trim().Trim('"');
					string projectPath = PathHelpers.Combine(folder, items[1].Trim().Trim('"'));
					if (projectPath.StartsWith(".."))
					{
						projectPath = System.IO.Path.Combine(Path, projectPath);
					}
					try
					{
						cSharpProjects.Add(projectName, CSharpProject.Parse(new FileInfo(projectPath)));
					}
					catch (Exception ex)
					{
						Trace.TraceError("parsing '" + file + "' : " + projectName + ": " + ex.Message + " (" + projectPath + ")");
						cSharpProjects.Add(projectName, new CSharpProject(new FileInfo(projectPath)));
					}
				}
				else if (line.StartsWith("Microsoft Visual Studio Solution File"))
				{
					int start = line.IndexOf("Format Version");
					if (start > 0)
						Version = line.Substring(start + "Format Version".Length + 1);
				}
				else if (line.StartsWith("Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"))
				{
					string[] items = line.Split(',');
					string projectName = items[0].Split('=')[1].Trim().Trim('"');
					string projectPath = PathHelpers.Combine(folder, items[1].Trim().Trim('"'));
					if (projectPath.StartsWith(".."))
					{
						projectPath = System.IO.Path.Combine(Path, projectPath);
					}
					try
					{
						cppProjects.Add(projectName, CppProject.Parse(new FileInfo(projectPath)));
					}
					catch (Exception ex)
					{
						Trace.TraceError("parsing '" + file + "' : " + projectName + ": " + ex.Message + " (" + projectPath + ")");
						cppProjects.Add(projectName, new CppProject(new FileInfo(projectPath)));
					}
				}
				else
				{
					//Trace.TraceInformation("skipping line: '" + line + "'");
				}
			}
			CSharpProjects = cSharpProjects;
			CppProjects = cppProjects;
		}

		public override string ToString()
		{
			return "Solution " + Name;
		}
	}
}