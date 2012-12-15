namespace Docu.Documentation
{
    using System;
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    public class Field : BaseDocumentationElement, IReferencable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public Field(FieldIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            this.Type = type;
        }

        public string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public string PrettyName
        {
            get
            {
                return this.Name;
            }
        }

        public IReferencable ReturnType { get; set; }

        public DeclaredType Type { get; set; }

        public static Field Unresolved(FieldIdentifier fieldIdentifier, DeclaredType type)
        {
            return new Field(fieldIdentifier, type) { IsResolved = false };
        }

        public static Field Unresolved(FieldIdentifier fieldIdentifier, DeclaredType type, IReferencable returnType)
        {
            return new Field(fieldIdentifier, type) { IsResolved = false, ReturnType = returnType };
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(this.identifier))
            {
                this.IsResolved = true;
                IReferencable referencable = referencables[this.identifier];
                var field = referencable as Field;

                if (field == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                this.ReturnType = field.ReturnType;

                if (!this.ReturnType.IsResolved)
                {
                    this.ReturnType.Resolve(referencables);
                }

                if (!this.Summary.IsResolved)
                {
                    this.Summary.Resolve(referencables);
                }

                if (!this.Remarks.IsResolved)
                {
                    this.Remarks.Resolve(referencables);
                }
            }
            else
            {
                this.ConvertToExternalReference();
            }
        }
    }
}