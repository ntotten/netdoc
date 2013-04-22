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


    public abstract class DocumentDataBase
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Summary { get; set; }
    }

    public class NamespaceDocumentData : DocumentDataBase
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

    public class NamedTypeDocumentData : DocumentDataBase
    {
        private List<MethodDocumentData> methods = new List<MethodDocumentData>();
        private List<PropertyDocumentData> properties = new List<PropertyDocumentData>();

        public void AddMethod(MethodDocumentData data)
        {
            this.methods.Add(data);
        }

        public void AddProperty(PropertyDocumentData data)
        {
            this.properties.Add(data);
        }

        public IEnumerable<MethodDocumentData> Methods
        {
            get
            {
                return this.methods.OrderBy(m => m.Name).ToArray();
            }
        }
        public IEnumerable<PropertyDocumentData> Properties
        {
            get
            {
                return this.properties.OrderBy(m => m.Name).ToArray();
            }
        }

    }

    public class MethodDocumentData : DocumentDataBase
    {

    }

    public class PropertyDocumentData : DocumentDataBase
    {

    }

}
