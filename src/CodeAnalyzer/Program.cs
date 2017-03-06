using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CodeTools.Core;

namespace CodeAnalyzer
{
	class Program
	{
		static void Main(string[] args)
		{
			Trace.Listeners.Add(new ConsoleTraceListener(true));
			Trace.Listeners.Add(new TextWriterTraceListener("CodeAnalyzer.log", "codeanalyzer") { TraceOutputOptions = TraceOptions.DateTime });
			Trace.UseGlobalLock = false;
			try
			{
				//File.Delete(Path.Combine(args[0], "buildAll.bat"));
				//MergeFiles(Path.Combine(args[0], "buildAll.bat")
				//    , Directory.EnumerateFiles(args[0], "build.bat", SearchOption.AllDirectories));
				//File.AppendAllLines(Path.Combine(args[0], "buildAll.bat"),
				//    Directory.EnumerateFiles(args[0], "build.bat", SearchOption.AllDirectories)
				//        .Select(f => "cd \"" + System.IO.Path.GetDirectoryName(f) + "\"" + Environment.NewLine 
				//                        + "call \"" + f + "\""));
				//YumlBuilder.LoadPOSTImage(File.ReadAllText(args[0]), "solutions.png");
				ListSolutionsAndProjects(args);
			}
			catch (Exception ex)
			{
				Trace.TraceError("execution failure: " + ex);
			}
			Trace.Flush();
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		private static void MergeFiles(string target, IEnumerable<string> sourceFiles)
		{
			foreach (string sourceFile in sourceFiles)
			{
				File.AppendAllText(target, File.ReadAllText(sourceFile));
			}
		}

		private static void ListSolutionsAndProjects(string[] args)
		{
			//Solution[] solutions = GetSolutions(args[0]);
			string solutionFolder = Path.GetDirectoryName(args[0]);
			File.WriteAllLines(Path.Combine(solutionFolder, "files.txt"), 
				Directory.GetFiles(solutionFolder, "*.cs", SearchOption.AllDirectories)
				.Union(Directory.GetFiles(solutionFolder, "*.cpp", SearchOption.AllDirectories))
				.Union(Directory.GetFiles(solutionFolder, "*.hpp", SearchOption.AllDirectories))
				.Union(Directory.GetFiles(solutionFolder, "*.h", SearchOption.AllDirectories))
				.Union(Directory.GetFiles(solutionFolder, "*.rc", SearchOption.AllDirectories))
				.Union(Directory.GetFiles(solutionFolder, "*.rc2", SearchOption.AllDirectories))
				.Union(Directory.GetFiles(solutionFolder, "*.def", SearchOption.AllDirectories))
				//.Select(s => s.Replace(solutionFolder + "\\", String.Empty))
				.OrderBy(n => n));
			//File.WriteAllLines(Path.Combine(solutionFolder, "projects.txt"), Directory.GetFiles(solutionFolder, "*.*proj", SearchOption.AllDirectories).OrderBy(n => n));
			Solution s = Solution.Parse(args[0]);
			//File.WriteAllLines(Path.Combine(solutionFolder,Path.GetFileNameWithoutExtension(args[0]) + ".projects.txt"), GetProjectPathes(s));
			//File.WriteAllLines(Path.Combine(solutionFolder, Path.GetFileNameWithoutExtension(args[0]) + ".projects.references.txt"), GetProjectReferences(s));
			File.WriteAllLines(Path.Combine(solutionFolder, Path.GetFileNameWithoutExtension(args[0]) + ".projects.items.txt"), GetProjectItems(s));
			//BuildYumlDiagram(Path.Combine(solutionFolder, "solutions.png"), s);
			//BuildGraphVizDiagram(solutions, Path.Combine(args[0],"solutions.gv"));
			//File.WriteAllText(Path.Combine(args[0],"solutions.json"), JsonConvert.SerializeObject(solutions, Formatting.Indented));
			//Console.WriteLine(solutions.Display());
		}

		private static IEnumerable<string> GetProjectItems(Solution solution)
		{
			return
				solution.CSharpProjects.Values.SelectMany(csp => csp.Items)
					.Select(item => item.Path)
					.Union(solution.CppProjects.Values.SelectMany(csp => csp.Items).Select(item => item.Path))
					.Distinct()
					.OrderBy(n => n);
		}

		private static IEnumerable<string> GetProjectReferences(Solution solution)
		{
			return solution.CSharpProjects.SelectMany(csp => GetProjectReferences(csp.Value)).Select(r => r.Path.Path).OrderBy(n => n).Distinct();
		}

		private static IEnumerable<Reference> GetProjectReferences(CSharpProject csProject)
		{
			return csProject.References;
		}

		private static IEnumerable<string> GetProjectPathes(Solution s)
		{
			return s.CSharpProjects.Select(csp => csp.Value.Path.Path).Union(s.CppProjects.Select(cp => cp.Value.Path.Path)).OrderBy(n => n);
		}

		private static void BuildGraphVizDiagram(string outputDiagramFile, params Solution[] solutions)
		{
			GraphVizBuilder.CreateProjectDiagram(solutions, outputDiagramFile);
		}

		private static void BuildYumlDiagram(string outputDiagramFile, params Solution[] solutions)
		{
			YumlBuilder.CreateProjectDiagram(solutions, outputDiagramFile);
		}

		private static IEnumerable<Solution> GetSolutions(string root)
		{
			return Directory.GetFiles(root, "*.sln", SearchOption.AllDirectories).Select(Solution.Parse);
		}
	}
}
