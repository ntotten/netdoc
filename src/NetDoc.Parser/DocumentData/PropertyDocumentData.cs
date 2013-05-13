namespace NetDoc.Parser.Model
{
    public class PropertyDocumentData : DocumentDataObjectWithId
    {
        public DocumentDataObject Type { get; set; }

        public override void GenerateId()
        {
            this.Id = this.Name;
        }
    }
}
