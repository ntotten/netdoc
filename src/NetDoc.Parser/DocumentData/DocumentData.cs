namespace NetDoc.Parser.DocumentData
{
    using System.Collections.Generic;

    public class DocumentData
    {
        private SortedList<string, NamespaceDocumentData> namespaces = new SortedList<string, NamespaceDocumentData>();

        public IEnumerable<NamespaceDocumentData> Namespaces
        {
            get
            {
                return this.namespaces.Values;
            }
        }

        public void AddNamespace(NamespaceDocumentData data)
        {
            var key = data.Name ?? string.Empty;
            this.namespaces.Add(key, data);
        }
    }
}
