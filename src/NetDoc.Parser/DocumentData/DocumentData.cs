﻿namespace NetDoc.Parser.Model
{
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;
    using System.Linq;

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
            if (data != null)
            {
                var key = data.Name ?? string.Empty;
                this.namespaces.Add(key, data);
            }
        }

        internal NamespaceDocumentData GetNamespace(INamespaceSymbol symbol)
        {
            var name = symbol.Name ?? string.Empty;
            return this.namespaces.ContainsKey(name) ? this.namespaces[name] : null;
        }
    }
}
