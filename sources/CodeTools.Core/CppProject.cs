using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeTools.Core
{
	public class CppProject : Project
	{
		public string Version { get; private set; }

		public string Name { get; private set; }

		public static CppProject Parse(string projectPath)
		{
			var project = new CppProject(projectPath);
			project.Parse();
			return project;
		}

		protected void Parse()
		{
			if (System.IO.Path.GetExtension(Path) == ".vcxproj")
			{
				XElement root = XElement.Parse(File.ReadAllText(Path));
				Name = FindValueInProject(root, "ProjectName");
				ProjectGuid = Guid.Parse(FindInProject(root, "ProjectGuid").First().Value);
			}
			else
			{
				XElement root = XElement.Parse(File.ReadAllText(Path));
				//VisualStudioProject
				Name = GetProjectAttribute(root, "Name");
				Version = GetProjectAttribute(root, "Version");
				string attribute = GetProjectAttribute(root, "ProjectGUID");
				if (attribute != null)
				{
					ProjectGuid = Guid.Parse(attribute);
				}
			}
		}

		private string GetProjectAttribute(XElement projectRoot, string attributeName)
		{
			XAttribute attribute = projectRoot.Attributes(attributeName).FirstOrDefault();
			return attribute != null ? attribute.Value : null;
		}

		public CppProject(string projectPath)
			: base(projectPath)
		{
		}

		public override string ToString()
		{
			return Name;
		}
	}
}