using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Versioning;

namespace CodeTools.VisualStudio.Tools
{
    internal class Specification
    {
        public string Id { get; set; }

        public IList<string> Authors { get; set; }

        public NuGetVersion Version { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public IList<string> Owners { get; set; }

        public Uri ProjectUrl { get; set; }

        public Uri LicenseUrl { get; set; }

        public Uri IconUrl { get; set; }

        public string Copyright { get; set; }

        public string ReleaseNotes { get; set; }

        public IList<string> Tags { get; set; }

        internal FrameworkAssemblyCollection FrameworkAssemblies { get; set; }

        public PackageDependencyGroupCollection Dependencies { get; set; }

        public IList<ManifestFile> Files { get; set; }

        public IList<ManifestContentFiles> ContentFiles { get; set; }

        public string BasePath { get; set; }

        public Specification()
        {
            BasePath = String.Empty;
            FrameworkAssemblies = new FrameworkAssemblyCollection();
            Dependencies = new PackageDependencyGroupCollection();
            Tags = new List<string>();
            Authors = new List<string>();
            Owners = new List<string>();
            Files = new List<ManifestFile>();
            ContentFiles = new List<ManifestContentFiles>();
        }

        public class PackageDependencyGroupCollection : KeyedCollection<NuGetFramework, PackageDependencyGroup>
        {
            protected override NuGetFramework GetKeyForItem(PackageDependencyGroup item)
            {
                return item.TargetFramework;
            }

            public void AddRange(IEnumerable<PackageDependencyGroup> dependencyGroups)
            {
                if (dependencyGroups == null) throw new ArgumentNullException(nameof(dependencyGroups));
                foreach (PackageDependencyGroup dependencyGroup in dependencyGroups)
                {
                    Add(dependencyGroup);
                }
            }
        }

        public class FrameworkAssemblyCollection : KeyedCollection<string, FrameworkAssemblyReference>
        {
            protected override string GetKeyForItem(FrameworkAssemblyReference item)
            {
                return item.AssemblyName;
            }

            public void Add(string assemblyName, NuGetFramework targetFramework)
            {
                if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));
                if (targetFramework == null) throw new ArgumentNullException(nameof(targetFramework));
                FrameworkAssemblyReference fmkRef;
                if (Dictionary != null && Dictionary.TryGetValue(assemblyName, out fmkRef))
                {
                    var targetFmks = new HashSet<NuGetFramework> { targetFramework };
                    foreach (NuGetFramework framework in fmkRef.SupportedFrameworks)
                    {
                        targetFmks.Add(framework);
                    }
                    SetItem(IndexOf(fmkRef), new FrameworkAssemblyReference(assemblyName, targetFmks));
                }
                else
                {
                    Add(new FrameworkAssemblyReference(assemblyName, new[] { targetFramework }));
                }
            }
        }

    }
}