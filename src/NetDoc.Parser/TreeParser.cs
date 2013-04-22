using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDoc
{
    public static class TreeParser
    {

        public static DocumentData Parse(IEnumerable<SyntaxTree> trees)
        {
            var compilation = Compilation.Create("doc", syntaxTrees: trees);

            var data = new DocumentData();

            var globalNamespace = compilation.GlobalNamespace;
            var namespaces = globalNamespace.GetNamespaceMembers();
            foreach (var namespaceSymbol in namespaces)
            {
                ParseNamespace(data, namespaceSymbol, null);
            }

            return data;
        }

        private static T CreateDocumentData<T>(Roslyn.Compilers.Common.ISymbol symbol, string rootName) where T: DocumentDataBase, new()
        {
            var data = new T();
            data.Name = symbol.Name;

            if (string.IsNullOrEmpty(rootName))
            {
                data.FullName = data.Name;
            }
            else
            {
                data.FullName = rootName + "." + data.Name;
            }

            DocumentationComment comment = null;
            try
            {
                comment = symbol.GetDocumentationComment();
            }
            catch
            {

            }
            if (comment != null)
            {
                data.Summary = comment.SummaryTextOpt;
            }
            return data;
        }

        private static void ParseNamespace(DocumentData parent, NamespaceSymbol symbol, string rootName)
        {
            var data = CreateDocumentData<NamespaceDocumentData>(symbol, rootName);
            parent.AddNamespace(data);


            // Namespaces
            var namespaces = symbol.GetNamespaceMembers();
            foreach (var namespaceSymbol in namespaces)
            {
                ParseNamespace(parent, namespaceSymbol, rootName: data.Name);
            }

            // Types
            var typeMembers = symbol.GetTypeMembers();
            foreach (var typeMember in typeMembers)
            {
                ParseTypeMember(data, typeMember, data.FullName);
            }
        }

        private static void ParseTypeMember(NamespaceDocumentData parent, NamedTypeSymbol symbol, string rootName)
        {
            var data = CreateDocumentData<NamedTypeDocumentData>(symbol, rootName);
            parent.AddNamedType(data);

            // Process Members
            var members = symbol.GetMembers();
            foreach (var member in members)
            {
                switch (member.Kind)
                {
                    case SymbolKind.Method:

                        ParseMethod(data, (MethodSymbol)member, data.FullName);
                        break;
                    case SymbolKind.Property:

                        ParseProperty(data, (PropertySymbol)member, data.FullName);
                        break;
                }
            }
        }

        private static void ParseMethod(NamedTypeDocumentData parent, MethodSymbol symbol, string rootName)
        {
            if (symbol.AssociatedPropertyOrEvent != null)
            {
                // We don't want to include methods that are associated with
                // events or properties.
                return;
            }
            var data = CreateDocumentData<MethodDocumentData>(symbol, rootName);
            parent.AddMethod(data);
        }

        private static void ParseProperty(NamedTypeDocumentData parent, PropertySymbol symbol, string rootName)
        {
            var data = CreateDocumentData<PropertyDocumentData>(symbol, rootName);
            parent.AddProperty(data);
        }

    }
}
