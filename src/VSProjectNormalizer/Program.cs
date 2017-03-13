using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodeTools.Core;
using CodeTools.Core.Projects;

namespace CodeTools.VisualStudio.Tools
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
                        File.WriteAllText(fileName, NewNormalizer().Normalize(new FileInfo(fileName)));
                    }
                    else if (fileName.EndsWith(".sln"))
                    {
                        VSProjectNormalizer normalizer = NewNormalizer();
                        var solution = Solution.Parse(fileName);
                        foreach (KeyValuePair<string, CSharpProject> reference in solution.CSharpProjects)
                        {
                            Console.WriteLine("converting project file '" + reference + "'");
                            string path = reference.Value.Path;
                            File.WriteAllText(path, normalizer.Normalize(new FileInfo(path)));
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
            Console.WriteLine("Visual Studio project file converter v{0} (c) 2016 OFI AM", GetAttribute<AssemblyInformationalVersionAttribute>(Assembly.GetExecutingAssembly()).InformationalVersion);
            Console.WriteLine(name + " <projectfile.csproj> : convert specified project file");
            Console.WriteLine(name + " <solutionfile.sln> : convert all project files in solution");
        }

        private static T GetAttribute<T>(Assembly assembly) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(assembly, typeof(T));
        }

        private static VSProjectNormalizer NewNormalizer()
        {
            var normalizer = new VSProjectNormalizer(new Settings()
            {
                SolutionBuildFolder = Resource.Default.SOLUTION_BUILD_FOLDER,
                SolutionIntermediateFolder = Resource.Default.SOLUTION_INTERMEDIATE_FOLDER,
                ProjectBuildFolder = Resource.Default.PROJECT_BUILD_FOLDER,
                ProjectIntermediateFolder = Resource.Default.PROJECT_INTERMEDIATE_FOLDER,
                CommonPropsFile = Resource.Default.SOLUTIONDIR_BUILD_COMMON_PROPS
            });
            return normalizer;
        }
    }
}
