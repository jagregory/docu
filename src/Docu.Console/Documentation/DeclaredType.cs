namespace Docu.Documentation
{
    using System;
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    public class DeclaredType : BaseDocumentationElement, IReferencable
    {
        private readonly List<Event> events = new List<Event>();

        private readonly List<Field> fields = new List<Field>();

        private readonly List<Method> methods = new List<Method>();

        private readonly List<Property> properties = new List<Property>();

        private Type representedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeclaredType"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="ns">
        /// The ns.
        /// </param>
        public DeclaredType(TypeIdentifier name, Namespace ns)
            : base(name)
        {
            this.Namespace = ns;
            this.Interfaces = new List<IReferencable>();
        }

        public IList<Event> Events
        {
            get
            {
                return this.events;
            }
        }

        public IList<Field> Fields
        {
            get
            {
                return this.fields;
            }
        }

        public string FullName
        {
            get
            {
                return (this.Namespace == null ? string.Empty : this.Namespace.FullName + ".") + this.PrettyName;
            }
        }

        public IList<IReferencable> Interfaces { get; set; }

        public bool IsInterface
        {
            get
            {
                return this.representedType != null && this.representedType.IsInterface;
            }
        }

        public IList<Method> Methods
        {
            get
            {
                return this.methods;
            }
        }

        public Namespace Namespace { get; private set; }

        public IReferencable ParentType { get; set; }

        public string PrettyName
        {
            get
            {
                return this.representedType == null ? this.Name : this.representedType.GetPrettyName();
            }
        }

        public IList<Property> Properties
        {
            get
            {
                return this.properties;
            }
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Type type, Namespace ns)
        {
            var declaredType = new DeclaredType(typeIdentifier, ns) { IsResolved = false, representedType = type };

            if (type.BaseType != null)
            {
                declaredType.ParentType = Unresolved(
                    Identifier.FromType(type.BaseType), 
                    type.BaseType, 
                    Namespace.Unresolved(Identifier.FromNamespace(type.BaseType.Namespace)));
            }

            IEnumerable<Type> interfaces = GetInterfaces(type);

            foreach (Type face in interfaces)
            {
                declaredType.Interfaces.Add(
                    Unresolved(
                        Identifier.FromType(face), face, Namespace.Unresolved(Identifier.FromNamespace(face.Namespace))));
            }

            return declaredType;
        }

        public static DeclaredType Unresolved(TypeIdentifier typeIdentifier, Namespace ns)
        {
            return new DeclaredType(typeIdentifier, ns) { IsResolved = false };
        }

        public void AddEvent(Event ev)
        {
            this.events.Add(ev);
        }

        public void AddField(Field field)
        {
            this.fields.Add(field);
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(this.identifier))
            {
                this.IsResolved = true;

                IReferencable referencable = referencables[this.identifier];
                var type = referencable as DeclaredType;

                if (type == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                this.Namespace = type.Namespace;
                this.representedType = type.representedType;
                this.ParentType = type.ParentType;
                this.Interfaces = type.Interfaces;

                if (!this.Namespace.IsResolved)
                {
                    this.Namespace.Resolve(referencables);
                }

                if (this.ParentType != null && !this.ParentType.IsResolved)
                {
                    this.ParentType.Resolve(referencables);
                }

                foreach (IReferencable face in this.Interfaces)
                {
                    if (!face.IsResolved)
                    {
                        face.Resolve(referencables);
                    }
                }

                if (!this.Summary.IsResolved)
                {
                    this.Summary.Resolve(referencables);
                }

                foreach (Method method in this.Methods)
                {
                    if (!method.IsResolved)
                    {
                        method.Resolve(referencables);
                    }
                }

                foreach (Property property in this.Properties)
                {
                    if (!property.IsResolved)
                    {
                        property.Resolve(referencables);
                    }
                }

                foreach (Event ev in this.Events)
                {
                    if (!ev.IsResolved)
                    {
                        ev.Resolve(referencables);
                    }
                }

                foreach (Field field in this.Fields)
                {
                    if (!field.IsResolved)
                    {
                        field.Resolve(referencables);
                    }
                }
            }
            else
            {
                this.ConvertToExternalReference();
            }
        }

        public void Sort()
        {
            this.methods.Sort((x, y) => x.Name.CompareTo(y.Name));
            this.properties.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        public override string ToString()
        {
            return base.ToString() + " { Name = '" + this.Name + "'}";
        }

        internal void AddMethod(Method method)
        {
            this.methods.Add(method);
        }

        internal void AddProperty(Property property)
        {
            this.properties.Add(property);
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
            {
                FilterInterfaces(topLevelInterfaces, type.BaseType);
            }
        }

        private static IEnumerable<Type> GetInterfaces(Type type)
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