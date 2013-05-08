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
                this.Parse(project["path"], namespacesBegins, project["id"]);
            }
        }

        public void Parse(string projectPath, IEnumerable<string> namespacesBegins)
        {
            this.Parse(projectPath, namespacesBegins, string.Empty);
        }

        public void Parse(string projectPath, IEnumerable<string> namespacesBegins, string id)
        {
            var workspace = Roslyn.Services.Workspace.LoadStandAloneProject(projectPath);
            var compilation = workspace.CurrentSolution.Projects.First().GetCompilation();

            this.Parse(compilation, namespacesBegins, id);
        }

        public void Parse(CommonCompilation compilation, IEnumerable<string> namespacesBegins, string id)
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
                    ParseNamespace(this.Data, namespaceSymbol, null, id);
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

        private static string GetFullName(ISymbol symbol, string rootName)
        {
            return string.IsNullOrEmpty(rootName) ? symbol.Name : rootName + "." + symbol.Name;
        }

        private static void ParseNamespace(DocumentData parent, INamespaceSymbol symbol, string rootName, string id)
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
                ParseNamespace(parent, namespaceSymbol, data.Name, id);
            }

            // Types
            var typeMembers = symbol.GetTypeMembers();
            foreach (var typeMember in typeMembers)
            {
                ParseTypeMember(data, typeMember, data.FullName, id);
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
                    case CommonSymbolKind.Field:

                        ParseField(data, (IFieldSymbol)member, data.FullName, id);
                        break;
                    case CommonSymbolKind.Event:

                        ParseEvent(data, (IEventSymbol)member, data.FullName, id);
                        break;
                    case CommonSymbolKind.Method:

                        ParseMethod(data, (IMethodSymbol)member, data.FullName, id);
                        break;
                    case CommonSymbolKind.Property:

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

            var data = CreateDocumentData<MethodDocumentData>(symbol, rootName, id);

            DocumentationComment comment = null;
            try
            {
                comment = symbol.GetDocumentationComment();
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
            if (symbol.MethodKind == CommonMethodKind.Constructor || symbol.MethodKind == CommonMethodKind.StaticConstructor)
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
            return symbol.DeclaredAccessibility == CommonAccessibility.Private ||
                symbol.DeclaredAccessibility == CommonAccessibility.Internal ||
                symbol.DeclaredAccessibility == CommonAccessibility.ProtectedAndInternal ||
                symbol.DeclaredAccessibility == CommonAccessibility.ProtectedOrInternal;
        }
    }
}
