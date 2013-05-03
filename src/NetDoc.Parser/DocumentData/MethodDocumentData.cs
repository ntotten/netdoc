﻿namespace NetDoc.Parser.DocumentData
{
    using System.Collections.Generic;

    public class MethodDocumentData : IdentificableDocumentDataObject
    {
        public MethodDocumentData()
        {
            this.Parameters = new List<MethodParameterData>();
            this.TypeArguments = new List<MethodTypeArgumentData>();
        }

        public List<MethodParameterData> Parameters { get; private set; }

        public List<MethodTypeArgumentData> TypeArguments { get; private set; }

        public DocumentDataObject ReturnType { get; set; }

        public override void GenerateId()
        {
            this.Id = this.Name;

            if (this.TypeArguments.Count > 0)
            {
                this.Id += "_";
            }

            foreach (var arg in this.TypeArguments)
            {
                this.Id += string.Format("{0}_", arg.Name);
            }

            this.Id += "(";

            if (this.Parameters.Count > 0)
            {
                var types = new List<string>();

                foreach (var param in this.Parameters)
                {
                    types.Add(param.Type.DisplayName);
                }

                this.Id += string.Join(",", types);
            }

            this.Id += ")";
        }
    }
}
