using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VSProjectNormalizer.MsBuild;

namespace VSProjectNormalizer.Normalizers
{
	internal class WebApplicationProjectFileNormalizer : CsharpProjectFileNormalizer
	{
		public WebApplicationProjectFileNormalizer(Settings settings) : base(settings)
		{
		}

		public override void Normalize(XElement root)
		{
            RemoveNodes(root, INTERMEDIATE_OUTPUT_TAG);
            if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
            {
                RemoveNodes(root, BUILD_DIRECTORY_TAG);
            }
            UpdateSolutionProperties(root);
            HandleOutputPath(root);
        }

	    protected override void HandleOutputPath(XElement root)
	    {
            XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
            var label = new XAttribute("Label", VSPROJECTNORMALIZER_LABEL);
            RemoveNodesWithAttribute(root, label);
            XAttribute buildTagDefined = Condition.TagDefined(BUILD_DIRECTORY_TAG);
            XAttribute buildTagNotDefined = Condition.TagNotDefined(BUILD_DIRECTORY_TAG);
            XElement choose;
            if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
            {
                commonPropertyGroup.Add(Elements.Property(BUILD_DIRECTORY_TAG, buildTagNotDefined, CurrentSettings.BuildPath));
                choose = BuildIntermediateOutputNodes(Path.Combine(CurrentSettings.BuildPath, CurrentSettings.IntermediateOutputPath));
            }
            else
            {
                choose = Elements.Choose(Elements.When(buildTagDefined, BuildIntermediateOutputNodes(Path.Combine(CurrentSettings.BuildPrefix, CurrentSettings.IntermediateOutputPath)))
                                        , Elements.Otherwise(BuildIntermediateOutputNodes(Path.Combine(CurrentSettings.DefaultBuildPrefix, CurrentSettings.IntermediateOutputPath))));
            }
            choose.Add(label);
            commonPropertyGroup.AddAfterSelf(choose);
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