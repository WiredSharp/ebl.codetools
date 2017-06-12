#region file header
// ////////////////////////////////////////////////////////////////////
// ///
// /// eric brunel 
// /// 11.03.2017
// ///
// ////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using NuGet.Packaging;
using NuGet.Versioning;

namespace CodeTools.VisualStudio.Tools
{
    /// <summary>
    /// wrapper for <see cref="NuGet.Packaging.PackageBuilder"/>
    /// </summary>
    internal class NugetPacker
	{
		protected Specification Current { get; }

		public NugetPacker(Specification specification)
		{
			if (specification == null)
				throw new ArgumentNullException(nameof(specification));
			Current = specification;
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
			if (Current.ContentFiles != null)
			{
				foreach (ManifestContentFiles content in Current.ContentFiles)
				{
					builder.ContentFiles.Add(content);
				}
			}
		}

		private void UpdateFiles(PackageBuilder builder)
		{
			if (Current.Files != null)
			{
				foreach (var file in Current.Files)
				{
					builder.AddFiles(Current.BasePath, file.Source, file.Target);
				}
			}
		}

		private void UpdateFrameworkAssemblies(PackageBuilder builder)
		{
			if (Current.FrameworkAssemblies != null)
			{
				foreach (FrameworkAssemblyReference assembly in Current.FrameworkAssemblies)
				{
					builder.FrameworkReferences.Add(assembly);
				}
			}
		}

		private void UpdateDependencies(PackageBuilder builder)
		{
			if (Current.Dependencies != null)
			{
				foreach (PackageDependencyGroup dependencyGroup in Current.Dependencies)
				{
					builder.DependencyGroups.Add(dependencyGroup);
				}
			}
		}
	}
}