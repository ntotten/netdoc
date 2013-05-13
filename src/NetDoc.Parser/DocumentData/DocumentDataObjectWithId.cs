namespace NetDoc.Parser.Model
{
    public abstract class DocumentDataObjectWithId : DocumentDataObject
    {
        public string Id { get; set; }

        public abstract void GenerateId();
    }
}
