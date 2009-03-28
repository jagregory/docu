using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Field : BaseDocumentationElement, IReferencable
    {
        public Field(FieldIdentifier identifier)
            : base(identifier)
        {}

        public string FullName
        {
            get { return Name; }
        }

        public string PrettyName
        {
            get { return Name; }
        }

        public IReferencable ReturnType { get; set; }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                IsResolved = true;
                var referencable = referencables[identifier];
                var field = referencable as Field;

                if (field == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                ReturnType = field.ReturnType;

                if (!ReturnType.IsResolved)
                    ReturnType.Resolve(referencables);

                if (!Summary.IsResolved)
                    Summary.Resolve(referencables);

                if (!Remarks.IsResolved)
                    Remarks.Resolve(referencables);
            }
            else
                ConvertToExternalReference();
        }

        public static Field Unresolved(FieldIdentifier fieldIdentifier)
        {
            return new Field(fieldIdentifier) { IsResolved = false };
        }

        public static Field Unresolved(FieldIdentifier fieldIdentifier, IReferencable returnType)
        {
            return new Field(fieldIdentifier) { IsResolved = false, ReturnType = returnType };
        }
    }
}