using System;

namespace CodeAnalyzer
{
    internal abstract class GraphBuilder
    {
        protected static string ToProjectId(ProjectReference reference)
        {
            return ToProjectId(reference.ProjectId);
        }

        protected static string ToProjectId(Guid projectId)
        {
            return projectId.ToString().Substring(30);
        }

        protected static string ToProjectId(Project project)
        {
            return ToProjectId(project.ProjectGuid);
        }
    }
}