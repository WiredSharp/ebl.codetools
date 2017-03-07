using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeTools.Core.Projects
{
	public class CSharpProject : Project
	{
		public string TargetFrameworkVersion { get; private set; }

		public Reference[] References { get; private set; }

		public ProjectReference[] LocalReferences { get; private set; }

		private static readonly Reference[] EMPTY_REFERENCES = new Reference[0];
		private static readonly ProjectReference[] EMPTY_LOCAL_REFERENCES = new ProjectReference[0];
		private IEnumerable<PathLink> _items;

		public CSharpProject(FileInfo path)
			:base(path)
		{
			LocalReferences = EMPTY_LOCAL_REFERENCES;
			References = EMPTY_REFERENCES;
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
			XElement root = XElement.Parse(File.ReadAllText(Path));
			string projectPath = System.IO.Path.GetDirectoryName(Path);
			var items = new List<PathLink>();
			foreach (XElement item in FindInProject(root, "Compile"))
			{
				var path = item.Attribute("Include");
				if (!String.IsNullOrEmpty(path?.Value))
				{
					items.Add(new PathLink(PathHelpers.Combine(projectPath, path.Value)));
				}
				else
				{
					Trace.TraceWarning("missing include path in project " + Path + " (" + item + ")");
				}
			}
			return items;
		}

		protected void Parse()
		{
			XElement root = XElement.Parse(File.ReadAllText(Path));
			var references = new List<Reference>();
			string folder = System.IO.Path.GetDirectoryName(Path);
			TargetFrameworkVersion = FindValueInProject(root, "TargetFrameworkVersion");
			ProjectGuid = Guid.Parse(FindInProject(root, "ProjectGuid").First().Value);
			foreach (XElement xref in FindInProject(root, "Reference"))
			{
				var refPath = xref.Element(XName.Get("HintPath", ns));
				if (refPath != null)
				{
					var fqdnItems = xref.Attribute("Include").Value.Split(',');
					references.Add(new Reference(fqdnItems.First(), PathHelpers.Combine(folder, refPath.Value)));
				}
			}
			References = references.ToArray();
			var localReferences = new List<ProjectReference>();
			foreach (XElement xref in FindInProject(root, "ProjectReference"))
			{
				var refName = xref.Element(XName.Get("Name", ns));
				Guid refProjectId = Guid.Parse(xref.Element(XName.Get("Project", ns)).Value);
				localReferences.Add(new ProjectReference(refName.Value, PathHelpers.Combine(folder, xref.Attribute("Include").Value), refProjectId));
			}
			LocalReferences = localReferences.ToArray();
		}

		public static CSharpProject Parse(FileInfo projectPath)
		{
			var project = new CSharpProject(projectPath);
			project.Parse();
			return project;
		}
	}
}