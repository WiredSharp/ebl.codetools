using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VSProjectNormalizer.Normalizers;

namespace VSProjectNormalizer
{
	public class VSProjectNormalizer
	{
		private const string CSHARP_PROJECT_TYPE = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		private const string WEB_APPLICATION_PROJECT_TYPE = "{349C5851-65DF-11DA-9384-00065B846F21}";

		protected Settings CurrentSettings { get; set; }

		public VSProjectNormalizer()
			: this(Settings.Default)
		{
		}

		public VSProjectNormalizer(Settings settings)
		{
			if (settings == null) throw new ArgumentNullException("settings");
			CurrentSettings = settings;
		}

		public string Normalize(TextReader projectFileReader)
		{
			return Normalize(XDocument.Load(projectFileReader));
		}

		public string Normalize(FileInfo projectFile)
		{
			return Normalize(XDocument.Load(projectFile.FullName));
		}

		public string Normalize(string projectFileContent)
		{
			return Normalize(XDocument.Parse(projectFileContent));
		}

		private string Normalize(XDocument document)
		{
			XElement root = document.Root;
			if (root != null)
			{
				if (IsCSharpProjectFile(root))
				{
					var normalizer = new CsharpProjectFileNormalizer(CurrentSettings);
					normalizer.Normalize(root);
				}
				else if (IsWebProjectFile(root))
				{
					var normalizer = new WebApplicationProjectFileNormalizer(CurrentSettings);
					normalizer.Normalize(root);
				}
			}
			return ToXml(document);
		}

		private static bool IsWebProjectFile(XElement root)
		{
			string projectType = GetProjectType(root);
			return projectType == null || projectType.Contains(WEB_APPLICATION_PROJECT_TYPE);
		}

		private static bool IsCSharpProjectFile(XElement root)
		{
			string projectType = GetProjectType(root);
			return projectType == null || projectType == CSHARP_PROJECT_TYPE;
		}

		private static string GetProjectType(XElement root)
		{
			XElement projectTypeNode =
				root.Descendants(root.GetDefaultNamespace().GetName("ProjectTypeGuids")).FirstOrDefault();
			return projectTypeNode != null ? projectTypeNode.Value : null;
		}

		private static string ToXml(XDocument document)
		{
			Encoding encoding;
			try
			{
				encoding = Encoding.GetEncoding(document.Declaration.Encoding);
			}
			catch
			{
				encoding = Encoding.UTF8;
			}
			using (var writer = new StringWriterWithEncoding(encoding))
			{
				document.Save(writer);
				writer.Flush();
				return writer.ToString();
			}
		}

		private class StringWriterWithEncoding : StringWriter
		{
			private readonly Encoding _encoding;

			public StringWriterWithEncoding(Encoding encoding)
			{
				_encoding = encoding;
			}

			public override Encoding Encoding
			{
				get { return _encoding; }
			}
		}
	}
}