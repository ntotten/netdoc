using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDoc
{
    public class DocumentData
    {
        private SortedList<string, NamespaceDocumentData> namespaces = new SortedList<string, NamespaceDocumentData>();

        public void AddNamespace(NamespaceDocumentData data)
        {
            var key = data.Name ?? string.Empty;
            this.namespaces.Add(key, data);
        }

        public IEnumerable<NamespaceDocumentData> Namespaces
        {
            get
            {
                return this.namespaces.Values;
            }
        }
    }


    public class DocumentDataObject
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Summary { get; set; }
        public string ReturnDescription { get; set; }
    }

    public class NamespaceDocumentData : DocumentDataObject
    {

        private List<NamedTypeDocumentData> namedTypes = new List<NamedTypeDocumentData>();

        public void AddNamedType(NamedTypeDocumentData data)
        {
            this.namedTypes.Add(data);
        }

        public IEnumerable<NamedTypeDocumentData> NamedTypes
        {
            get
            {
                return this.namedTypes.OrderBy(m => m.Name).ToArray();
            }
        }
    }

    public class NamedTypeDocumentData : DocumentDataObject
    {
        private List<ConstantDocumentData> constants = new List<ConstantDocumentData>();
        private List<MethodDocumentData> constructors = new List<MethodDocumentData>();
        private List<PropertyDocumentData> properties = new List<PropertyDocumentData>();
        private List<MethodDocumentData> methods = new List<MethodDocumentData>();

        public void AddConstant(ConstantDocumentData data)
        {
            this.constants.Add(data);
        }

        public void AddConstructor(MethodDocumentData data)
        {
            this.constructors.Add(data);
        }

        public void AddProperty(PropertyDocumentData data)
        {
            this.properties.Add(data);
        }

        public void AddMethod(MethodDocumentData data)
        {
            this.methods.Add(data);
        }

        public IEnumerable<ConstantDocumentData> Constants
        {
            get
            {
                return this.constants.OrderBy(m => m.Name).ToArray();
            }
        }

        public IEnumerable<MethodDocumentData> Constructors
        {
            get
            {
                return this.constructors.OrderBy(m => m.Name).ToArray();
            }
        }

        public IEnumerable<PropertyDocumentData> Properties
        {
            get
            {
                return this.properties.OrderBy(m => m.Name).ToArray();
            }
        }

        public IEnumerable<MethodDocumentData> Methods
        {
            get
            {
                return this.methods.OrderBy(m => m.Name).ToArray();
            }
        }

    }

    public class ConstantDocumentData : DocumentDataObject
    {
        public string Value { get; set; }
    }

    public class PropertyDocumentData : DocumentDataObject
    {
        public DocumentDataObject Type { get; set; }
    }

    public class MethodDocumentData : DocumentDataObject
    {
        public MethodDocumentData()
        {
            this.Parameters = new List<MethodParameterData>();
            this.TypeArguments = new List<MethodTypeArgumentData>();
        }

        public List<MethodParameterData> Parameters { get; private set; }
        public List<MethodTypeArgumentData> TypeArguments { get; private set; }

        public DocumentDataObject ReturnType { get; set; }
    }

    public class MethodParameterData : DocumentDataObject
    {
        public DocumentDataObject Type { get; set; }
        public bool IsOut { get; set; }
        public bool IsRef { get; set; }
    }

    public class MethodTypeArgumentData : DocumentDataObject
    {
        
    }

}
