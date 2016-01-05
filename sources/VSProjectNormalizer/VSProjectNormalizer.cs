using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace VSProjectNormalizer
{
	public class VSProjectNormalizer
	{
		private const string OUTPUT_PATH_TAG = "OutputPath";
		private const string BUILD_DIRECTORY_TAG = "BuildDir";
		private const string ASSEMBLY_NAME_TAG = "AssemblyName";
		private const string INTERMEDIATE_OUTPUT_TAG = "IntermediateOutputPath";

		public class Settings
		{
			private const string WITH_PLATFORM = @".$(Platform)\";
			private const string DEFAULT_BUILD_FOLDER = @"$(SolutionDir)$(Configuration)";
			private const string SPECIFIC_BUILD_FOLDER = @"$(BuildDir)$(SolutionName)\$(Configuration)";

			private const string ACCEPTANCE_TEST_OUTPUT_PATH = @"acceptance\$(AssemblyName)";
			private const string TEST_OUTPUT_PATH = @"tests\$(AssemblyName)";
			private const string BIN_OUTPUT_PATH = @"bin\$(AssemblyName)";
			private const string INTERMEDIATE_OUTPUT_PATH = @"obj\$(AssemblyName)";

			private static readonly Lazy<Settings> DEFAULT = new Lazy<Settings>();
			private string _acceptanceTestOutputPath;
			private string _testOutputPath;
			private string _binOutputPath;
			private string _intermediateOutputPath;

			public static Settings Default
			{
				get { return DEFAULT.Value; }
			}

			public bool UsePlatform { get; set; }

			public string BuildPrefix
			{
				get { return UsePlatform ? SPECIFIC_BUILD_FOLDER + WITH_PLATFORM : SPECIFIC_BUILD_FOLDER + @"\"; }
			}

			public string DefaultBuildPrefix
			{
				get { return UsePlatform ? DEFAULT_BUILD_FOLDER + WITH_PLATFORM : DEFAULT_BUILD_FOLDER + @"\"; }
			}

			public string AcceptanceTestOutputPath
			{
				get
				{
					return String.IsNullOrEmpty(_acceptanceTestOutputPath) ? ACCEPTANCE_TEST_OUTPUT_PATH : _acceptanceTestOutputPath;
				}
				set { _acceptanceTestOutputPath = value; }
			}

			public string TestOutputPath
			{
				get
				{
					return String.IsNullOrEmpty(_testOutputPath) ? TEST_OUTPUT_PATH : _testOutputPath;
				}
				set { _testOutputPath = value; }
			}

			public string BinOutputPath
			{
				get
				{
					return String.IsNullOrEmpty(_binOutputPath) ? BIN_OUTPUT_PATH : _binOutputPath;
				}
				set { _binOutputPath = value; }
			}

			public string IntermediateOutputPath
			{
				get
				{
					return String.IsNullOrEmpty(_intermediateOutputPath) ? INTERMEDIATE_OUTPUT_PATH : _intermediateOutputPath;
				}
				set { _intermediateOutputPath = value; }
			}

			public string BuildPath { get; set; }

			public Settings()
			{
				AcceptanceTestOutputPath = ACCEPTANCE_TEST_OUTPUT_PATH;
				TestOutputPath = TEST_OUTPUT_PATH;
				BinOutputPath = BIN_OUTPUT_PATH;
				IntermediateOutputPath = INTERMEDIATE_OUTPUT_PATH;
			}
		}

		protected Settings CurrentSettings { get; set; }

		public VSProjectNormalizer()
			: this(Settings.Default)
		{
		}

		public VSProjectNormalizer(Settings settings)
		{
			if (settings == null) throw new ArgumentNullException("settings");
			CurrentSettings = settings;
		}

		public string Normalize(TextReader projectFileReader)
		{
			return Normalize(XElement.Load(projectFileReader));			
		}

		public string Normalize(FileInfo projectFile)
		{
			return Normalize(XElement.Load(projectFile.FullName));
		}

		public string Normalize(string projectFileContent)
		{
			return Normalize(XElement.Parse(projectFileContent));
		}

		private string Normalize(XElement root)
		{
			XNamespace defaultNamespace = root.GetDefaultNamespace();
			RemoveNodes(root, OUTPUT_PATH_TAG, BUILD_DIRECTORY_TAG, INTERMEDIATE_OUTPUT_TAG);
			XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
			var buildTagExists = new XAttribute("Condition", " '$(" + BUILD_DIRECTORY_TAG + ")' != '' ");
			var buildTagNotExists = new XAttribute("Condition", " '$(" + BUILD_DIRECTORY_TAG + ")' == '' ");
			if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
			{
				commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(BUILD_DIRECTORY_TAG),
					buildTagExists, CurrentSettings.BuildPath));
			}
			string assemblyName = commonPropertyGroup.Elements(defaultNamespace.GetName(ASSEMBLY_NAME_TAG)).Single().Value;
			string outputPath = BuildOutputPath(assemblyName);
			if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
			{
				commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(OUTPUT_PATH_TAG), CurrentSettings.BuildPrefix + outputPath)
					, new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG), CurrentSettings.BuildPrefix + CurrentSettings.IntermediateOutputPath));
			}
			else
			{
				commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(OUTPUT_PATH_TAG)
					, buildTagExists, CurrentSettings.BuildPrefix + outputPath)
				, new XElement(defaultNamespace.GetName(OUTPUT_PATH_TAG)
					, buildTagNotExists, CurrentSettings.DefaultBuildPrefix + outputPath)
				, new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG)
					, buildTagExists, CurrentSettings.BuildPrefix + CurrentSettings.IntermediateOutputPath)
				, new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG)
					, buildTagNotExists, CurrentSettings.DefaultBuildPrefix + CurrentSettings.IntermediateOutputPath));
			}
			return root.ToString();
		}

		private string BuildOutputPath(string assemblyName)
		{
			string outputPath;
			if (assemblyName.EndsWith("Acceptance.Test", StringComparison.CurrentCultureIgnoreCase)
			    || assemblyName.EndsWith("Acceptance.Tests", StringComparison.CurrentCultureIgnoreCase)
			    || assemblyName.EndsWith("Acceptance", StringComparison.CurrentCultureIgnoreCase))
			{
				outputPath = CurrentSettings.AcceptanceTestOutputPath;
			}
			else if (assemblyName.EndsWith(".Test", StringComparison.CurrentCultureIgnoreCase)
			         || assemblyName.EndsWith(".Tests", StringComparison.CurrentCultureIgnoreCase))
			{
				outputPath = CurrentSettings.TestOutputPath;
			}
			else
			{
				outputPath = CurrentSettings.BinOutputPath;
			}
			return outputPath;
		}

		protected void RemoveNodes(XContainer root, params string[] nodeNames)
		{
			if (nodeNames == null) throw new ArgumentNullException("nodeNames");
			if (nodeNames.Length == 0) return;
			var toRemove = new List<XElement>();
			foreach (
				 XElement node in
					  root.DescendantNodes().OfType<XElement>().Where(
							node => nodeNames.Contains(node.Name.LocalName)))
			{
				Debug.WriteLine(node.Name + "='" + node.Value);
				toRemove.Add(node);
			}
			foreach (XElement node in toRemove)
			{
				node.Remove();
			}
		}

		protected XElement GetFirstCommonPropertyGroup(XContainer root)
		{
			return
				 root.Elements().
					  FirstOrDefault(node => !node.HasAttributes && node.Name.ToString().EndsWith("PropertyGroup"));
		}
	}
}