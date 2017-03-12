using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using CodeTools.MSBuild.Helpers.VisualStudio;
using CodeTools.VisualStudio.Tools;
using VSProjectNormalizer.Normalizers;

namespace VSProjectNormalizer
{
	public class VSProjectNormalizer
	{
		protected Settings CurrentSettings { get; set; }

		public VSProjectNormalizer(Settings settings)
		{
			if (settings == null) throw new ArgumentNullException(nameof(settings));
			CurrentSettings = settings;
		}

		public string Normalize(FileInfo projectFile)
		{
			return Normalize(ProjectFile.Load(projectFile));
		}

		public string Normalize(string projectFileContent)
		{
			return Normalize(ProjectFile.Parse(projectFileContent));
		}

		private string Normalize(ProjectFile projectFile)
	    {
	        if (projectFile.IsCSharpProject() || projectFile.IsWpfProject())
	        {
	            var normalizer = new CsharpProjectFileNormalizer(projectFile, CurrentSettings);
	            normalizer.Normalize();
	        }
	        else if (projectFile.IsWebProject())
	        {
	            var normalizer = new WebApplicationProjectFileNormalizer(projectFile, CurrentSettings);
	            normalizer.Normalize();
	        }
	        return projectFile.ToXml();
	    }
	}
}