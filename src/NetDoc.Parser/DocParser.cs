namespace NetDoc.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetDoc.Parser.Model;
    using Roslyn.Compilers;
    using Roslyn.Compilers.Common;

    public class DocParser
    {
        public DocParser()
        {
            this.Data = new DocumentData();
        }

        public DocumentData Data { get; set; }

        public void Parse(IDictionary<string, string>[] projects, IEnumerable<string> namespacesBegins)
        {
            foreach (var project in projects)
            {
                var workspace = Roslyn.Services.Workspace.LoadStandAloneProject(project["path"]);
                var compilation = workspace.CurrentSolution.Projects.First().GetCompilation();

                this.Parse(compilation, namespacesBegins);
            }
        }

        public void Parse(string projectPath, IEnumerable<string> namespacesBegins)
        {
            var workspace = Roslyn.Services.Workspace.LoadStandAloneProject(projectPath);
            var compilation = workspace.CurrentSolution.Projects.First().GetCompilation();

            this.Parse(compilation, namespacesBegins);
        }

        public void Parse(CommonCompilation compilation, IEnumerable<string> namespacesBegins)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException("compilation");
            }

            var globalNamespace = compilation.GlobalNamespace;
            var namespaces = globalNamespace.GetNamespaceMembers();
            foreach (var namespaceSymbol in namespaces)
            {
                if (namespacesBegins == null || namespacesBegins.Count() == 0 || namespacesBegins.Any(n => namespaceSymbol.Name.StartsWith(n, StringComparison.OrdinalIgnoreCase)))
                {
                    ParseNamespace(this.Data, namespaceSymbol, null);
                }
            }
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

            data.AccessModifier = Helper.RetrieveAccessModifier(symbol.DeclaredAccessibility);
            data.FullName = string.IsNullOrEmpty(rootName) ? data.Name : rootName + "." + data.Name;

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
                data.ReturnDescription = comment.ReturnsTextOpt;
            }

            return data;
        }

        private static void ParseNamespace(DocumentData parent, INamespaceSymbol symbol, string rootName)
        {
            var data = parent.GetNamespace(symbol);
            if (data == null)
            {
                data = CreateDocumentData<NamespaceDocumentData>(symbol, rootName);
                parent.AddNamespace(data);
            }

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
            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = CreateDocumentData<NamedTypeDocumentData>(symbol, rootName);
            parent.AddNamedType(data);

            // Process Members
            var members = symbol.GetMembers();
            foreach (var member in members)
            {
                switch (member.Kind)
                {
                    case CommonSymbolKind.Field:

                        ParseField(data, (IFieldSymbol)member, data.FullName);
                        break;
                    case CommonSymbolKind.Event:

                        ParseEvent(data, (IEventSymbol)member, data.FullName);
                        break;
                    case CommonSymbolKind.Method:

                        ParseMethod(data, (IMethodSymbol)member, data.FullName);
                        break;
                    case CommonSymbolKind.Property:

                        ParseProperty(data, (IPropertySymbol)member, data.FullName);
                        break;
                }
            }
        }

        private static void ParseEvent(NamedTypeDocumentData parent, IEventSymbol symbol, string rootName)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = CreateDocumentData<EventDocumentData>(symbol, rootName);
            parent.AddEvent(data);
        }

        private static void ParseField(NamedTypeDocumentData parent, IFieldSymbol symbol, string rootName)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol) || !symbol.IsConst)
            {
                return;
            }

            var data = CreateDocumentData<ConstantDocumentData>(symbol, rootName);
            data.Value = symbol.ConstantValue.ToString();
            data.MemberType = CreateDocumentData<DocumentDataObject>(symbol.Type, null);
            parent.AddConstant(data);
        }

        private static void ParseMethod(NamedTypeDocumentData parent, IMethodSymbol symbol, string rootName)
        {
            if (symbol.AssociatedPropertyOrEvent != null)
            {
                // We don't want to include methods that are associated with
                // events or properties.
                return;
            }

            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = CreateDocumentData<MethodDocumentData>(symbol, rootName);

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

            if (symbol.MethodKind == CommonMethodKind.Constructor || symbol.MethodKind == CommonMethodKind.StaticConstructor)
            {
                data.ReturnType = null;
                parent.AddConstructor(data);
            }
            else
            {
                data.ReturnType = CreateDocumentData<DocumentDataObject>(symbol.ReturnType, null);
                parent.AddMethod(data);
            }
        }

        private static void ParseProperty(NamedTypeDocumentData parent, IPropertySymbol symbol, string rootName)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = CreateDocumentData<PropertyDocumentData>(symbol, rootName);
            data.Type = CreateDocumentData<DocumentDataObject>(symbol.Type, null);
            parent.AddProperty(data);
        }

        private static bool IsNotVisibleInGeneratedDocumentation(ISymbol symbol)
        {
            return symbol.DeclaredAccessibility == CommonAccessibility.Private ||
                symbol.DeclaredAccessibility == CommonAccessibility.Internal ||
                symbol.DeclaredAccessibility == CommonAccessibility.ProtectedAndInternal ||
                symbol.DeclaredAccessibility == CommonAccessibility.ProtectedOrInternal;
        }
    }
}
