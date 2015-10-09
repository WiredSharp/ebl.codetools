using System;

namespace CodeAnalyzer
{
	public class Reference
    {
        public string Name { get; private set; }

        public PathLink Path { get; private set; }

        public string Version { get; private set; }

        public Reference(string name, string path)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("name");
            if (String.IsNullOrEmpty(path)) throw new ArgumentException("path");
            Name = name;
            Path = new PathLink(path);
        }

        public Reference(string name, string path, string version)
            :this(name, path)
        {
            Version = version;
        }

        public override string ToString()
        {
            return "-> " + Name;
        }
    }

	public class ProjectReference : Reference
    {
        public Guid ProjectId { get; private set; }
        public ProjectReference(string name, string path, Guid projectId)
            :base(name, path)
        {
            ProjectId = projectId;
        }        
    }
}