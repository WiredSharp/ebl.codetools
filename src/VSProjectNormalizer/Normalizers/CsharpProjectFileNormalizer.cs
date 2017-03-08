using System;
using System.Linq;
using System.Xml.Linq;

namespace VSProjectNormalizer.Normalizers
{
	internal class CsharpProjectFileNormalizer : ProjectFileNormalizer
	{
	    private const string SOLUTION_DIR_TAG = "SolutionDir";
	    private const string SOLUTION_NAME_TAG = "SolutionName";
	    private const string OUTPUT_PATH_TAG = "OutputPath";
        private const string ASSEMBLY_NAME_TAG = "AssemblyName";

		public CsharpProjectFileNormalizer(Settings settings) : base(settings)
		{
		}

		public void Normalize(XElement root)
		{
			XNamespace defaultNamespace = root.GetDefaultNamespace();
			RemoveNodes(root, OUTPUT_PATH_TAG, BUILD_DIRECTORY_TAG, INTERMEDIATE_OUTPUT_TAG);
			XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
			XAttribute buildTagExists = ExistCondition(BUILD_DIRECTORY_TAG);
			XAttribute buildTagNotExists = NotExistCondition(BUILD_DIRECTORY_TAG);
            if (!FindNodeByName(root, SOLUTION_DIR_TAG).Any())
            {
                commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(SOLUTION_DIR_TAG), EqualCondition($"$({SOLUTION_DIR_TAG})", ""), @"..\.."));
            }
            if (!FindNodeByName(root, SOLUTION_NAME_TAG).Any())
            {
                commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(SOLUTION_NAME_TAG), EqualCondition($"$({SOLUTION_NAME_TAG})", ""), @"$(AssemblyName)"));
            }
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
		}
	}
}
