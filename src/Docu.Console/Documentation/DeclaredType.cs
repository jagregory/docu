using System;
using System.Collections.Generic;
using System.Reflection;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class DeclaredType : BaseDocumentationElement, IReferencable
    {
        readonly List<Event> events = new List<Event>();

        readonly List<Field> fields = new List<Field>();

        readonly List<Method> methods = new List<Method>();

        readonly List<Property> properties = new List<Property>();

        Type declaration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeclaredType" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="ns">
        ///     The ns.
        /// </param>
        public DeclaredType(TypeIdentifier name, Namespace ns)
            : base(name)
        {
            Namespace = ns;
            Interfaces = new List<IReferencable>();
        }

        public IList<Event> Events
        {
            get { return events; }
        }

        public IList<Field> Fields
        {
            get { return fields; }
        }

        public string FullName
        {
            get { return (Namespace == null ? string.Empty : Namespace.FullName + ".") + PrettyName; }
        }

        public IList<IReferencable> Interfaces { get; set; }

        public bool IsInterface
        {
            get { return declaration != null && declaration.IsInterface; }
        }

        public IList<Method> Methods
        {
            get { return methods; }
        }

        public Namespace Namespace { get; private set; }

        public IReferencable ParentType { get; set; }

        public string PrettyName
        {
            get { return declaration == null ? Name : declaration.GetPrettyName(); }
        }

        public IList<Property> Properties
        {
            get { return properties; }
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Type declaration, Namespace ns)
        {
            var declaredType = new DeclaredType(typeIdentifier, ns) {IsResolved = false, declaration = declaration};

            if (declaration.BaseType != null)
            {
                declaredType.ParentType = Unresolved(
                    IdentifierFor.Type(declaration.BaseType),
                    declaration.BaseType,
                    Namespace.Unresolved(IdentifierFor.Namespace(declaration.BaseType.Namespace)));
            }

            IEnumerable<Type> interfaces = GetInterfaces(declaration);

            foreach (Type face in interfaces)
            {
                declaredType.Interfaces.Add(
                    Unresolved(IdentifierFor.Type(face), face, Namespace.Unresolved(IdentifierFor.Namespace(face.Namespace))));
            }

            return declaredType;
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Namespace ns)
        {
            return new DeclaredType(typeIdentifier, ns) {IsResolved = false};
        }

        public void AddEvent(Event ev)
        {
            events.Add(ev);
        }

        public void AddField(Field field)
        {
            fields.Add(field);
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                IsResolved = true;

                IReferencable referencable = referencables[identifier];
                var type = referencable as DeclaredType;

                if (type == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                Namespace = type.Namespace;
                declaration = type.declaration;
                ParentType = type.ParentType;
                Interfaces = type.Interfaces;

                if (!Namespace.IsResolved)
                {
                    Namespace.Resolve(referencables);
                }

                if (ParentType != null && !ParentType.IsResolved)
                {
                    ParentType.Resolve(referencables);
                }

                foreach (IReferencable face in Interfaces)
                {
                    if (!face.IsResolved)
                    {
                        face.Resolve(referencables);
                    }
                }

                if (declaration != null && declaration.IsDefined(typeof(ObsoleteAttribute)))
                {
                    ObsoleteReason = declaration.GetCustomAttribute<ObsoleteAttribute>().Message;
                }

                if (!Summary.IsResolved)
                {
                    Summary.Resolve(referencables);
                }

                foreach (Method method in Methods)
                {
                    if (!method.IsResolved)
                    {
                        method.Resolve(referencables);
                    }
                }

                foreach (Property property in Properties)
                {
                    if (!property.IsResolved)
                    {
                        property.Resolve(referencables);
                    }
                }

                foreach (Event ev in Events)
                {
                    if (!ev.IsResolved)
                    {
                        ev.Resolve(referencables);
                    }
                }

                foreach (Field field in Fields)
                {
                    if (!field.IsResolved)
                    {
                        field.Resolve(referencables);
                    }
                }
            }
            else
            {
                ConvertToExternalReference();
            }
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

        internal void AddMethod(Method method)
        {
            methods.Add(method);
        }

        internal void AddProperty(Property property)
        {
            properties.Add(property);
        }

        static void FilterInterfaces(IList<Type> topLevelInterfaces, Type type)
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
            {
                FilterInterfaces(topLevelInterfaces, type.BaseType);
            }
        }

        static IEnumerable<Type> GetInterfaces(Type type)
        {
            var interfaces = new List<Type>(type.GetInterfaces());

            foreach (Type face in type.GetInterfaces())
            {
                FilterInterfaces(interfaces, face);
            }

            if (type.BaseType != null)
            {
                FilterInterfaces(interfaces, type.BaseType);
            }

            return interfaces;
        }
    }
}
