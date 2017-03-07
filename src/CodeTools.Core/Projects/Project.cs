using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeTools.Core.Projects
{
	public class Project
	{
		protected const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		public PathLink Path { get; protected set; }
		public Guid ProjectGuid { get; protected set; }

		protected Project(FileInfo path)
		{
			if (path == null) throw new ArgumentNullException("path");
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
}