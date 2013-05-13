namespace NetDoc.Parser.Model
{
    public class MethodParameterData : DocumentDataObject
    {
        public DocumentDataObject Type { get; set; }

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }
    }
}
