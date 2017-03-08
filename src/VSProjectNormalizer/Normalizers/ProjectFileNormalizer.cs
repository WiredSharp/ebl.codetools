using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace VSProjectNormalizer.Normalizers
{
	/// <summary>
	/// visual studio project file normalizer base type
	/// </summary>
	internal abstract class ProjectFileNormalizer
	{
		protected const string BUILD_DIRECTORY_TAG = "BuildDir";
		protected const string INTERMEDIATE_OUTPUT_TAG = "IntermediateOutputPath";

		protected ProjectFileNormalizer(Settings settings)
		{
			if (settings == null) throw new ArgumentNullException(nameof(settings));
			CurrentSettings = settings;
		}

		protected Settings CurrentSettings { get; set; }

		protected string BuildOutputPath(string assemblyName)
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
			if (nodeNames == null) throw new ArgumentNullException(nameof(nodeNames));
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

	    protected XElement[] FindNodeByName(XContainer root, string nodeName)
	    {
	        return root.DescendantNodes().OfType<XElement>().Where(node => node.Name.LocalName == nodeName).ToArray();
	    }

		protected XElement GetFirstCommonPropertyGroup(XContainer root)
		{
			return
				root.Elements().
					FirstOrDefault(node => !node.HasAttributes && node.Name.ToString().EndsWith("PropertyGroup"));
		}

		protected static XAttribute ExistCondition(string tag)
		{
			return new XAttribute("Condition", " '$(" + tag + ")' != '' ");
		}

		protected static XAttribute NotExistCondition(string tag)
		{
			return new XAttribute("Condition", " '$(" + tag + ")' == '' ");
		}

	    protected XAttribute EqualCondition(string lhs, string rhs)
	    {
            return new XAttribute("Condition", $"'{lhs}'=='{rhs}'");
        }
	}
}