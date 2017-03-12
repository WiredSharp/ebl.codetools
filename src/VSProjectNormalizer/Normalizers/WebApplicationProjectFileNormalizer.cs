using System.Xml.Linq;
using CodeTools.MSBuild.Helpers.VisualStudio;
using CodeTools.VisualStudio.Tools;

namespace VSProjectNormalizer.Normalizers
{
	internal class WebApplicationProjectFileNormalizer : CsharpProjectFileNormalizer
	{
		public WebApplicationProjectFileNormalizer(ProjectFile projectFile, Settings settings) 
            : base(projectFile, settings)
		{
		}

	    protected override void RemoveUnwantedNodes()
	    {
            _projectFile.RemoveNodes(INTERMEDIATE_OUTPUT_TAG);
        }

	    protected override XNode HandleOutputPath(XNode current)
	    {
            XElement propertyGroup = _projectFile.NewPropertyGroup(Condition.TagNotDefined(SOLUTION_DIR_TAG)
                , _projectFile.NewProperty(INTERMEDIATE_OUTPUT_TAG, CurrentSettings.ProjectIntermediateFolder, !ADD_IS_DEFINED));
            current.AddAfterSelf(propertyGroup);
            propertyGroup = _projectFile.NewPropertyGroup(Condition.TagDefined(SOLUTION_DIR_TAG)
                , _projectFile.NewProperty(INTERMEDIATE_OUTPUT_TAG, CurrentSettings.SolutionIntermediateFolder, !ADD_IS_DEFINED));
            current.AddAfterSelf(propertyGroup);
            return propertyGroup;
        }
	}
}