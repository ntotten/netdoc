namespace NetDoc.Parser.Model
{
    public class ConstantDocumentData : DocumentDataObjectWithId
    {
        public string Value { get; set; }

        public DocumentDataObject MemberType { get; set; }

        public override void GenerateId()
        {
            this.Id = this.Name;
        }
    }
}
