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
			private const string DEFAULT_BUILD_DIR = @"$(SolutionDir)..\";

#if USE_PLATFORM
			private const string CURRENT_BUILD_DIR = @"$(BuildDir)$(SolutionName)\$(Configuration).$(Platform)\";
#else
			private const string CURRENT_BUILD_DIR = @"$(BuildDir)$(SolutionName)\$(Configuration)\";
#endif

			private const string ACCEPTANCE_TEST_OUTPUT_PATH = CURRENT_BUILD_DIR + @"acceptance\$(AssemblyName)";
			private const string TEST_OUTPUT_PATH = CURRENT_BUILD_DIR + @"tests\$(AssemblyName)";
			private const string BIN_OUTPUT_PATH = CURRENT_BUILD_DIR + @"bin\$(AssemblyName)";
			private const string INTERMEDIATE_OUTPUT_PATH = CURRENT_BUILD_DIR + @"obj\$(AssemblyName)";

			private static readonly Lazy<Settings> DEFAULT = new Lazy<Settings>();

			public static Settings Default
			{
				get { return DEFAULT.Value; }
			}

			public string AcceptanceTestOutputPath { get; set; }

			public string TestOutputPath { get; set; }

			public string BinOutputPath { get; set; }

			public string IntermediateOutputPath { get; set; }

			public string BuildPath { get; set; }

			public Settings()
			{
				AcceptanceTestOutputPath = ACCEPTANCE_TEST_OUTPUT_PATH;
				TestOutputPath = TEST_OUTPUT_PATH;
				BinOutputPath = BIN_OUTPUT_PATH;
				IntermediateOutputPath = INTERMEDIATE_OUTPUT_PATH;
				BuildPath = DEFAULT_BUILD_DIR;
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
			commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(BUILD_DIRECTORY_TAG),
				new XAttribute("Condition", " '$(BuildDir)' == '' "), CurrentSettings.BuildPath));
			string assemblyName = commonPropertyGroup.Elements(defaultNamespace.GetName(ASSEMBLY_NAME_TAG)).Single().Value;
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
			commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(OUTPUT_PATH_TAG), outputPath));
			commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG), CurrentSettings.IntermediateOutputPath));
			return root.ToString();
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