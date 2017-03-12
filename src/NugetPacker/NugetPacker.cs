#region file header
// ////////////////////////////////////////////////////////////////////
// ///
// /// eric brunel 
// /// 11.03.2017
// ///
// ////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CodeTools.Nuget.Helpers;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace CodeTools.VisualStudio.Tools
{
	internal class NugetPacker
	{
		protected Settings Current { get; }

		public NugetPacker(Settings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));
			Current = settings;
		}

		/// <summary>
		/// Generate a package from multiple framework target
		/// update version
		/// update copyright
		/// update description
		/// update author
		/// update project url
		/// update framework assemblies
		/// update package dependencies from {project folder}\packages.config
		/// files should be handled by standard folder structure https://docs.microsoft.com/en-us/nuget/schema/nuspec
		/// </summary>
		/// <param name="packageFile"></param>
		/// <returns></returns>
		public void Pack(FileInfo packageFile)
		{
			if (packageFile == null)
				throw new ArgumentNullException(nameof(packageFile));
			PackageBuilder builder = new PackageBuilder
			{
				Id = Current.Id,
				Version =
					new NuGetVersion(Current.Version.Major,
						Current.Version.Minor,
						Current.Version.Patch,
						Current.Version.ReleaseLabels,
						Current.Version.Metadata),
				Description = Current.Description,
				Title = Current.Title,
				ProjectUrl = Current.ProjectUrl,
				Copyright = Current.Copyright,
				ReleaseNotes = Current.ReleaseNotes
			};
			builder.Authors.AddRange(Current.Authors);
			builder.Owners.AddRange(Current.Owners);
			builder.Tags.AddRange(Current.Tags);
			UpdateDependencies(builder);
			UpdateFrameworkAssemblies(builder);
			UpdateFiles(builder);
			UpdateContent(builder);
			using (var stream = File.OpenWrite(GetFileName(packageFile)))
			{
				builder.Save(stream);
			}
		}

		private static string GetFileName(FileInfo packageFile)
		{
			if (packageFile.Name.EndsWith(".nupkg"))
			{
				return packageFile.FullName;
			}
			else
			{
				return packageFile.FullName + ".nupkg";
			}
		}

		private void UpdateContent(PackageBuilder builder)
		{
			if (Current.Content != null)
			{
				foreach (Settings.ContentFiles content in Current.Content)
				{
					builder.ContentFiles.Add(new ManifestContentFiles()
					{
						BuildAction = content.BuildAction
						,CopyToOutput = content.CopyToOutput.ToString()
						,Exclude = content.Exclude
						,Flatten = content.Flatten.ToString()
						,Include = content.Source
					});
				}
			}
		}

		private void UpdateFiles(PackageBuilder builder)
		{
			if (Current.Libraries != null)
			{
				foreach (Settings.LibraryFiles library in Current.Libraries)
				{
					var framework = NuGetFramework.Parse(library.TargetFramework);
					builder.AddFiles(Current.BasePath, library.Source, Path.Combine(PackagingConstants.Folders.Lib, framework.GetShortFolderName()));
					builder.TargetFrameworks.Add(framework);
				}
			}
			if (Current.Tools != null)
			{
				foreach (Settings.ToolFiles tool in Current.Tools)
				{
					builder.AddFiles(Current.BasePath, tool.Source, PackagingConstants.Folders.Tools);
				}
			}
			if (Current.Build != null)
			{
				foreach (Settings.BuildFiles build in Current.Build)
				{
					builder.AddFiles(Current.BasePath, build.Source, PackagingConstants.Folders.Build);
				}
			}
		}

		private void UpdateFrameworkAssemblies(PackageBuilder builder)
		{
			if (Current.FrameworkAssemblies != null)
			{
				foreach (Settings.FrameworkAssembly assembly in Current.FrameworkAssemblies)
				{
					builder.FrameworkReferences.Add(new FrameworkAssemblyReference(assembly.Name,
						assembly.TargetFrameworks.Select(NuGetFramework.Parse)));
				}
			}
		}

		private void UpdateDependencies(PackageBuilder builder)
		{
			if (Current.Dependencies != null)
			{
				foreach (Settings.DependencyGroup dependencyGroup in Current.Dependencies)
				{
					var packageDependencyGroup = new PackageDependencyGroup(NuGetFramework.Parse(dependencyGroup.TargetFramework),
						dependencyGroup.Select(d => new PackageDependency(d.Id, VersionRange.Parse(d.VersionRange), d.Include, d.Exclude)));
					builder.DependencyGroups.Add(packageDependencyGroup);
				}
			}
		}

		public class Settings
		{
			public string Id { get; set; }

			public string[] Authors { get; set; }

			public NugetVersion Version { get; set; }

			public string Description { get; set; }

			public string Title { get; set; }

			public string[] Owners { get; set; }

			public Uri ProjectUrl { get; set; }

			public string Copyright { get; set; }

			public string ReleaseNotes { get; set; }

			public string[] Tags { get; set; }

			public DependencyGroup[] Dependencies { get; set; }

			public FrameworkAssembly[] FrameworkAssemblies { get; set; }

			public LibraryFiles[] Libraries { get; set; }

			public ToolFiles[] Tools { get; set; }

			public BuildFiles[] Build { get; set; }

			public ContentFiles[] Content { get; set; }

			public string BasePath { get; set; }

			public Settings()
			{
				BasePath = String.Empty;
			}

			public class FrameworkAssembly
			{
				public string Name { get; set; }

				public string[] TargetFrameworks { get; set; }
			}

			public class NugetVersion
			{
				public int Major { get; set; }

				public int Minor { get; set; }

				public int Patch { get; set; }

				public string ReleaseLabels { get; set; }

				public string Metadata { get; set; }

				public NugetVersion(int major, int minor, int patch)
				{
					Major = major;
					Minor = minor;
					Patch = patch;
				}

				public NugetVersion(int major, int minor, int patch, string releaseLabels)
					:this(major,minor,patch)
				{
					ReleaseLabels = releaseLabels;
				}

				public NugetVersion(int major, int minor, int patch, string releaseLabels, string metadata)
					: this(major, minor, patch, releaseLabels)
				{
					Metadata = metadata;
				}
			}

			public class Dependency
			{
				public string Id { get; set; }
				public string VersionRange { get; set; }
				public string[] Include { get; set; }
				public string[] Exclude { get; set; }
			}

			public class DependencyGroup: Collection<Dependency>
			{
				public DependencyGroup(string targetFramework)
				{
					if (targetFramework == null)
						throw new ArgumentNullException(nameof(targetFramework));
					TargetFramework = targetFramework;
				}

				public string TargetFramework { get; set; }
			}

			public class Files
			{
				public string Source { get; set; }

				public string Exclude { get; set; }
			}

			public class LibraryFiles: Files
			{
				public string TargetFramework { get; set; }
			}

			public class ToolFiles: Files
			{
			}

			public class BuildFiles: Files
			{
			}

			public class ContentFiles : Files
			{
				public string BuildAction { get; set; }

				public bool CopyToOutput { get; set; }

				public bool Flatten { get; set; }
			}
		}
	}
}