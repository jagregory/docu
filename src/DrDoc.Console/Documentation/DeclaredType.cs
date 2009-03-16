using System;
using System.Collections.Generic;
using DrDoc.Documentation.Comments;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class DeclaredType : BaseDocumentationElement, IReferencable
    {
        private readonly List<Method> methods = new List<Method>();
        private readonly List<Property> properties = new List<Property>();
        private Type representedType;

        public DeclaredType(TypeIdentifier name, Namespace ns)
            : base(name)
        {
            Namespace = ns;
            Summary = new List<IComment>();
        }

        internal void AddMethod(Method method)
        {
            methods.Add(method);
        }

        internal void AddProperty(Property property)
        {
            properties.Add(property);
        }

        public Namespace Namespace { get; private set; }
        public string PrettyName
        {
            get { return representedType == null ? Name : representedType.GetPrettyName(); }
        }

        public string FullName
        {
            get { return (Namespace == null ? "" : Namespace.FullName + ".") + PrettyName; }
        }

        public IList<Method> Methods
        {
            get { return methods; }
        }

        public IList<Property> Properties
        {
            get { return properties; }
        }

        public IList<IComment> Summary { get; internal set; }
        public IReferencable ParentType { get; set; }

        public void Sort()
        {
            methods.Sort((x, y) => x.Name.CompareTo(y.Name));
            properties.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                var referencable = referencables[identifier];
                var type = referencable as DeclaredType;

                if (type == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                Namespace = type.Namespace;
                representedType = type.representedType;
                ParentType = type.ParentType;

                if (!Namespace.IsResolved)
                    Namespace.Resolve(referencables);
                if (ParentType != null && !ParentType.IsResolved)
                    ParentType.Resolve(referencables);
                
                IsResolved = true;
            }
            else
                ConvertToExternalReference();
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Type type, Namespace ns)
        {
            var declaredType = new DeclaredType(typeIdentifier, ns) { IsResolved = false, representedType = type };

            if (type.BaseType != null)
                declaredType.ParentType = Unresolved(
                    Identifier.FromType(type.BaseType),
                    type.BaseType,
                    Namespace.Unresolved(Identifier.FromNamespace(type.BaseType.Namespace)));

            return declaredType;
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Namespace ns)
        {
            return new DeclaredType(typeIdentifier, ns) { IsResolved = false };
        }
    }
}