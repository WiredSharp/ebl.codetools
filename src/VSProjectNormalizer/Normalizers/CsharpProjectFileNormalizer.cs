﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VSProjectNormalizer.MsBuild;

namespace VSProjectNormalizer.Normalizers
{
	internal class CsharpProjectFileNormalizer : ProjectFileNormalizer
	{
	    protected const string SOLUTION_DIR_TAG = "SolutionDir";
        protected const string SOLUTION_NAME_TAG = "SolutionName";
        protected const string OUTPUT_PATH_TAG = "OutputPath";
        protected const string ASSEMBLY_NAME_TAG = "AssemblyName";
        protected const string VSPROJECTNORMALIZER_LABEL = "vsprojectnormalizer.outputpath";

	    public CsharpProjectFileNormalizer(Settings settings) : base(settings)
		{
		}

		public virtual void Normalize(XElement root)
		{
            RemoveNodes(root, OUTPUT_PATH_TAG, INTERMEDIATE_OUTPUT_TAG);
		    if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
		    {
		        RemoveNodes(root, BUILD_DIRECTORY_TAG);
		    }
		    UpdateSolutionProperties(root);
		    HandleOutputPath(root);
		}

	    protected virtual void UpdateSolutionProperties(XElement root)
	    {
            XNamespace defaultNamespace = root.GetDefaultNamespace();
            XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
            if (!FindNodeByName(root, SOLUTION_DIR_TAG).Any())
	        {
	            commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(SOLUTION_DIR_TAG),
	                                                 Condition.TagNotDefined(SOLUTION_DIR_TAG), CurrentSettings.DefaultSolutionDir));
	        }
	        if (!FindNodeByName(root, SOLUTION_NAME_TAG).Any())
	        {
	            commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(SOLUTION_NAME_TAG),
	                                                 Condition.TagNotDefined(SOLUTION_NAME_TAG), CurrentSettings.DefaultSolutionName));
	        }
	    }

	    protected virtual void HandleOutputPath(XElement root)
	    {
            XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
	        XNamespace defaultNamespace = root.GetDefaultNamespace();
	        var label = new XAttribute("Label", VSPROJECTNORMALIZER_LABEL);
	        RemoveNodesWithAttribute(root, label);
            XAttribute buildTagDefined = Condition.TagDefined(BUILD_DIRECTORY_TAG);
            XAttribute buildTagNotDefined = Condition.TagNotDefined(BUILD_DIRECTORY_TAG);
            string assemblyName = commonPropertyGroup.Elements(defaultNamespace.GetName(ASSEMBLY_NAME_TAG)).Single().Value;
            string outputPath = BuildOutputPath(assemblyName);
	        XElement choose;
	        if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
	        {
                commonPropertyGroup.Add(Elements.Property(BUILD_DIRECTORY_TAG, buildTagNotDefined, CurrentSettings.BuildPath));
	            choose = BuildOutputNodes(Path.Combine(CurrentSettings.BuildPath, outputPath), Path.Combine(CurrentSettings.BuildPath, CurrentSettings.IntermediateOutputPath));
	        }
	        else
	        {
	            choose = Elements.Choose(Elements.When(buildTagDefined, BuildOutputNodes(Path.Combine(CurrentSettings.ExternalBuildPrefix, outputPath), Path.Combine(CurrentSettings.ExternalBuildPrefix, CurrentSettings.IntermediateOutputPath)))
	                                    , Elements.Otherwise(BuildOutputNodes(Path.Combine(CurrentSettings.DefaultBuildPrefix, outputPath), Path.Combine(CurrentSettings.DefaultBuildPrefix, CurrentSettings.IntermediateOutputPath))));
	        }
            choose.Add(label);
            commonPropertyGroup.AddAfterSelf(choose);
        }

	    protected virtual XElement BuildOutputNodes(string outputPath, string intermediatePath)
	    {
	        return Elements.Choose(
	                Elements.When(Condition.And(Condition.TagDefined("Platform"), Condition.TagNotEqual("Platform", "AnyCPU"))
	                    , Elements.PropertyGroup(
	                            Elements.Property(OUTPUT_PATH_TAG, CurrentSettings.WithPlatform(outputPath))
	                            , Elements.Property(INTERMEDIATE_OUTPUT_TAG, CurrentSettings.WithPlatform(intermediatePath))
	                            ))
	                , Elements.Otherwise(
	                    Elements.PropertyGroup(
	                            Elements.Property(OUTPUT_PATH_TAG, outputPath)
	                            , Elements.Property(INTERMEDIATE_OUTPUT_TAG, intermediatePath)
	                            )));
	    }
	}
}
