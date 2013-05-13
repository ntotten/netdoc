namespace NetDoc.Parser
{
    using System.Collections.Generic;

    public class Configuration
    {
        public ICollection<ProjectInfo> Projects { get; set; }

        public ICollection<string> FilteredNamespaces { get; set; }
    }
}
