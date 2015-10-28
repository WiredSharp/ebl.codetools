using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodeAnalyzer;

namespace VSProjectNormalizer
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				ShowHelp();
			}
			else
			{
				try
				{
					string fileName;
					if (String.IsNullOrWhiteSpace(args[0]))
					{
						ShowHelp();
						return;
					}
					else if (Path.IsPathRooted(args[0]))
					{
						fileName = args[0];
					}
					else
					{
						fileName = Path.Combine(Environment.CurrentDirectory, args[0]);
					}

					if (fileName.EndsWith(".csproj"))
					{
						File.WriteAllText(fileName, NewNormalizer().Normalize(fileName));
					}
					else if (fileName.EndsWith(".sln"))
					{
						VSProjectNormalizer normalizer = NewNormalizer();
						var solution = Solution.Parse(fileName);
						foreach (KeyValuePair<string, CSharpProject> reference in solution.CSharpProjects)
						{
							Console.WriteLine("converting project file '" + reference + "'");
							string path = reference.Value.Path;
							File.WriteAllText(path, normalizer.Normalize(path));
						}
					}
					Console.WriteLine("Work complete !");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					Console.WriteLine("Press any key to continue...");
					try
					{
						Console.ReadKey();
					}
					catch (InvalidOperationException)
					{ }
				}
			}
		}

		private static void ShowHelp()
		{
			var name = Assembly.GetEntryAssembly().GetName().Name;
			Console.WriteLine("Visual Studio project file converter v{0} (c) 2011 Natixis AM", Assembly.GetExecutingAssembly().GetName().Version);
			Console.WriteLine(name + " <projectfile.csproj> : convert specified project file");
			Console.WriteLine(name + " <solutionfile.sln> : convert all project files in solution");
		}

		private static VSProjectNormalizer NewNormalizer()
		{
			var normalizer = new VSProjectNormalizer(new VSProjectNormalizer.Settings()
			{
				TestOutputPath = Resource.Default.TEST_OUTPUT_PATH
				,
				AcceptanceTestOutputPath = Resource.Default.ACCEPTANCE_TEST_OUTPUT_PATH
				,
				BinOutputPath = Resource.Default.BIN_OUTPUT_PATH
				,
				IntermediateOutputPath = Resource.Default.INTERMEDIATE_OUTPUT_PATH
			});
			return normalizer;
		}
	}
}
