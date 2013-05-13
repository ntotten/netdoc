namespace NetDoc.Parser.Model
{
    using System.Collections.Generic;

    public class DocumentDataObject
    {
        public DocumentDataObject()
        {
            this.SupportedProjects = new List<string>();
        }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string DisplayName { get; set; }

        public string Summary { get; set; }

        public string ReturnDescription { get; set; }

        public string AccessModifier { get; set; }

        public ICollection<string> SupportedProjects { get; set; }
    }
}
