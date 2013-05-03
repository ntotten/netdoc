namespace NetDoc.Parser.DocumentData
{
    public class ConstantDocumentData : IdentificableDocumentDataObject
    {
        public string Value { get; set; }

        public override void GenerateId()
        {
            this.Id = this.Name;
        }
    }
}
