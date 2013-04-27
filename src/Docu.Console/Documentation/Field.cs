using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Field : BaseDocumentationElement, IReferencable
    {
        public Field(FieldIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            Type = type;
        }

        public IReferencable ReturnType { get; set; }

        public DeclaredType Type { get; set; }

        public string FullName
        {
            get { return Name; }
        }

        public string PrettyName
        {
            get { return Name; }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                IsResolved = true;
                IReferencable referencable = referencables[identifier];
                var field = referencable as Field;

                if (field == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                ReturnType = field.ReturnType;

                if (!ReturnType.IsResolved)
                {
                    ReturnType.Resolve(referencables);
                }

                if (!Summary.IsResolved)
                {
                    Summary.Resolve(referencables);
                }

                if (!Remarks.IsResolved)
                {
                    Remarks.Resolve(referencables);
                }
            }
            else
            {
                ConvertToExternalReference();
            }
        }

        public static Field Unresolved(FieldIdentifier fieldIdentifier, DeclaredType type)
        {
            return new Field(fieldIdentifier, type) {IsResolved = false};
        }

        public static Field Unresolved(FieldIdentifier fieldIdentifier, DeclaredType type, IReferencable returnType)
        {
            return new Field(fieldIdentifier, type) {IsResolved = false, ReturnType = returnType};
        }
    }
}
