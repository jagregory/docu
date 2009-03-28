using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class DeclaredType : BaseDocumentationElement, IReferencable
    {
        private readonly List<Method> methods = new List<Method>();
        private readonly List<Property> properties = new List<Property>();
        private readonly List<Event> events = new List<Event>();
        private readonly List<Field> fields = new List<Field>();
        private Type representedType;

        public DeclaredType(TypeIdentifier name, Namespace ns)
            : base(name)
        {
            Namespace = ns;
            Interfaces = new List<IReferencable>();
        }

        public Namespace Namespace { get; private set; }

        public IList<Method> Methods
        {
            get { return methods; }
        }

        public IList<Property> Properties
        {
            get { return properties; }
        }

        public IReferencable ParentType { get; set; }
        public IList<IReferencable> Interfaces { get; set; }

        public string PrettyName
        {
            get { return representedType == null ? Name : representedType.GetPrettyName(); }
        }

        public string FullName
        {
            get { return (Namespace == null ? "" : Namespace.FullName + ".") + PrettyName; }
        }

        public IList<Event> Events
        {
            get { return events; }
        }

        public IList<Field> Fields
        {
            get { return fields; }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                IsResolved = true;

                IReferencable referencable = referencables[identifier];
                var type = referencable as DeclaredType;

                if (type == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                Namespace = type.Namespace;
                representedType = type.representedType;
                ParentType = type.ParentType;
                Interfaces = type.Interfaces;

                if (!Namespace.IsResolved)
                    Namespace.Resolve(referencables);
                if (ParentType != null && !ParentType.IsResolved)
                    ParentType.Resolve(referencables);

                foreach (IReferencable face in Interfaces)
                {
                    if (!face.IsResolved)
                        face.Resolve(referencables);
                }

                if (!Summary.IsResolved)
                    Summary.Resolve(referencables);

                foreach (var method in Methods)
                {
                    if (!method.IsResolved)
                        method.Resolve(referencables);
                }

                foreach (var property in Properties)
                {
                    if (!property.IsResolved)
                        property.Resolve(referencables);
                }

                foreach (var ev in Events)
                {
                    if (!ev.IsResolved)
                        ev.Resolve(referencables);
                }

                foreach (var field in Fields)
                {
                    if (!field.IsResolved)
                        field.Resolve(referencables);
                }
            }
            else
                ConvertToExternalReference();
        }

        internal void AddMethod(Method method)
        {
            methods.Add(method);
        }

        internal void AddProperty(Property property)
        {
            properties.Add(property);
        }

        public void AddEvent(Event ev)
        {
            events.Add(ev);
        }

        public void AddField(Field field)
        {
            fields.Add(field);
        }

        public void Sort()
        {
            methods.Sort((x, y) => x.Name.CompareTo(y.Name));
            properties.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        public override string ToString()
        {
            return base.ToString() + " { Name = '" + Name + "'}";
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Type type, Namespace ns)
        {
            var declaredType = new DeclaredType(typeIdentifier, ns) { IsResolved = false, representedType = type };

            if (type.BaseType != null)
                declaredType.ParentType = Unresolved(
                    Identifier.FromType(type.BaseType),
                    type.BaseType,
                    Namespace.Unresolved(Identifier.FromNamespace(type.BaseType.Namespace)));

            IEnumerable<Type> interfaces = GetInterfaces(type);

            foreach (Type face in interfaces)
            {
                declaredType.Interfaces.Add(
                    Unresolved(
                        Identifier.FromType(face),
                        face,
                        Namespace.Unresolved(Identifier.FromNamespace(face.Namespace))));
            }

            return declaredType;
        }

        private static IEnumerable<Type> GetInterfaces(Type type)
        {
            var interfaces = new List<Type>(type.GetInterfaces());

            foreach (Type face in type.GetInterfaces())
            {
                FilterInterfaces(interfaces, face);
            }

            if (type.BaseType != null)
                FilterInterfaces(interfaces, type.BaseType);

            return interfaces;
        }

        private static void FilterInterfaces(IList<Type> topLevelInterfaces, Type type)
        {
            foreach (Type face in type.GetInterfaces())
            {
                if (topLevelInterfaces.Contains(face))
                {
                    topLevelInterfaces.Remove(face);
                    continue;
                }

                FilterInterfaces(topLevelInterfaces, face);
            }

            if (type.BaseType != null)
                FilterInterfaces(topLevelInterfaces, type.BaseType);
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Namespace ns)
        {
            return new DeclaredType(typeIdentifier, ns) { IsResolved = false };
        }
    }
}