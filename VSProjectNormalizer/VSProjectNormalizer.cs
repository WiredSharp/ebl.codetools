using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace VSProjectNormalizer
{
	public class VSProjectNormalizer
	{
		public class Settings
		{
#if USE_PLATFORM
        private const string ACCEPTANCE_TEST_OUTPUT_PATH = @"$(SolutionDir)$(Configuration).$(Platform)\Acceptance\Libraries\$(AssemblyName)";
        private const string TEST_OUTPUT_PATH = @"$(SolutionDir)$(Configuration).$(Platform)\tests\$(AssemblyName)";
        private const string BIN_OUTPUT_PATH = @"$(SolutionDir)$(Configuration).$(Platform)\bin\$(AssemblyName)";
        private const string INTERMEDIATE_OUTPUT_PATH = @"$(SolutionDir)$(Configuration).$(Platform)\obj\$(AssemblyName)";
#else
			private const string ACCEPTANCE_TEST_OUTPUT_PATH = @"$(SolutionDir)Acceptance\$(Configuration)\Libraries\$(AssemblyName)";
			private const string TEST_OUTPUT_PATH = @"$(SolutionDir)$(Configuration)\tests\$(AssemblyName)";
			private const string BIN_OUTPUT_PATH = @"$(SolutionDir)$(Configuration)\bin\$(AssemblyName)";
			private const string INTERMEDIATE_OUTPUT_PATH = @"$(SolutionDir)$(Configuration)\obj\$(AssemblyName)";
#endif

			private static readonly Lazy<Settings> DEFAULT = new Lazy<Settings>();

			public static Settings Default
			{
				get { return DEFAULT.Value; }
			}

			public string AcceptanceTestOutputPath { get; set; }

			public string TestOutputPath { get; set; }

			public string BinOutputPath { get; set; }

			public string IntermediateOutputPath { get; set; }

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
			:this(Settings.Default)
		{
		}

		public VSProjectNormalizer(Settings settings)
		{
			if (settings == null) throw new ArgumentNullException("settings");
			CurrentSettings = settings;
		}

		public string Normalize(string projectFile)
		{
			XElement root = XElement.Load(projectFile);
			XNamespace defaultNamespace = root.GetDefaultNamespace();
			RemoveOutputPathNodes(root);
			XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
			string assemblyName = commonPropertyGroup.Elements(defaultNamespace.GetName("AssemblyName")).Single().Value;
			string outputPath;
			if (assemblyName.EndsWith("Acceptance.Test", StringComparison.CurrentCultureIgnoreCase) || assemblyName.EndsWith("Acceptance.Tests", StringComparison.CurrentCultureIgnoreCase) || assemblyName.EndsWith("Acceptance", StringComparison.CurrentCultureIgnoreCase))
			{
				outputPath = CurrentSettings.AcceptanceTestOutputPath;
			}
			else if (assemblyName.EndsWith(".Test", StringComparison.CurrentCultureIgnoreCase) || assemblyName.EndsWith(".Tests", StringComparison.CurrentCultureIgnoreCase))
			{
				outputPath = CurrentSettings.TestOutputPath;
			}
			else
			{
				outputPath = CurrentSettings.BinOutputPath;
			}
			commonPropertyGroup.Add(new XElement(defaultNamespace.GetName("OutputPath"), outputPath));
			commonPropertyGroup.Add(new XElement(defaultNamespace.GetName("IntermediateOutputPath"), CurrentSettings.IntermediateOutputPath));
			return root.ToString();
		}

		protected void RemoveOutputPathNodes(XContainer root)
		{
			var toRemove = new List<XElement>();
			foreach (
				 XElement outputPathNode in
					  root.DescendantNodes().OfType<XElement>().Where(
							outputPathNode => outputPathNode.Name.ToString().EndsWith("OutputPath")))
			{
				Debug.WriteLine(outputPathNode.Name + "='" + outputPathNode.Value);
				toRemove.Add(outputPathNode);
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