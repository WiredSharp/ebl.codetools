using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeAnalyzer
{
	public class Project
    {
        public PathLink Path { get; private set; }

        public Guid ProjectGuid { get; private set; }

        public string TargetFrameworkVersion { get; private set; }

        public Reference[] References { get; private set; }

        public ProjectReference[] LocalReferences { get; private set; }

        private static readonly Reference[] EMPTY_REFERENCES = new Reference[0];
        private static readonly ProjectReference[] EMPTY_LOCAL_REFERENCES = new ProjectReference[0];
        public Project(string path)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentException("path");
            Path = new PathLink(path);
            LocalReferences = EMPTY_LOCAL_REFERENCES;
            References = EMPTY_REFERENCES;
        }

        private void Parse()
        {
            XElement root = XElement.Parse(File.ReadAllText(Path));
            var references = new List<Reference>();
            const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";
            string folder = System.IO.Path.GetDirectoryName(Path);
            TargetFrameworkVersion = root.Descendants(XName.Get("TargetFrameworkVersion", ns)).First().Value;
            ProjectGuid = Guid.Parse(root.Descendants(XName.Get("ProjectGuid", ns)).First().Value);
            foreach (XElement xref in root.Descendants(XName.Get("Reference", ns)))
            {
                var refPath = xref.Element(XName.Get("HintPath", ns));
                if (refPath != null)
                {
                    var fqdnItems = xref.Attribute("Include").Value.Split(',');
                    references.Add(new Reference(fqdnItems.First(), PathHelpers.Combine(folder,refPath.Value)));
                }
            }
            References = references.ToArray();
            var localReferences = new List<ProjectReference>();
            foreach (XElement xref in root.Descendants(XName.Get("ProjectReference", ns)))
            {
                var refName = xref.Element(XName.Get("Name", ns));
                Guid refProjectId = Guid.Parse(xref.Element(XName.Get("Project", ns)).Value);
                localReferences.Add(new ProjectReference(refName.Value, PathHelpers.Combine(folder, xref.Attribute("Include").Value), refProjectId));
            }
            LocalReferences = localReferences.ToArray();
        }

        public static Project Parse(string projectPath)
        {
            var project = new Project(projectPath);
            project.Parse();
            return project;
        }
    }
}