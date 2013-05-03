namespace NetDoc.Parser.DocumentData
{
    public class PropertyDocumentData : IdentificableDocumentDataObject
    {
        public DocumentDataObject Type { get; set; }

        public override void GenerateId()
        {
            this.Id = this.Name;
        }
    }
}
