namespace NetDoc.Parser.DocumentData
{
    public class EventDocumentData : IdentificableDocumentDataObject
    {
        public override void GenerateId()
        {
            this.Id = this.Name;
        }
    }
}
