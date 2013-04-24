using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDoc
{
    public class DocParser
    {

        public static DocumentData Parse(string projectPath)
        {
            var workspace = Roslyn.Services.Workspace.LoadStandAloneProject(projectPath);
            var compilation = workspace.CurrentSolution.Projects.First().GetCompilation();


            return Parse(compilation);
        }

        public static DocumentData Parse(CommonCompilation compilation)
        {
            var data = new DocumentData();

            var globalNamespace = compilation.GlobalNamespace;
            var namespaces = globalNamespace.GetNamespaceMembers();
            foreach (var namespaceSymbol in namespaces)
            {
                if (namespaceSymbol.Name == "Facebook")
                {
                    ParseNamespace(data, namespaceSymbol, null);
                }
            }

            return data;
        }

        private static T CreateDocumentData<T>(ISymbol symbol, string rootName) where T : DocumentDataObject, new()
        {
            var data = new T();
            data.Name = symbol.Name;
            data.DisplayName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            if (rootName != null)
            {
                data.DisplayName = data.DisplayName.Replace(rootName, string.Empty).Substring(1);
            }

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

        private static void ParseNamespace(DocumentData parent, INamespaceSymbol symbol, string rootName)
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

        private static void ParseTypeMember(NamespaceDocumentData parent, INamedTypeSymbol symbol, string rootName)
        {
            var data = CreateDocumentData<NamedTypeDocumentData>(symbol, rootName);
            parent.AddNamedType(data);

            // Process Members
            var members = symbol.GetMembers();
            foreach (var member in members)
            {
                switch (member.Kind)
                {
                    case CommonSymbolKind.Method:

                        ParseMethod(data, (IMethodSymbol)member, data.FullName);
                        break;
                    case CommonSymbolKind.Property:

                        ParseProperty(data, (IPropertySymbol)member, data.FullName);
                        break;
                }
            }
        }

        private static void ParseMethod(NamedTypeDocumentData parent, IMethodSymbol symbol, string rootName)
        {
            if (symbol.AssociatedPropertyOrEvent != null)
            {
                // We don't want to include methods that are associated with
                // events or properties.
                return;
            }
            var data = CreateDocumentData<MethodDocumentData>(symbol, rootName);
            parent.AddMethod(data);

            var parameters = symbol.Parameters;
            foreach (var parameter in parameters)
            {
                var parameterData = CreateDocumentData<MethodParameterData>(parameter, null);
                parameterData.Type = CreateDocumentData<DocumentDataObject>(parameter.Type, null);
                data.Parameters.Add(parameterData);
            }

            var typeArguments = symbol.TypeArguments;
            foreach (var typeArgument in typeArguments)
            {
                var typeArgumentData = CreateDocumentData<MethodTypeArgumentData>(typeArgument, null);
                data.TypeArguments.Add(typeArgumentData);
            }
        }

        private static void ParseProperty(NamedTypeDocumentData parent, IPropertySymbol symbol, string rootName)
        {
            var data = CreateDocumentData<PropertyDocumentData>(symbol, rootName);
            parent.AddProperty(data);
        }

    }

}
