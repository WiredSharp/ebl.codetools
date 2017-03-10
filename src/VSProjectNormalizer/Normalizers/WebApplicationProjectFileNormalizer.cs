using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.MSBuild.Helpers.VisualStudio;

namespace VSProjectNormalizer.Normalizers
{
	internal class WebApplicationProjectFileNormalizer : CsharpProjectFileNormalizer
	{
		public WebApplicationProjectFileNormalizer(Settings settings) : base(settings)
		{
		}

		internal override void Normalize(ProjectFile projectFile)
		{
            projectFile.RemoveNodes(INTERMEDIATE_OUTPUT_TAG);
            if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
            {
                projectFile.RemoveNodes(BUILD_DIRECTORY_TAG);
            }
            UpdateSolutionProperties(projectFile);
            HandleOutputPath(projectFile);
        }

	    protected override void HandleOutputPath(ProjectFile projectFile)
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
                choose = BuildIntermediateOutputNodes(Path.Combine(CurrentSettings.BuildPath, CurrentSettings.IntermediateOutputPath));
            }
            else
            {
                choose = Elements.Choose(Elements.When(buildTagDefined, BuildIntermediateOutputNodes(Path.Combine(CurrentSettings.ExternalBuildPrefix, CurrentSettings.IntermediateOutputPath)))
                                        , Elements.Otherwise(BuildIntermediateOutputNodes(Path.Combine(CurrentSettings.DefaultBuildPrefix, CurrentSettings.IntermediateOutputPath))));
            }
            commonPropertyGroup.AddAfterSelf(choose);
            XNode generatedRegionMark = BuildGeneratedRegionMark();
            choose.AddBeforeSelf(generatedRegionMark);
            choose.AddAfterSelf(generatedRegionMark);
        }

        private XElement BuildIntermediateOutputNodes(string intermediatePath)
	    {
            return Elements.Choose(
                    Elements.When(Condition.And(Condition.TagDefined("Platform"), Condition.TagNotEqual("Platform", "AnyCPU"))
                        , Elements.PropertyGroup(
                                Elements.Property(INTERMEDIATE_OUTPUT_TAG, CurrentSettings.WithPlatform(intermediatePath))
                                ))
                    , Elements.Otherwise(
                        Elements.PropertyGroup(
                                Elements.Property(INTERMEDIATE_OUTPUT_TAG, intermediatePath)
                                )));
        }
	}
}