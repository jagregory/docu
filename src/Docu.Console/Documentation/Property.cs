namespace Docu.Documentation
{
    using System;
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    public class Property : BaseDocumentationElement, IReferencable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public Property(PropertyIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            this.Type = type;
            this.HasGet = identifier.HasGet;
            this.HasSet = identifier.HasSet;
        }

        public string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public bool HasGet { get; private set; }

        public bool HasSet { get; private set; }

        public string PrettyName
        {
            get
            {
                return this.Name;
            }
        }

        public IReferencable ReturnType { get; set; }

        public DeclaredType Type { get; set; }

        public static Property Unresolved(PropertyIdentifier propertyIdentifier, DeclaredType type)
        {
            return new Property(propertyIdentifier, type) { IsResolved = false };
        }

        public static Property Unresolved(
            PropertyIdentifier propertyIdentifier, DeclaredType type, IReferencable returnType)
        {
            return new Property(propertyIdentifier, type) { IsResolved = false, ReturnType = returnType };
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(this.identifier))
            {
                this.IsResolved = true;
                IReferencable referencable = referencables[this.identifier];
                var property = referencable as Property;

                if (property == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                this.ReturnType = property.ReturnType;

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