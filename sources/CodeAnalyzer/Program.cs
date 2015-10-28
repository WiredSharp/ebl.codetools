using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
			Solution[] solutions = GetSolutions(args[0]);
			BuildYumlDiagram(solutions, Path.Combine(args[0], "solutions.png"));
			//BuildGraphVizDiagram(solutions, Path.Combine(args[0],"solutions.gv"));
			//File.WriteAllText(Path.Combine(args[0],"solutions.json"), JsonConvert.SerializeObject(solutions, Formatting.Indented));
			//Console.WriteLine(solutions.Display());
		}

		private static void BuildGraphVizDiagram(IEnumerable<Solution> solutions, string outputDiagramFile)
		{
			GraphVizBuilder.CreateProjectDiagram(solutions, outputDiagramFile);
		}

		private static void BuildYumlDiagram(IEnumerable<Solution> solutions, string outputDiagramFile)
		{
			YumlBuilder.CreateProjectDiagram(solutions, outputDiagramFile);
		}

		private static Solution[] GetSolutions(string root)
		{
			return Directory.GetFiles(root, "*.sln", SearchOption.AllDirectories).Select(Solution.Parse).ToArray();
		}
	}
}
