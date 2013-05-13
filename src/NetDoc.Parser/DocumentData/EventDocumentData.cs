namespace NetDoc.Parser.Model
{
    public class EventDocumentData : DocumentDataObjectWithId
    {
        public override void GenerateId()
        {
            this.Id = this.Name;
        }
    }
}
