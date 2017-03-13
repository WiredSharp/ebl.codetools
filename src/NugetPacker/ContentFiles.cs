namespace CodeTools.VisualStudio.Tools
{
    public class ContentFiles : Files
    {
        public string BuildAction { get; set; }

        public bool CopyToOutput { get; set; }

        public bool Flatten { get; set; }
    }
}