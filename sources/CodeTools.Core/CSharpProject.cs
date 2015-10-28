using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeTools.Core
{
	public class Project
	{
		protected const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		public PathLink Path { get; protected set; }
		public Guid ProjectGuid { get; protected set; }

		protected Project(string path)
		{
			if (String.IsNullOrEmpty(path)) throw new ArgumentException("path");
			Path = new PathLink(path);
		}

		protected static IEnumerable<XElement> FindInProject(XContainer root, string localName, string xns = ns)
		{
			return root.Descendants(XName.Get(localName, xns));
		}

		protected string FindValueInProject(XContainer root, string localName)
		{
			XElement element = FindInProject(root, localName).FirstOrDefault();
			if (element != null)
			{
				return element.Value;
			}
			else
			{
				Trace.TraceWarning(localName + " not found in " + Path);
				return null;
			}
		}
	}

	public class CSharpProject : Project
	{
		public string TargetFrameworkVersion { get; private set; }

		public Reference[] References { get; private set; }

		public ProjectReference[] LocalReferences { get; private set; }

		private static readonly Reference[] EMPTY_REFERENCES = new Reference[0];
		private static readonly ProjectReference[] EMPTY_LOCAL_REFERENCES = new ProjectReference[0];
		public CSharpProject(string path)
			:base(path)
		{
			LocalReferences = EMPTY_LOCAL_REFERENCES;
			References = EMPTY_REFERENCES;
		}

		protected void Parse()
		{
			XElement root = XElement.Parse(File.ReadAllText(Path));
			var references = new List<Reference>();
			string folder = System.IO.Path.GetDirectoryName(Path);
			FindValueInProject(root, "TargetFrameworkVersion");
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

		public static CSharpProject Parse(string projectPath)
		{
			var project = new CSharpProject(projectPath);
			project.Parse();
			return project;
		}
	}
}