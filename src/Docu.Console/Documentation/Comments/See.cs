namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    public class See : BaseComment, IReferrer
    {
        public See(IReferencable reference)
        {
            this.Reference = reference;
        }

        public IReferencable Reference { get; set; }
        
        public override bool IsResolved
        {
            get { return this.Reference.IsResolved; }
        }

        public override void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            this.Reference.Resolve(referencables);
        }
    }
}