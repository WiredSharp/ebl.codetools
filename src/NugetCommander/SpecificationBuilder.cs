#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-13 (09:47)
// ///
// ///
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Xml.Linq;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace CodeTools.NugetCommander
{
    public class SpecificationBuilder
    {
        private const string DEFAULT_ICON_URL = "http://ICON_URL_HERE_OR_DELETE_THIS_LINE";
        private const string DEFAULT_LICENCE_URL = "http://LICENCE_URL_HERE_OR_DELETE_THIS_LINE";
        private const string DEFAULT_PROJECT_URL = "http://PROJECT_URL_HERE_OR_DELETE_THIS_LINE";
        private const string DEFAULT_TAGS = "Tag1 Tag2";
        private const string DEFAULT_RELEASE_NOTES = "Summary of changes made in this release of the package.";

        internal Specification Specification { get; private set; }

        public SpecificationBuilder()
        {
            Specification = new Specification();
        }

        public SpecificationBuilder WithNuSpec(Stream nuspecContent)
        {
            var reader = new NuspecReader(nuspecContent);
            ProcessNuspecContent(reader);
            return this;
        }

        public SpecificationBuilder WithNuSpec(FileInfo nuspecFile)
        {
            if (nuspecFile == null) throw new ArgumentNullException(nameof(nuspecFile));
            var reader = new NuspecReader(nuspecFile.FullName);
            ProcessNuspecContent(reader);
            return this;
        }

        public SpecificationBuilder WithPackages(FileInfo packageConfig)
        {
            using (var reader = File.OpenRead(packageConfig.FullName))
            {
                ProcessPackagesContent(reader);
            }
            return this;
        }

        public SpecificationBuilder WithPackages(Stream packageConfig)
        {
            ProcessPackagesContent(packageConfig);
            return this;
        }

        public SpecificationBuilder WithAssembly(FileInfo assemblyFile)
        {
            if (assemblyFile == null) throw new ArgumentNullException(nameof(assemblyFile));
            Assembly assembly = LoadFromFile(assemblyFile);
            NuGetFramework targetFramework = ProcessAssembly(assembly);
            Specification.Files.Add(new LibraryFiles(targetFramework) { Source = assemblyFile.DirectoryName });
            return this;
        }

        public SpecificationBuilder WithFiles(ManifestFile files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            Specification.Files.Add(files);
            return this;
        }

        public SpecificationBuilder WithContentFiles(ManifestContentFiles files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            Specification.ContentFiles.Add(files);
            return this;
        }

        private static Assembly LoadFromFile(FileInfo assemblyFile)
        {
            Assembly assembly;
            using (var mem = new MemoryStream())
            {
                using (FileStream stream = File.OpenRead(assemblyFile.FullName))
                {
                    stream.CopyTo(mem);
                }
                assembly = Assembly.Load(mem.GetBuffer());
            }
            return assembly;
        }

        private void ProcessPackagesContent(Stream packagesFileContent)
        {
            var parser = new PackagesConfigReader(XDocument.Load(packagesFileContent));
            AddPackageReferences(parser.GetPackages());
        }

        private NuGetFramework ProcessAssembly(Assembly assembly)
        {
            IList<CustomAttributeData> attributesData = assembly.GetCustomAttributesData();
            Specification.Id = assembly.FullName;
            NuGetFramework targetFramework = null;
            foreach (CustomAttributeData attributeData in attributesData)
            {
                if (attributeData.Constructor.DeclaringType == typeof(AssemblyCopyrightAttribute))
                {
                    Specification.Copyright = attributeData.ConstructorArguments.First().ToString().Trim(new[] { '"' });
                }
                else if (attributeData.Constructor.DeclaringType == typeof(AssemblyInformationalVersionAttribute))
                {
                    string[] version = attributeData.ConstructorArguments.First().ToString().Trim(new []{'"'}).Split(new []{'-'}, StringSplitOptions.RemoveEmptyEntries);
                    if (version.Length == 1)
                    {
                        Specification.Version = NuGetVersion.Parse(version[0]);
                    }
                    else
                    {
                        Specification.Version = new NuGetVersion(System.Version.Parse(version[0]), version[1]);
                    }
                }
                else if (attributeData.Constructor.DeclaringType ==
                         typeof(TargetFrameworkAttribute))
                {
                    targetFramework = NuGetFramework.ParseFrameworkName(
                                                                         attributeData.NamedArguments.First().TypedValue.Value.ToString().Trim(new[] { '"' }),
                                                                         DefaultFrameworkNameProvider.Instance);
                }
            }
            if (targetFramework == null)
            {
                throw new ArgumentException("no target framework identified in assembly " + assembly);
            }
            foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
            {
                Specification.FrameworkAssemblies.Add(referencedAssembly.FullName, targetFramework);
            }
            return targetFramework;
        }

        private void AddPackageReferences(IEnumerable<PackageReference> packageReference)
        {
            foreach(var packageByFramework in packageReference.GroupBy(pr => pr.TargetFramework))
            {
                Specification.Dependencies.Add(new PackageDependencyGroup(packageByFramework.Key, packageByFramework.Select(ToPackageDependency)));
            }
        }

        private PackageDependency ToPackageDependency(PackageReference packageReference)
        {
            return new PackageDependency(packageReference.PackageIdentity.Id, packageReference.AllowedVersions);
        }

        private void ProcessNuspecContent(NuspecReader reader)
        {
            if (Specification.Id == null)
            {
                Update(reader.GetId(), (id) => Specification.Id = id);
            }
            UpdateUrl(reader.GetLicenseUrl(), url => Specification.LicenseUrl = url, DEFAULT_LICENCE_URL);
            UpdateUrl(reader.GetProjectUrl(), url => Specification.ProjectUrl = url, DEFAULT_PROJECT_URL);
            UpdateUrl(reader.GetIconUrl(), url => Specification.IconUrl = url, DEFAULT_ICON_URL);
            UpdateList(reader.GetAuthors(), list => Specification.Authors = list);
            UpdateList(reader.GetOwners(), list => Specification.Owners = list);
            Update(reader.GetReleaseNotes(), notes => Specification.ReleaseNotes = notes, DEFAULT_RELEASE_NOTES);
            Update(reader.GetCopyright(), copyright => Specification.Copyright = copyright);
            UpdateList(reader.GetTags(), list => Specification.Tags = list, ' ', DEFAULT_TAGS);
            Update(reader.GetDescription(), desc => Specification.Description = desc);
        }

        private static bool UpdateList(string list, Action<string[]> setter, char separator = ',', string defaultValue = null)
        {
            if (!String.IsNullOrWhiteSpace(list) && !IsVariableToken(list) && String.Compare(list, defaultValue, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                setter(list.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries));
                return true;
            }
            return false;
        }

        private static bool UpdateUrl(string url, Action<Uri> setter, string defaultValue = null)
        {
            if (!String.IsNullOrWhiteSpace(url) && !IsVariableToken(url) && String.Compare(url, defaultValue, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                setter(new Uri(url));
                return true;
            }
            return false;
        }

        private static bool Update(string value, Action<string> setter, string defaultValue = null)
        {
            if (!String.IsNullOrWhiteSpace(value) && !IsVariableToken(value) && String.Compare(value, defaultValue, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                setter(value);
                return true;
            }
            return false;
        }

        private static bool IsVariableToken(string text)
        {
            string trimmed = text?.Trim();
            return trimmed != null && trimmed.StartsWith("$") && trimmed.EndsWith("$");
        }
    }
}