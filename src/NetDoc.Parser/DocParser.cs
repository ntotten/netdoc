namespace NetDoc.Parser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NetDoc.Parser.Model;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Shared.Utilities;

    public class DocParser
    {
        public DocParser()
        {
            this.Data = new DocumentData();
        }

        public DocumentData Data { get; set; }

        public async Task Parse(Configuration config)
        {
            foreach (var project in config.Projects)
            {
                await this.Parse(project.Path, config.FilteredNamespaces, !string.IsNullOrEmpty(project.Id) ? project.Id : string.Empty);
            }
        }

        public async Task Parse(string projectPath, IEnumerable<string> namespacesBegins)
        {
            await this.Parse(projectPath, namespacesBegins, string.Empty);
        }

        public async Task Parse(string projectPath, IEnumerable<string> namespacesBegins, string id)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var project = await workspace.OpenProjectAsync(projectPath);
                var compilation = await project.GetCompilationAsync();

                this.Parse(compilation, namespacesBegins, id);
            }
        }

        public void Parse(Compilation compilation, IEnumerable<string> namespacesBegins, string id)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException("compilation");
            }

            var globalNamespace = compilation.GlobalNamespace;
            var namespaces = globalNamespace.GetNamespaceMembers();
            foreach (var namespaceSymbol in namespaces)
            {
                if (namespacesBegins == null || namespacesBegins.Count() == 0 || namespacesBegins.Any(n => ValidNamespace(0, namespaceSymbol, n)))
                {
                    ParseNamespace(this.Data, namespaceSymbol, null, namespacesBegins, 1, id);
                }
            }
        }

        private static T CreateDocumentData<T>(ISymbol symbol, string rootName, string id) where T : DocumentDataObject, new()
        {
            var data = new T();
            data.Name = symbol.Name;
            data.DisplayName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            if (!string.IsNullOrEmpty(id))
            {
                data.SupportedProjects.Add(id);
            }

            if (rootName != null)
            {
                data.DisplayName = data.DisplayName.Replace(rootName, string.Empty).Substring(1);
            }

            data.AccessModifier = Helper.RetrieveAccessModifier(symbol.DeclaredAccessibility);
            data.FullName = GetFullName(symbol, rootName);

            DocumentationComment comment = null;
            try
            {
                var commentXml = symbol.GetDocumentationCommentXml();
                comment = DocumentationComment.FromXmlFragment(commentXml);
            }
            catch
            {
            }

            if (comment != null)
            {
                // TODO: Parse XML
                data.Summary = comment.SummaryText;
                data.ReturnDescription = comment.ReturnsText;
            }

            return data;
        }

        private static string GetFullName(ISymbol symbol, string rootName)
        {
            return string.IsNullOrEmpty(rootName) ? symbol.Name : rootName + "." + symbol.Name;
        }

        private static void ParseNamespace(DocumentData parent, INamespaceSymbol symbol, string rootName, IEnumerable<string> namespacesBegins, int namespaceIndex, string id)
        {
            var data = parent.GetNamespace(symbol);
            if (data == null)
            {
                data = CreateDocumentData<NamespaceDocumentData>(symbol, rootName, id);
                parent.AddNamespace(data);
            }
            else
            {
                data.SupportedProjects.Add(id);
            }

            // Namespaces
            var namespaces = symbol.GetNamespaceMembers();
            foreach (var namespaceSymbol in namespaces)
            {
                if (namespacesBegins == null || namespacesBegins.Count() == 0 || namespacesBegins.Any(n => ValidNamespace(namespaceIndex, namespaceSymbol, n)))
                {
                    ParseNamespace(parent, namespaceSymbol, data.Name, namespacesBegins, namespaceIndex + 1, id);
                }
            }

            if (namespacesBegins == null || namespacesBegins.Count() == 0 || namespacesBegins.Any(n => ValidNamespace(namespaceIndex, symbol, n)))
            {
                // Types
                var typeMembers = symbol.GetTypeMembers();
                foreach (var typeMember in typeMembers)
                {
                    ParseTypeMember(data, typeMember, data.FullName, id);
                }
            }
        }

        private static bool ValidNamespace(int namespaceIndex, INamespaceSymbol namespaceSymbol, string filterNamespace)
        {
            var splittedNamespace = filterNamespace.Split('.');

            if (splittedNamespace.Length > namespaceIndex)
            {
                return namespaceSymbol.Name.StartsWith(splittedNamespace[namespaceIndex], StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return true;
            }
        }

        private static void ParseTypeMember(NamespaceDocumentData parent, INamedTypeSymbol symbol, string rootName, string id)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = parent.GetTypeMember(GetFullName(symbol, rootName));
            if (data == null)
            {
                data = CreateDocumentData<NamedTypeDocumentData>(symbol, rootName, id);
                parent.AddNamedType(data);
            }
            else
            {
                data.SupportedProjects.Add(id);
            }

            data.TypeKind = symbol.TypeKind.ToString();

            // Process Members
            var members = symbol.GetMembers();
            foreach (var member in members)
            {
                switch (member.Kind)
                {
                    case SymbolKind.Field:

                        ParseField(data, (IFieldSymbol)member, data.FullName, id);
                        break;
                    case SymbolKind.Event:

                        ParseEvent(data, (IEventSymbol)member, data.FullName, id);
                        break;
                    case SymbolKind.Method:

                        ParseMethod(data, (IMethodSymbol)member, data.FullName, id);
                        break;
                    case SymbolKind.Property:

                        ParseProperty(data, (IPropertySymbol)member, data.FullName, id);
                        break;
                }
            }
        }

        private static void ParseEvent(NamedTypeDocumentData parent, IEventSymbol symbol, string rootName, string id)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = parent.GetEvent(symbol.Name);
            if (data == null)
            {
                data = CreateDocumentData<EventDocumentData>(symbol, rootName, id);
                parent.AddEvent(data);
            }
            else
            {
                data.SupportedProjects.Add(id);
            }
        }

        private static void ParseField(NamedTypeDocumentData parent, IFieldSymbol symbol, string rootName, string id)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol) || !symbol.IsConst)
            {
                return;
            }

            var data = parent.GetConstant(symbol.Name);
            if (data == null)
            {
                data = CreateDocumentData<ConstantDocumentData>(symbol, rootName, id);
                data.Value = symbol.ConstantValue.ToString();
                data.MemberType = CreateDocumentData<DocumentDataObject>(symbol.Type, null, string.Empty);
                parent.AddConstant(data);
            }
            else
            {
                data.SupportedProjects.Add(id);
            }
        }

        private static void ParseMethod(NamedTypeDocumentData parent, IMethodSymbol symbol, string rootName, string id)
        {
            if (symbol.AssociatedSymbol != null)
            {
                // We don't want to include methods that are associated with
                // events or properties.
                return;
            }

            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = CreateDocumentData<MethodDocumentData>(symbol, rootName, id);

            DocumentationComment comment = null;
            try
            {
                var commentXml = symbol.GetDocumentationCommentXml();
                comment = DocumentationComment.FromXmlFragment(commentXml);
            }
            catch
            {
            }

            var parameters = symbol.Parameters;
            foreach (var parameter in parameters)
            {
                var parameterData = CreateDocumentData<MethodParameterData>(parameter, null, string.Empty);
                parameterData.Summary = comment.GetParameterText(parameterData.Name) ?? string.Empty;
                parameterData.Type = CreateDocumentData<DocumentDataObject>(parameter.Type, null, string.Empty);
                data.Parameters.Add(parameterData);
            }

            var typeArguments = symbol.TypeArguments;
            foreach (var typeArgument in typeArguments)
            {
                var typeArgumentData = CreateDocumentData<MethodTypeArgumentData>(typeArgument, null, string.Empty);
                data.TypeArguments.Add(typeArgumentData);
            }

            data.GenerateId();
            if (symbol.MethodKind == MethodKind.Constructor || symbol.MethodKind == MethodKind.StaticConstructor)
            {
                var existingConstructor = parent.GetConstructor(data.Id);
                if (existingConstructor == null)
                {
                    data.ReturnType = null;
                    parent.AddConstructor(data);
                }
                else
                {
                    existingConstructor.SupportedProjects.Add(id);
                }
            }
            else
            {
                var existingMethod = parent.GetMethod(data.Id);
                if (existingMethod == null)
                {
                    data.ReturnType = CreateDocumentData<DocumentDataObject>(symbol.ReturnType, null, string.Empty);
                    parent.AddMethod(data);
                }
                else
                {
                    existingMethod.SupportedProjects.Add(id);
                }
            }
        }

        private static void ParseProperty(NamedTypeDocumentData parent, IPropertySymbol symbol, string rootName, string id)
        {
            if (IsNotVisibleInGeneratedDocumentation(symbol))
            {
                return;
            }

            var data = parent.GetProperty(symbol.Name);
            if (data == null)
            {
                data = CreateDocumentData<PropertyDocumentData>(symbol, rootName, id);
                data.Type = CreateDocumentData<DocumentDataObject>(symbol.Type, null, string.Empty);
                parent.AddProperty(data);
            }
            else
            {
                data.SupportedProjects.Add(id);
            }
        }

        private static bool IsNotVisibleInGeneratedDocumentation(ISymbol symbol)
        {
            return symbol.DeclaredAccessibility == Accessibility.Private ||
                symbol.DeclaredAccessibility == Accessibility.Internal ||
                symbol.DeclaredAccessibility == Accessibility.ProtectedAndInternal ||
                symbol.DeclaredAccessibility == Accessibility.ProtectedOrInternal;
        }
    }
}
