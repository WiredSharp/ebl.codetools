using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using CodeTools.VisualStudio.Tools;

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

		protected void RemoveNodes(XContainer root, params string[] localNames)
		{
			if (localNames == null) throw new ArgumentNullException(nameof(localNames));
			if (localNames.Length == 0) return;
			var toRemove = new List<XElement>();
			foreach (
				XElement node in
					root.Descendants().Where(
						node => localNames.Contains(node.Name.LocalName)))
			{
				Debug.WriteLine($"{node.Name}='{node.Value}'");
				toRemove.Add(node);
			}
			foreach (XElement node in toRemove)
			{
				node.Remove();
			}
		}

	    protected XElement[] FindNodeByName(XContainer root, string localName)
	    {
	        return root.Descendants().Where(node => node.Name.LocalName == localName).ToArray();
	    }

		protected XElement GetFirstCommonPropertyGroup(XContainer root)
		{
			return
				root.Elements().
					FirstOrDefault(node => !node.HasAttributes && node.Name.LocalName.ToString().EndsWith("PropertyGroup"));
		}

	    protected static void RemoveNodesWithAttribute(XElement root, XAttribute label)
	    {
	        var toRemove = root.Descendants()
	                           .Where(n => n.Attributes().Any(a => a.Name == label.Name && a.Value == label.Value)).ToArray();
	        foreach (XElement element in toRemove)
	        {
	            element.Remove();
	        }
	    }

        protected string BuildOutputPath(string assemblyName)
        {
            string outputPath;
            if (assemblyName.EndsWith("Acceptance.Test", StringComparison.CurrentCultureIgnoreCase)
                || assemblyName.EndsWith("Acceptance.Tests", StringComparison.CurrentCultureIgnoreCase)
                || assemblyName.EndsWith("Acceptance", StringComparison.CurrentCultureIgnoreCase))
            {
                outputPath = "Acceptance";
            }
            else if (assemblyName.EndsWith(".Test", StringComparison.CurrentCultureIgnoreCase)
                     || assemblyName.EndsWith(".Tests", StringComparison.CurrentCultureIgnoreCase))
            {
                outputPath = "Test";
            }
            else
            {
                outputPath = "Bin";
            }
            return outputPath;
        }
    }
}