using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeTools.Core.Projects
{
	public class CppProject : Project
	{
		private IEnumerable<PathLink> _items;
		public string Version { get; private set; }

		public string Name { get; private set; }

		public static CppProject Parse(string projectPath)
		{
			var project = new CppProject(projectPath);
			project.Parse();
			return project;
		}

		public IEnumerable<PathLink> Items
		{
			get
			{
				if (_items == null)
				{
					_items = ParseIncludes();
				}
				return _items;
			}
		}

		private IEnumerable<PathLink> ParseIncludes()
		{
			if (System.IO.Path.GetExtension(Path) == ".vcxproj")
			{
				string projectPath = System.IO.Path.GetDirectoryName(Path);
				XElement root = XElement.Parse(File.ReadAllText(Path));
				IEnumerable<XElement> includedItems = FindInProject(root, "ClInclude")
					.Union(FindInProject(root, "ClCompile"))
					.Union(FindInProject(root, "ResourceCompile")).ToArray();
				var items = new List<PathLink>();
				foreach (XElement item in includedItems)
				{
					var path = item.Attribute("Include");
					if (path != null && ! String.IsNullOrEmpty(path.Value))
					{
						items.Add(new PathLink(PathHelpers.Combine(projectPath, path.Value)));
					}
					else
					{
						Trace.TraceWarning("missing include path in project " + Name + " (" + Path + ") (" + item + ")");
					}
				}
				return items;
			}
			else
			{
				Trace.TraceWarning(Path + ": not supported");
				return new PathLink[0];
			}
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