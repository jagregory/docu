namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;
    using Parsing.Model;

    public sealed class See : Comment, IReferrer
    {
        public See(IReferencable reference)
        {
            Reference = reference;
        }

        public IReferencable Reference { get; set; }

        public override bool IsResolved
        {
            get { return Reference.IsResolved; }
        }

        public override void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            Reference.Resolve(referencables);
        }
    }
}
