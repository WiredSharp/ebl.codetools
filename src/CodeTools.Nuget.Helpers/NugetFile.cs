#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (17:48)
// ///
// ///
#endregion

using System;
using System.IO;
using System.Xml.Linq;
using CodeTools.Helpers.Core;

namespace CodeTools.Nuget.Helpers
{
    public class NugetFile : XmlFileWrapper
    {
        public static NugetFile Create()
        {
            return new NugetFile(@"<?xml version=""1.0""?>
            <package/>");
        }

        public static NugetFile Load(FileInfo nugetFile)
        {
            return new NugetFile(nugetFile);
        }

        public NugetFile(FileInfo file)
            : base(file)
        {
        }

        public NugetFile(string fileContent) : base(fileContent)
        {
        }

        public string Id
        {
            get { return GetMetadataValue(nameof(Id)); }
            set
            {
                SetMetadataValue(nameof(Id), value);
            }
        }

        public string Version
        {
            get { return GetMetadataValue(nameof(Version)); }
            set
            {
                SetMetadataValue(nameof(Version), value);
            }
        }

        public string[] Authors
        {
            get { return GetMetadataArrayValue(nameof(Authors)); }
            set
            {
                SetMetadataArrayValue(nameof(Authors), value);
            }
        }

        public string[] Owners
        {
            get { return GetMetadataArrayValue(nameof(Owners)); }
            set
            {
                SetMetadataArrayValue(nameof(Owners), value);
            }
        }

        public string Title
        {
            get { return GetMetadataValue(nameof(Title)); }
            set
            {
                SetMetadataValue(nameof(Title), value);
            }
        }

        public string Description
        {
            get { return GetMetadataValue(nameof(Description)); }
            set
            {
                SetMetadataValue(nameof(Description), value);
            }
        }

        public Uri ProjectUrl
        {
            get {
                return GetMetadataUriValue(nameof(ProjectUrl));
            }
            set {
                SetMetadataUriValue(nameof(ProjectUrl), value);
            }
        }

        public Uri LicenseUrl
        {
            get
            {
                return GetMetadataUriValue(nameof(LicenseUrl));
            }
            set
            {
                SetMetadataUriValue(nameof(LicenseUrl), value);
            }
        }

        public Uri IconUrl
        {
            get
            {
                return GetMetadataUriValue(nameof(IconUrl));
            }
            set
            {
                SetMetadataUriValue(nameof(IconUrl), value);
            }
        }

        public bool? RequireLicenseAcceptance
        {
            get
            {
                string metadataValue = GetMetadataValue(nameof(RequireLicenseAcceptance));
                if (metadataValue != null)
                {
                    return Boolean.Parse(metadataValue);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                SetMetadataValue(nameof(RequireLicenseAcceptance), value == null ? "" : value.ToString());
            }
        }

        public string Summary
        {
            get { return GetMetadataValue(nameof(Summary)); }
            set
            {
                SetMetadataValue(nameof(Summary), value);
            }
        }

        public string ReleaseNotes
        {
            get { return GetMetadataValue(nameof(ReleaseNotes)); }
            set
            {
                SetMetadataValue(nameof(ReleaseNotes), value);
            }
        }

        public string Copyright
        {
            get { return GetMetadataValue(nameof(Copyright)); }
            set
            {
                SetMetadataValue(nameof(Copyright), value);
            }
        }

        public string Language
        {
            get { return GetMetadataValue(nameof(Language)); }
            set
            {
                SetMetadataValue(nameof(Language), value);
            }
        }

        public string[] Tags
        {
            get { return GetMetadataArrayValue(nameof(Tags), ' '); }
            set
            {
                SetMetadataArrayValue(nameof(Tags), value, " ");
            }
        }

        private string[] GetMetadataArrayValue(string localName, char separator = ',')
        {
            string metadataValue = GetMetadataValue(localName);
            if (metadataValue != null)
            {
                return metadataValue.Split(separator);
            }
            else
            {
                return null;
            }
        }

        private void SetMetadataArrayValue(string localName, string[] array, string separator = ",")
        {
            if (array == null)
            {
                SetMetadataValue(localName, String.Empty);
            }
            else
            {
                SetMetadataValue(localName, String.Join(separator, array));
            }
        }

        private void SetMetadataUriValue(string localName, Uri value)
        {
            SetMetadataValue(localName, value == null ? "" : value.ToString().TrimEnd('/'));
        }

        private Uri GetMetadataUriValue(string localName)
        {
            string metadataValue = GetMetadataValue(localName);
            if (metadataValue != null)
            {
                return new Uri(metadataValue);
            }
            else
            {
                return null;
            }
        }

        private void SetMetadataValue(string localName, string value)
        {
            TryAddMetadata(localName).Value = value;
        }

        private string GetMetadataValue(string name)
        {
            return Metadata?.Element(name)?.Value;
        }

        private XElement TryAddMetadata(string localName)
        {
            return Root.TryAdd("package").TryAdd("metadata").TryAdd(localName.ToPascalCase());
        }

        private XElement Metadata => Root.Element("package")?.Element("metadata");
    }
}