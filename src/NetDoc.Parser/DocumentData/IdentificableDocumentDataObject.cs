namespace NetDoc.Parser.DocumentData
{
    public abstract class IdentificableDocumentDataObject : DocumentDataObject
    {
        public string Id { get; set; }

        public abstract void GenerateId();
    }
}
