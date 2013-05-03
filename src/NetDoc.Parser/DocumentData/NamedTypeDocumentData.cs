﻿namespace NetDoc.Parser.DocumentData
{
    using System.Collections.Generic;
    using System.Linq;

    public class NamedTypeDocumentData : DocumentDataObject
    {
        private List<ConstantDocumentData> constants = new List<ConstantDocumentData>();
        private List<MethodDocumentData> constructors = new List<MethodDocumentData>();
        private List<PropertyDocumentData> properties = new List<PropertyDocumentData>();
        private List<MethodDocumentData> methods = new List<MethodDocumentData>();
        private List<EventDocumentData> events = new List<EventDocumentData>();

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

        public IEnumerable<EventDocumentData> Events
        {
            get
            {
                return this.events.OrderBy(m => m.Name).ToArray();
            }
        }

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

        public void AddEvent(EventDocumentData data)
        {
            this.events.Add(data);
        }
    }
}
