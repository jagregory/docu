using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Property : BaseDocumentationElement, IReferencable
    {
        public Property(PropertyIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            Type = type;
            HasGet = identifier.HasGet;
            HasSet = identifier.HasSet;
        }

        public DeclaredType Type { get; set; }
        public bool HasGet { get; private set; }
        public bool HasSet { get; private set; }
        public List<Attribute> Attributes { get; set; }

        public IReferencable ReturnType { get; set; }

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
                var property = referencable as Property;

                if (property == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                ReturnType = property.ReturnType;
                Attributes = property.Attributes;

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

        public static Property Unresolved(PropertyIdentifier propertyIdentifier, DeclaredType type)
        {
            return new Property(propertyIdentifier, type) { IsResolved = false };
        }

        public static Property Unresolved(PropertyIdentifier propertyIdentifier, DeclaredType type, IReferencable returnType, List<Attribute> attributes)
        {
            return new Property(propertyIdentifier, type) { IsResolved = false, ReturnType = returnType, Attributes = attributes };
        }
    }
}