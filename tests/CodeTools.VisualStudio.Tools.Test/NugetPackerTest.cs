#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (17:26)
// ///
// ///
#endregion

using System;
using System.IO;
using CodeTools.Test.Common;
using NUnit.Framework;

namespace CodeTools.VisualStudio.Tools.Test
{
	/// <summary>
	/// update version
	/// update copyright
	/// update description
	/// update author
	/// update project url
	/// update framework assemblies
	/// update package dependencies from {project folder}\packages.config
	/// files should be handled by standard folder structure https://docs.microsoft.com/en-us/nuget/schema/nuspec
	/// </summary>
	[TestFixture]
	public class NugetPackerTest
	{
		private NugetPacker.Settings DefaultSettings { get; set; }

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			foreach (var file in Directory.GetFiles("".GetTestPath(), "test_pack.*"))
			{
				file.GetTestFileInfo().Delete();
			}
		}

		[SetUp]
		public void Setup()
		{
			DefaultSettings = new NugetPacker.Settings()
			{
				Id = "CodeTools.Test"
				, Version = new NugetPacker.Settings.NugetVersion(1,2,3,"test", "meta")
				, Copyright = "copyright 2017 EBL Consulting"
				, Description = "this is a description"
				, Authors = new[] {"author 1", "author 2"}
				, Owners = new[] {"owner 1", "owner 2"}
				, Tags = new[] {"tag 1", "tag 2"}
				, Title = "title"
			};
		}

		[Test]
		public void i_can_build_package_with_package_dependencies([Values("net35","net40","net403", "net45", "net451", "net452", "net461", "net462")] string targetFramework)
		{
			var packer = NewNugetPacker();
			DefaultSettings.Dependencies = new[]
			{
				new NugetPacker.Settings.DependencyGroup(targetFramework)
				{
					new NugetPacker.Settings.Dependency()
						{ Id = "another_package", VersionRange = "1.2.0" }
				}
			};
			packer.Pack($"test_pack.{targetFramework}".GetTestFileInfo());
		}

		[Test]
		public void i_can_build_package_with_library_content()
		{
			var packer = NewNugetPacker();
			DefaultSettings.Libraries = new[]
			{
				new NugetPacker.Settings.LibraryFiles()
				{
					Source = Path.Combine(@"regular\output\net40".GetTestPath(), "*.*")
					,TargetFramework = "net40"
				}
			};
			packer.Pack("test_pack".GetTestFileInfo());
		}

		[Test]
		public void i_can_build_package_with_tools_content()
		{
			var packer = NewNugetPacker();
			DefaultSettings.Tools = new[]
			{
				new NugetPacker.Settings.ToolFiles()
				{
					Source = "*.xml".GetTestPath()
				}
			};
			packer.Pack("test_pack".GetTestFileInfo());
		}

		[Test]
		public void i_can_build_package_with_build_content()
		{
			var packer = NewNugetPacker();
			DefaultSettings.Build = new[]
			{
				new NugetPacker.Settings.BuildFiles()
				{
					Source = "*.xml".GetTestPath()
				}
			};
			packer.Pack("test_pack".GetTestFileInfo());
		}

		[Test]
		public void i_can_build_package_with_two_framework_targets()
		{
			var packer = NewNugetPacker();
			DefaultSettings.Libraries = new[]
			{
				new NugetPacker.Settings.LibraryFiles()
				{
					Source = Path.Combine(@"regular\output\net40".GetTestPath(), "*.*")
					,TargetFramework = "net40"
				},
				new NugetPacker.Settings.LibraryFiles()
				{
					Source = Path.Combine(@"regular\output\net45".GetTestPath(), "*.*")
					,TargetFramework = "net45"
				}
			};
			packer.Pack("test_pack".GetTestFileInfo());
		}

		[Test]
		public void i_can_add_framework_assembly()
		{
			var packer = NewNugetPacker();
			DefaultSettings.FrameworkAssemblies = new[]
			{
				new NugetPacker.Settings.FrameworkAssembly()
				{
					Name = "System.Xml"
					,TargetFrameworks = new []{ "net40", "net45" }
				}
			};
			packer.Pack("test_pack".GetTestFileInfo());
		}

		private NugetPacker NewNugetPacker(NugetPacker.Settings settings = null)
		{
			return new NugetPacker(settings ?? DefaultSettings);
		}
	}
}