namespace NetDoc.Parser.DocumentData
{
    using System.Collections.Generic;

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
}
