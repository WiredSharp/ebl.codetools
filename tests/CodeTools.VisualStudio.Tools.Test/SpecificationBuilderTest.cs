#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-13 (10:49)
// ///
// ///
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.Test.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NUnit.Framework;

namespace CodeTools.VisualStudio.Tools.Test
{
    [TestFixture]
    public class SpecificationBuilderTest
    {
        [Test]
        public void i_can_retrieve_id_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("id").Single().Value, builder.Id, "unexpected id retrieved");
        }

        [Test]
        public void i_can_retrieve_authors_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            string[] expected = xmlNuspec.FindNodes("authors").Single().Value.Split(',');
            CollectionAssert.AreEqual(expected, builder.Authors, "unexpected authors retrieved");
        }

        [Test]
        public void i_can_retrieve_owners_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            string[] expected = xmlNuspec.FindNodes("owners").Single().Value.Split(',');
            CollectionAssert.AreEqual(expected, builder.Owners, "unexpected owners retrieved");
        }

        [Test]
        public void i_can_retrieve_tags_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            string[] expected = xmlNuspec.FindNodes("tags").Single().Value.Split(' ');
            CollectionAssert.AreEqual(expected, builder.Tags, "unexpected tags retrieved");
        }

        [Test]
        public void i_can_retrieve_projectUrl_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("projectUrl").Single().Value, builder.ProjectUrl.OriginalString, "unexpected project url retrieved");
        }

        [Test]
        public void i_can_retrieve_licenceUrl_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("licenseUrl").Single().Value, builder.LicenseUrl.OriginalString, "unexpected license url retrieved");
        }

        [Test]
        public void i_can_retrieve_iconUrl_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("iconUrl").Single().Value, builder.IconUrl.OriginalString, "unexpected icon url retrieved");
        }

        [Test]
        public void i_can_retrieve_description_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("description").Single().Value, builder.Description, "unexpected description retrieved");
        }

        [Test]
        public void i_can_retrieve_releaseNotes_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("releaseNotes").Single().Value, builder.ReleaseNotes, "unexpected release notes retrieved");
        }

        [Test]
        public void i_can_retrieve_copyright_from_nuspec_file()
        {
            var builder = new SpecificationBuilder();
            FileInfo nuspecFile = @"regular\regular.nuspec".GetTestFileInfo();
            builder.WithNuSpec(nuspecFile);
            XElement xmlNuspec = nuspecFile.LoadXml();
            Assert.AreEqual(xmlNuspec.FindNodes("copyright").Single().Value, builder.Copyright, "unexpected copyright retrieved");
        }

        [Test]
        public void i_can_inject_package_dependencies()
        {
            var builder = new SpecificationBuilder();
            FileInfo packagesFile = @"regular\packages.config".GetTestFileInfo();
            builder.WithPackages(packagesFile);
            XElement packages = packagesFile.LoadXml();
            CollectionAssert.AreEqual(packages.FindNodes("package").Select(ToPackageReference), builder.PackageReferences, PackageReferenceComparer.Default, "unexpected package references retrieved");
        }

        private PackageReference ToPackageReference(XElement packageNode)
        {
            return new PackageReference(
                                 new PackageIdentity(packageNode.Attribute("id").Value,
                                                     NuGetVersion.Parse(packageNode.Attribute("version").Value)),
                                 NuGetFramework.Parse(packageNode.Attribute("targetFramework").Value));
        }

        [Test]
        public void i_can_retrieve_version_from_assembly()
        {
            var builder = new SpecificationBuilder();
            builder.WithAssembly(@"regular\output\net40\EDO.Diagnostics.Core.dll".GetTestFileInfo());
            Assert.AreEqual(new NuGetVersion(1,0,0,new [] { "PreRelease" }, null), builder.Version, "unexpected version retrieved from assembly");
        }

        [Test]
        public void i_can_retrieve_copyright_from_assembly()
        {
            var builder = new SpecificationBuilder();
            builder.WithAssembly(@"regular\output\net40\EDO.Diagnostics.Core.dll".GetTestFileInfo());
            Assert.AreEqual("Copyright © OFI-AM 2017", builder.Copyright, "unexpected version retrieved from assembly");
        }

        [Test]
        public void framework_assemblies_are_updated()
        {
            var builder = new SpecificationBuilder();
            builder.WithAssembly(@"regular\output\net40\EDO.Diagnostics.Core.dll".GetTestFileInfo());
            var nuGetFrameworks = new List<NuGetFramework>() { NuGetFramework.ParseFrameworkName(".Net Framework 4", DefaultFrameworkNameProvider.Instance) };
            var expectedAssemblies = new List<FrameworkAssemblyReference>()
                                     {
                                         new FrameworkAssemblyReference("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", nuGetFrameworks)
                                         ,new FrameworkAssemblyReference("System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", nuGetFrameworks)
                                     };
            CollectionAssert.AreEqual(expectedAssemblies, builder.FrameworkAssemblies, FrameworkAssemblyReferenceComparer.Default, "unexpected version retrieved from assembly");
        }

        [Test]
        public void with_two_assemblies_framework_assemblies_are_updated_in_correct_group()
        {
            var builder = new SpecificationBuilder();
            builder.WithAssembly(@"regular\output\net40\EDO.Diagnostics.Core.dll".GetTestFileInfo());
            builder.WithAssembly(@"regular\output\net45\EDO.Diagnostics.Core.dll".GetTestFileInfo());
            var nuGetFrameworks = new List<NuGetFramework>()
                                  {
                                      NuGetFramework.ParseFrameworkName(".Net Framework 4", DefaultFrameworkNameProvider.Instance)
                                      ,NuGetFramework.ParseFrameworkName(".Net Framework 4.5.2", DefaultFrameworkNameProvider.Instance)
                                  };
            var expectedAssemblies = new List<FrameworkAssemblyReference>()
                                     {
                                         new FrameworkAssemblyReference("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", nuGetFrameworks)
                                         ,new FrameworkAssemblyReference("System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", nuGetFrameworks)
                                     };
            CollectionAssert.AreEqual(expectedAssemblies, builder.FrameworkAssemblies, FrameworkAssemblyReferenceComparer.Default, "unexpected version retrieved from assembly");
        }
    }

    internal class FrameworkAssemblyReferenceComparer : IComparer<FrameworkAssemblyReference>, IComparer
    {
        public static IComparer Default => new FrameworkAssemblyReferenceComparer();

        public int Compare(FrameworkAssemblyReference x, FrameworkAssemblyReference y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            if (String.Compare(x.AssemblyName, y.AssemblyName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                if (x.SupportedFrameworks.Count() == y.SupportedFrameworks.Count())
                {
                    foreach (NuGetFramework framework in x.SupportedFrameworks)
                    {
                        if (!y.SupportedFrameworks.Any(fmk => fmk.Equals(framework)))
                        {
                            return -1;
                        }
                    }
                    return 0;
                }
            }
            return -1;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as FrameworkAssemblyReference, y as FrameworkAssemblyReference);
        }
    }

    internal class PackageReferenceComparer: IComparer<PackageReference>, IComparer
    {
        public static IComparer Default => new PackageReferenceComparer();

        public int Compare(PackageReference x, PackageReference y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            if (x.PackageIdentity.Equals(y.PackageIdentity) && x.TargetFramework.Equals(y.TargetFramework))
            {
                return 0;
            }
            return 1;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as PackageReference, y as PackageReference);
        }
    }
}