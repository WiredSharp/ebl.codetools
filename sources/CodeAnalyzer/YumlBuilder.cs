using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using CodeTools.Core;

namespace CodeAnalyzer
{
	internal class YumlBuilder : GraphBuilder
	{
		//[HttpContext]uses -.->[Response]
		//[❝Customer❞{bg:orange}]❶- ☂>[Order{bg:green}]
		private const string URL = "http://yuml.me/diagram/scruffy/class/";
		private const string SOLUTION_COLOR = "orange";
		private const string CPP_PROJET_COLOR = "yellowgreen";
		private const string CSHARP_PROJET_COLOR = "turquoise";
		private const string PROJECT_COLOR = "turquoise";
		private const string ERROR_COLOR = "tomato";
		private const string REFERENCE_COLOR = "";
		private const string EXTERNAL_REFERENCE_COLOR = "yellow";

		public static void CreateProjectDiagram(IEnumerable<Solution> solutions, string filePath)
		{
			var builder = new YumlBuilder();
			string query = builder.BuildQuery(solutions);
			File.WriteAllText(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".yuml"), query);
			LoadPOSTImage(query, filePath);
		}

		protected virtual string BuildQuery(IEnumerable<Solution> solutions)
		{
			StringBuilder url = new StringBuilder();
			var projects = new HashSet<string>();
			var knownSolutions = new Dictionary<string, int>();
			foreach (Solution solution in solutions)
			{
				string solutionName = GetSolutionName(solution, knownSolutions);
				foreach (KeyValuePair<string, CSharpProject> projectItem in solution.CSharpProjects)
				{
					string projectName = projectItem.Key;
					url.Append(NewSolutionProjectRelation(solutionName, projectName, projectItem.Value));
					if (!projects.Add(projectName + "-" + projectItem.Value.ProjectGuid))
						continue;
					//foreach (Reference reference in projectItem.Value.References)
					//{
					//	url.Append(NewProjectProjectRelation(projectName, projectItem.Value, reference));
					//}
					foreach (ProjectReference projectReference in projectItem.Value.LocalReferences)
					{
						url.Append(NewProjectProjectRelation(projectName, projectItem.Value, projectReference));
					}
				}
				foreach (KeyValuePair<string, CppProject> projectItem in solution.CppProjects)
				{
					string projectName = projectItem.Key;
					url.Append(NewSolutionProjectRelation(solutionName, projectName, projectItem.Value));
					if (!projects.Add(projectName + "-" + projectItem.Value.ProjectGuid))
						continue;
					//foreach (Reference reference in projectItem.Value.References)
					//{
					//    url.Append(NewProjectProjectRelation(projectName, projectItem.Value, reference));
					//}
					//foreach (ProjectReference projectReference in projectItem.Value.LocalReferences)
					//{
					//	url.Append(NewProjectProjectRelation(projectName, projectItem.Value, projectReference));
					//}
				}
			}
			return url.ToString();
		}

		private static string GetSolutionName(Solution solution, IDictionary<string, int> knownSolutions)
		{
			int index = 0;
			if (!knownSolutions.ContainsKey(solution.Name))
			{
				knownSolutions.Add(solution.Name, 1);
			}
			else
			{
				index = knownSolutions[solution.Name];
				knownSolutions[solution.Name] = index + 1;
			}
			return index == 0 ? solution.Name : solution.Name + "_" + index.ToString("00");
		}

		public static void LoadPOSTImage(string query, string filePath)
		{
			using (var client = new WebClient())
			{
				IWebProxy wp = WebRequest.DefaultWebProxy;
				wp.Credentials = CredentialCache.DefaultCredentials;
				client.Proxy = wp;
				try
				{
					byte[] response =
							 client.UploadValues(URL, new NameValueCollection()
                           {
                               { "dsl_text", query }
                           });
					string result = Encoding.UTF8.GetString(response);
					LoadGETImage(result, filePath);
				}
				catch (WebException webEx)
				{
					Console.WriteLine(webEx.ToString());
					if (webEx.Status == WebExceptionStatus.ConnectFailure)
					{
						Trace.TraceWarning("Are you behind a firewall?  If so, go through the proxy server.");
					}
				}
				catch (Exception exception)
				{
					Trace.TraceError("unable to load yuml diagram: " + exception);
				}
			}
		}

		private static void LoadGETImage(string query, string filePath)
		{
			using (var client = new WebClient())
			{
				IWebProxy wp = WebRequest.DefaultWebProxy;
				wp.Credentials = CredentialCache.DefaultCredentials;
				client.Proxy = wp;
				try
				{
					client.DownloadFile(URL + query, filePath);
				}
				catch (WebException webEx)
				{
					Console.WriteLine(webEx.ToString());
					if (webEx.Status == WebExceptionStatus.ConnectFailure)
					{
						Trace.TraceWarning("Are you behind a firewall?  If so, go through the proxy server.");
					}
				}
				catch (Exception exception)
				{
					Trace.TraceError("unable to load yuml diagram: " + exception);
				}
			}
		}

		protected string NewSolutionProjectRelation(string solution, string projectName, Project project)
		{
			return String.Format("[{0}{{bg:{1}}}]-.->[{2}-{3}{{bg:{4}}}],", solution + "_S", GetSolutionColor(solution), projectName, ToProjectId(project), GetProjetColor(projectName, project));
		}

		private string NewProjectProjectRelation(string projectName, Project project, Reference reference)
		{
			return String.Format("[{0}-{1}{{bg:{2}}}]->[{3}{{bg:{4}}}],", projectName, ToProjectId(project), GetProjetColor(projectName, project), reference.Name, EXTERNAL_REFERENCE_COLOR);
		}

		protected string NewProjectProjectRelation(string projectName, Project project, ProjectReference reference)
		{
			return String.Format("[{0}-{1}{{bg:{2}}}]->[{3}-{4}{{bg:{5}}}],", projectName, ToProjectId(project), GetProjetColor(projectName, project), reference.Name, ToProjectId(reference), REFERENCE_COLOR);
		}

		private static string GetSolutionColor(string solutionName)
		{
			switch (solutionName.ToUpper())
			{
				default:
					return SOLUTION_COLOR;
			}
		}

		private static string GetProjetColor(string projectName)
		{
			switch (projectName)
			{
				default:
					return PROJECT_COLOR;
			}
		}

		private static string GetProjetColor(string projectName, Project project)
		{
			var type = project.GetType();
			if (!project.Path.Valid)
			{
				return ERROR_COLOR;
			}
			else if (type == typeof(CppProject))
			{
				return CPP_PROJET_COLOR;
			}
			else
			{
				return CSHARP_PROJET_COLOR;				
			}
		}
	}
}