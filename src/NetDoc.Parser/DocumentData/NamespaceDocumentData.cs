namespace NetDoc.Parser.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class NamespaceDocumentData : DocumentDataObject
    {
        private List<NamedTypeDocumentData> namedTypes = new List<NamedTypeDocumentData>();

        public IEnumerable<NamedTypeDocumentData> NamedTypes
        {
            get
            {
                return this.namedTypes.OrderBy(m => m.Name).ToArray();
            }
        }

        public void AddNamedType(NamedTypeDocumentData data)
        {
            if (data != null)
            {
                data.GenerateId();
                this.namedTypes.Add(data);
            }
        }
    }
}
