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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Hosting;
using System.Security.Policy;
using System.Xml.Linq;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Versioning;

namespace CodeTools.VisualStudio.Tools
{
    public class SpecificationBuilder
    {
        private const string DEFAULT_ICON_URL = "http://ICON_URL_HERE_OR_DELETE_THIS_LINE";
        private const string DEFAULT_LICENCE_URL = "http://LICENCE_URL_HERE_OR_DELETE_THIS_LINE";
        private const string DEFAULT_PROJECT_URL = "http://PROJECT_URL_HERE_OR_DELETE_THIS_LINE";
        private const string DEFAULT_TAGS = "Tag1 Tag2";
        private const string DEFAULT_RELEASE_NOTES = "Summary of changes made in this release of the package.";

        internal class FrameworkAssemblyCollection : KeyedCollection<string, FrameworkAssemblyReference>
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
                    var targetFmks = new HashSet<NuGetFramework> {targetFramework};
                    foreach (NuGetFramework framework in fmkRef.SupportedFrameworks)
                    {
                        targetFmks.Add(framework);
                    }
                    SetItem(IndexOf(fmkRef), new FrameworkAssemblyReference(assemblyName, targetFmks));
                }
                else
                {
                    Add(new FrameworkAssemblyReference(assemblyName, new[] {targetFramework}));
                }
            }
        }

        internal List<PackageReference> PackageReferences { get; private set; }

        internal FrameworkAssemblyCollection FrameworkAssemblies { get; private set; }

        internal NuGetFramework TargetFramework { get; private set; }

        internal string[] Owners { get; private set; }

        internal string[] Authors { get; private set; }

        internal string[] Tags { get; private set; }

        internal Uri LicenseUrl { get; private set; }

        internal string Id { get; private set; }

        internal string Description { get; private set; }

        internal Uri ProjectUrl { get; private set; }

        internal NuGetVersion Version { get; private set; }

        internal string Copyright { get; private set; }

        internal string ReleaseNotes { get; private set; }

        internal Uri IconUrl { get; private set; }

        public SpecificationBuilder()
        {
            PackageReferences = new List<PackageReference>();
            FrameworkAssemblies = new FrameworkAssemblyCollection();
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
            ProcessAssembly(assembly);
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
            foreach (PackageReference packageReference in parser.GetPackages())
            {
                AddPackageReference(packageReference);
            }
        }

        private void ProcessAssembly(Assembly assembly)
        {
            IList<CustomAttributeData> attributesData = assembly.GetCustomAttributesData();
            Id = assembly.FullName;
            foreach (CustomAttributeData attributeData in attributesData)
            {
                if (attributeData.Constructor.DeclaringType == typeof(AssemblyCopyrightAttribute))
                {
                    Copyright = attributeData.ConstructorArguments.First().ToString().Trim(new[] { '"' });
                }
                else if (attributeData.Constructor.DeclaringType == typeof(AssemblyInformationalVersionAttribute))
                {
                    string[] version = attributeData.ConstructorArguments.First().ToString().Trim(new []{'"'}).Split(new []{'-'}, StringSplitOptions.RemoveEmptyEntries);
                    if (version.Length == 1)
                    {
                        Version = NuGetVersion.Parse(version[0]);
                    }
                    else
                    {
                        Version = new NuGetVersion(System.Version.Parse(version[0]), version[1]);
                    }
                }
                else if (attributeData.Constructor.DeclaringType ==
                         typeof(System.Runtime.Versioning.TargetFrameworkAttribute))
                {
                    TargetFramework = NuGetFramework.ParseFrameworkName(
                                                                         attributeData.NamedArguments.First().TypedValue.Value.ToString().Trim(new[] { '"' }),
                                                                         DefaultFrameworkNameProvider.Instance);
                }
            }
            foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
            {
                FrameworkAssemblies.Add(referencedAssembly.FullName, TargetFramework);
            }
        }

        private void AddPackageReference(PackageReference packageReference)
        {
            PackageReferences.Add(packageReference);
        }

        private void ProcessNuspecContent(NuspecReader reader)
        {
            if (Id == null)
            {
                Update(reader.GetId(), (id) => Id = id);
            }
            UpdateUrl(reader.GetLicenseUrl(), url => LicenseUrl = url, DEFAULT_LICENCE_URL);
            UpdateUrl(reader.GetProjectUrl(), url => ProjectUrl = url, DEFAULT_PROJECT_URL);
            UpdateUrl(reader.GetIconUrl(), url => IconUrl = url, DEFAULT_ICON_URL);
            UpdateList(reader.GetAuthors(), list => Authors = list);
            UpdateList(reader.GetOwners(), list => Owners = list);
            Update(reader.GetReleaseNotes(), notes => ReleaseNotes = notes, DEFAULT_RELEASE_NOTES);
            Update(reader.GetCopyright(), copyright => Copyright = copyright);
            UpdateList(reader.GetTags(), list => Tags = list, ' ', DEFAULT_TAGS);
            Update(reader.GetDescription(), desc => Description = desc);
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