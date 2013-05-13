namespace NetDoc.Parser.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class NamedTypeDocumentData : DocumentDataObjectWithId
    {
        private IDictionary<string, ConstantDocumentData> constants = new SortedList<string, ConstantDocumentData>();
        private IDictionary<string, MethodDocumentData> constructors = new SortedList<string, MethodDocumentData>();
        private IDictionary<string, PropertyDocumentData> properties = new SortedList<string, PropertyDocumentData>();
        private IDictionary<string, MethodDocumentData> methods = new SortedList<string, MethodDocumentData>();
        private IDictionary<string, EventDocumentData> events = new SortedList<string, EventDocumentData>();

        public IEnumerable<ConstantDocumentData> Constants
        {
            get
            {
                return this.constants.Values.ToArray();
            }
        }

        public IEnumerable<MethodDocumentData> Constructors
        {
            get
            {
                return this.constructors.Values.ToArray();
            }
        }

        public IEnumerable<PropertyDocumentData> Properties
        {
            get
            {
                return this.properties.Values.ToArray();
            }
        }

        public IEnumerable<MethodDocumentData> Methods
        {
            get
            {
                return this.methods.Values.ToArray();
            }
        }

        public IEnumerable<EventDocumentData> Events
        {
            get
            {
                return this.events.Values.ToArray();
            }
        }

        public string TypeKind { get; set; }

        public void AddConstant(ConstantDocumentData data)
        {
            if (data != null)
            {
                if (data.Id == null)
                {
                    data.GenerateId();
                }

                this.constants.Add(data.Id, data);
            }
        }

        public void AddConstructor(MethodDocumentData data)
        {
            if (data != null)
            {
                if (data.Id == null)
                {
                    data.GenerateId();
                }

                this.constructors.Add(data.Id, data);
            }
        }

        public void AddProperty(PropertyDocumentData data)
        {
            if (data != null)
            {
                if (data.Id == null)
                {
                    data.GenerateId();
                }

                this.properties.Add(data.Id, data);
            }
        }

        public void AddMethod(MethodDocumentData data)
        {
            if (data != null)
            {
                if (data.Id == null)
                {
                    data.GenerateId();
                }

                this.methods.Add(data.Id, data);
            }
        }

        public void AddEvent(EventDocumentData data)
        {
            if (data != null)
            {
                if (data.Id == null)
                {
                    data.GenerateId();
                }

                this.events.Add(data.Id, data);
            }
        }

        public override void GenerateId()
        {
            this.Id = this.FullName;
        }

        internal EventDocumentData GetEvent(string id)
        {
            return id != null && this.events.ContainsKey(id) ? this.events[id] : null;
        }

        internal ConstantDocumentData GetConstant(string id)
        {
            return id != null && this.constants.ContainsKey(id) ? this.constants[id] : null;
        }

        internal MethodDocumentData GetConstructor(string id)
        {
            return id != null && this.constructors.ContainsKey(id) ? this.constructors[id] : null;
        }

        internal MethodDocumentData GetMethod(string id)
        {
            return id != null && this.methods.ContainsKey(id) ? this.methods[id] : null;
        }

        internal PropertyDocumentData GetProperty(string id)
        {
            return id != null && this.properties.ContainsKey(id) ? this.properties[id] : null;
        }
    }
}
