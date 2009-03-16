using System;
using System.Collections.Generic;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Property : BaseDocumentationElement, IReferencable
    {
        public Property(PropertyIdentifier identifier)
            : base(identifier)
        {
            Summary = new List<IComment>();
            HasGet = identifier.HasGet;
            HasSet = identifier.HasSet;
        }

        public bool HasGet { get; private set; }
        public bool HasSet { get; private set; }

        public IList<IComment> Summary { get; internal set; }
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
                IReferencable referencable = referencables[identifier];
                var property = referencable as Property;

                if (property == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                ReturnType = property.ReturnType;
                IsResolved = true;
            }

            ConvertToExternalReference();
        }

        public static Property Unresolved(PropertyIdentifier propertyIdentifier)
        {
            return new Property(propertyIdentifier) { IsResolved = false };
        }

        public static Property Unresolved(PropertyIdentifier propertyIdentifier, IReferencable returnType)
        {
            return new Property(propertyIdentifier) { IsResolved = false, ReturnType = returnType };
        }
    }
}