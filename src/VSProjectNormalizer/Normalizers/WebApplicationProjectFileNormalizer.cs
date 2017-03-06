using System;
using System.Xml.Linq;

namespace VSProjectNormalizer.Normalizers
{
	internal class WebApplicationProjectFileNormalizer : ProjectFileNormalizer
	{
		public WebApplicationProjectFileNormalizer(Settings settings) : base(settings)
		{
		}

		public void Normalize(XElement root)
		{
			XNamespace defaultNamespace = root.GetDefaultNamespace();
			RemoveNodes(root, BUILD_DIRECTORY_TAG, INTERMEDIATE_OUTPUT_TAG);
			XElement commonPropertyGroup = GetFirstCommonPropertyGroup(root);
			var buildTagExists = new XAttribute("Condition", " '$(" + BUILD_DIRECTORY_TAG + ")' != '' ");
			var buildTagNotExists = new XAttribute("Condition", " '$(" + BUILD_DIRECTORY_TAG + ")' == '' ");
			if (!String.IsNullOrEmpty(CurrentSettings.BuildPath))
			{
				commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(BUILD_DIRECTORY_TAG), buildTagExists, CurrentSettings.BuildPath));
				commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG), CurrentSettings.BuildPrefix + CurrentSettings.IntermediateOutputPath));
			}
			else
			{
				commonPropertyGroup.Add(new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG)
					, buildTagExists, CurrentSettings.BuildPrefix + CurrentSettings.IntermediateOutputPath)
					, new XElement(defaultNamespace.GetName(INTERMEDIATE_OUTPUT_TAG)
						, buildTagNotExists, CurrentSettings.DefaultBuildPrefix + CurrentSettings.IntermediateOutputPath));
			}
		}
	}
}