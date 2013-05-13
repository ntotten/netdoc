namespace NetDoc.Parser.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class NamespaceDocumentData : DocumentDataObject
    {
        private SortedList<string, NamedTypeDocumentData> namedTypes = new SortedList<string, NamedTypeDocumentData>();

        public IEnumerable<NamedTypeDocumentData> NamedTypes
        {
            get
            {
                return this.namedTypes.Values.ToArray();
            }
        }

        public void AddNamedType(NamedTypeDocumentData data)
        {
            if (data != null)
            {
                data.GenerateId();
                this.namedTypes.Add(data.Id, data);
            }
        }

        internal NamedTypeDocumentData GetTypeMember(string fullName)
        {
            return this.namedTypes.ContainsKey(fullName) ? this.namedTypes[fullName] : null;
        }
    }
}
