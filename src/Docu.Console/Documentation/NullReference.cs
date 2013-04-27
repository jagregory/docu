using System.Collections.Generic;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class NullReference : IReferencable
    {
        public bool IsResolved
        {
            get { return true; }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
        }

        public string Name
        {
            get { return string.Empty; }
        }

        public string FullName
        {
            get { return string.Empty; }
        }

        public string PrettyName
        {
            get { return string.Empty; }
        }

        public bool IsExternal
        {
            get { return true; }
        }

        public bool HasDocumentation
        {
            get { return false; }
        }

        public Summary Summary
        {
            get { return new Summary(); }
            set { }
        }

        public Remarks Remarks
        {
            get { return new Remarks(); }
            set { }
        }

        public Value Value
        {
            get { return new Value(); }
            set { }
        }

        public bool IsIdentifiedBy(Identifier otherIdentifier)
        {
            return false;
        }

        public void ConvertToExternalReference()
        {
        }
    }
}