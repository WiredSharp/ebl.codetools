﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.MSBuild.Helpers.VisualStudio;

namespace VSProjectNormalizer.Normalizers
{
	internal class CsharpProjectFileNormalizer : ProjectFileNormalizer
	{
	    protected const string SOLUTION_DIR_TAG = "SolutionDir";
        protected const string SOLUTION_NAME_TAG = "SolutionName";
        protected const string OUTPUT_PATH_TAG = "OutputPath";
        protected const string ASSEMBLY_NAME_TAG = "AssemblyName";
	    protected const string GENERATED_REGION_TAG = "GENERATED BY VSPROJECTNORMALIZER: DO NOT REMOVE COMMENT";


        public CsharpProjectFileNormalizer(Settings settings) 
            : base(settings)
		{
		}

	    public void Normalize(FileInfo projectFile)
	    {
	        if (projectFile == null) throw new ArgumentNullException(nameof(projectFile));
	        File.WriteAllText(projectFile.FullName, Normalize(File.ReadAllText(projectFile.FullName)));
	    }

	    public string Normalize(string fileContent)
	    {
	        ProjectFile projectFile = ProjectFile.Parse(fileContent);
	        Normalize(projectFile);
	        return projectFile.ToXml();
	    }

	    internal virtual void Normalize(ProjectFile projectFile)
	    {
	        projectFile.RemoveNodes(OUTPUT_PATH_TAG, INTERMEDIATE_OUTPUT_TAG);
		    if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
		    {
                projectFile.RemoveNodes(BUILD_DIRECTORY_TAG);
		    }
		    UpdateSolutionProperties(projectFile);
		    HandleOutputPath(projectFile);
		}

	    protected virtual void UpdateSolutionProperties(ProjectFile projectFile)
	    {
            XElement commonPropertyGroup = projectFile.PropertyGroups().First();
            if (!projectFile.GetProperties(SOLUTION_DIR_TAG).Any())
	        {
	            commonPropertyGroup.Add(projectFile.NewProperty(SOLUTION_DIR_TAG, CurrentSettings.DefaultSolutionDir, true));
	        }
            if (!projectFile.GetProperties(SOLUTION_NAME_TAG).Any())
            {
                commonPropertyGroup.Add(projectFile.NewProperty(SOLUTION_NAME_TAG, CurrentSettings.DefaultSolutionDir, true));
            }
	    }

	    protected virtual void HandleOutputPath(ProjectFile projectFile)
	    {
            XElement commonPropertyGroup = projectFile.PropertyGroups().First();
            RemoveGeneratedNodes(projectFile);
	        XAttribute buildTagDefined = Condition.TagDefined(BUILD_DIRECTORY_TAG);
            XAttribute buildTagNotDefined = Condition.TagNotDefined(BUILD_DIRECTORY_TAG);
            string assemblyName = projectFile.AssemblyName().First().Value;
            string outputPath = BuildOutputPath(assemblyName);
	        XElement choose;
	        if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
	        {
                commonPropertyGroup.Add(Elements.Property(BUILD_DIRECTORY_TAG, CurrentSettings.BuildPath, buildTagNotDefined));
	            choose = BuildOutputNodes(Path.Combine(CurrentSettings.BuildPath, outputPath), Path.Combine(CurrentSettings.BuildPath, CurrentSettings.IntermediateOutputPath));
	        }
	        else
	        {
	            choose = Elements.Choose(Elements.When(buildTagDefined, BuildOutputNodes(Path.Combine(CurrentSettings.ExternalBuildPrefix, outputPath), Path.Combine(CurrentSettings.ExternalBuildPrefix, CurrentSettings.IntermediateOutputPath)))
	                                    , Elements.Otherwise(BuildOutputNodes(Path.Combine(CurrentSettings.DefaultBuildPrefix, outputPath), Path.Combine(CurrentSettings.DefaultBuildPrefix, CurrentSettings.IntermediateOutputPath))));
	        }
            commonPropertyGroup.AddAfterSelf(choose);
	        var generatedRegionMark = BuildGeneratedRegionMark();
            choose.AddBeforeSelf(generatedRegionMark);
            choose.AddAfterSelf(generatedRegionMark);
        }

	    protected void RemoveGeneratedNodes(ProjectFile projectFile)
	    {
	        XComment generatedRegionMark = BuildGeneratedRegionMark();
	        foreach (XNode node in projectFile.GetRegion(generatedRegionMark))
	        {
	            node.Remove();
	        }
	    }

	    protected XComment BuildGeneratedRegionMark()
	    {
	        return new XComment(GENERATED_REGION_TAG);
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
