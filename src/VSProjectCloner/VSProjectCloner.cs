#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-08 (18:05)
// ///
// ///
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.MSBuild.Helpers.VisualStudio;

namespace CodeTools.VisualStudio.Tools
{
	/// <summary>
	/// Synchronize C# Project
	/// </summary>
	internal class VSProjectCloner
	{
		private readonly FileInfo _projectFileInfo;

		public VSProjectCloner(FileInfo projectFile)
		{
			if (projectFile == null) throw new ArgumentNullException(nameof(projectFile));
			if (!projectFile.Exists) throw new ArgumentException($"project file {projectFile} does not exist", nameof(projectFile));
			if (projectFile.DirectoryName == null)
			{
				throw new ArgumentException("project file must have its parent directory defined");
			}
			_projectFileInfo = projectFile;
		}

		protected ProjectFile Clone(string targetFrameworkVersion)
		{
			var project = ProjectFile.Load(_projectFileInfo);
			// generate a new Guid
			foreach (XElement projectGuid in project.ProjectGuid())
			{
				projectGuid.Value = Guid.NewGuid().ToString("B");
			}
			// update target framework
			foreach (XElement fmk in project.TargetFrameworkVersion())
			{
				fmk.Value = targetFrameworkVersion;
			}
			// TODO: replace local file reference with link
			foreach (XElement item in project.GetItems(p => p.Name.LocalName == "Compile"))
			{
				ConvertToLinkedCompileItem(project, item);
			}
			return project;
		}

		private void ConvertToLinkedCompileItem(ProjectFile project, XElement item)
		{
			XAttribute file = item.Attribute("Include");
			if (null == file)
			{
				Trace.TraceWarning($"compile item {item} does not have include attribute");
				return;
			}
			string link = item.Element("Link")?.Value;
			if (null == link) // compile item is not a link
			{
				file.Value = Path.Combine("..", GetFolder(_projectFileInfo.DirectoryName), file.Value);
				item.Add(project.NewLink(Path.GetFileName(file.Value)));
			}
		}

		private string GetFolder(string directoryName)
		{
			return directoryName.Split(Path.DirectorySeparatorChar).Last();
		}


		public FileInfo CloneTo(NetFramework targetFrameworkVersion)
		{
			ProjectFile cloned = Clone(targetFrameworkVersion.MsbuildTag);
			string projectName = Path.GetFileNameWithoutExtension(_projectFileInfo.Name);
			string targetFolderName = projectName + "." + targetFrameworkVersion.NugetFolder;
			string targetFolder = Path.Combine(_projectFileInfo.DirectoryName, "..", targetFolderName);
			if (!Directory.Exists(targetFolder))
			{
				Directory.CreateDirectory(targetFolder);
			}
			string clonedProjectFile = Path.Combine(targetFolder, projectName + Path.GetExtension(_projectFileInfo.Name));
			File.WriteAllText(clonedProjectFile, cloned.ToString());
			Trace.TraceInformation($"project file generated: {clonedProjectFile}");
			return new FileInfo(clonedProjectFile);
		}
	}
}