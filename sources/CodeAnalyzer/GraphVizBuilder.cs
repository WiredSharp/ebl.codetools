using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeAnalyzer
{
    internal class GraphVizBuilder : GraphBuilder
    {
        protected string Separator
        {
            get { return ";" + Environment.NewLine; }
        }

        public static void CreateProjectDiagram(IEnumerable<Solution> solutions, string filePath)
        {
            var builder = new GraphVizBuilder();
            File.WriteAllText(filePath, builder.BuildQuery(solutions));
        }
        protected virtual string BuildQuery(IEnumerable<Solution> solutions)
        {
            StringBuilder url = new StringBuilder("digraph Solutions { " + Environment.NewLine);
            var projects = new HashSet<string>();
            foreach (Solution solution in solutions)
            {
                url.Append(BuildNode(solution) + "[label=\"" + solution.Name + "\"]" + Separator);
                foreach (KeyValuePair<string, CSharpProject> projectItem in solution.CSharpProjects)
                {
                    if (projects.Add(projectItem.Key + "-" + projectItem.Value.ProjectGuid))
                    {
                        url.Append(BuildNode(projectItem.Key, projectItem.Value) + "[shape=box, label=\"" + projectItem.Key + "\"]" + Separator);
                    }
                        url.Append(NewSolutionProjectRelation(solution, projectItem.Key, projectItem.Value));
                    if (!projects.Add(projectItem.Key + "-" + projectItem.Value.ProjectGuid))
                        continue;
                    foreach (ProjectReference projectReference in projectItem.Value.LocalReferences)
                    {
                        url.Append(NewProjectProjectRelation(projectItem.Key, projectItem.Value, projectReference));
                    }
                }
            }
            url.Append(Environment.NewLine + "}");
            return url.ToString();
        }

        protected string NewProjectProjectRelation(string projectName, CSharpProject project, ProjectReference reference)
        {
            return BuildNode(projectName, project) + "->" + BuildNode(reference) + Separator;
        }

        protected string NewSolutionProjectRelation(Solution solution, string projectName, CSharpProject project)
        {
            return BuildNode(solution) + "->" + BuildNode(projectName, project) + " [style=dotted]" + Separator;
        }

        private static string BuildNode(ProjectReference reference)
        {
            return Normalize(reference.Name) + ToProjectId(reference);// + "[label=\"" + reference.Name + "\"]";
        }

        private static string BuildNode(string projectName, CSharpProject project)
        {
            return Normalize(projectName) + ToProjectId(project);// + "[label=\"" + projectName + "\", style=dotted]";
        }

        private static string BuildNode(Solution solution)
        {
            return Normalize(solution.Name) + "_S";// " [label=\"" + solution + "\"]";
        }

        private static string Normalize(string projectName)
        {
            return projectName.Replace(".", "_");
        }
    }
}