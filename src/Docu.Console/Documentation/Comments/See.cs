using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation.Comments
{
    public class See : BaseComment, IReferrer
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