using System;
using System.Collections.Generic;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public abstract class Identifier : IComparable<Identifier>, IEquatable<Identifier>
    {
        private readonly string name;
        private static Dictionary<string, Type> nameToType;

        protected Identifier(string name)
        {
            this.name = name;
        }

        protected string Name
        {
            get { return name; }
        }

        public static TypeIdentifier FromType(Type type)
        {
            return new TypeIdentifier(type.Name, type.Namespace);
        }

        public static MethodIdentifier FromMethod(MethodInfo method, Type type)
        {
            string name = method.Name;
            var parameters = new List<TypeIdentifier>();

            if (method.IsGenericMethod)
                name += "``" + method.GetGenericArguments().Length;

            foreach (ParameterInfo param in method.GetParameters())
            {
                parameters.Add(FromType(param.ParameterType));
            }

            return new MethodIdentifier(name, parameters.ToArray(), method.IsStatic, method.IsPublic, FromType(type));
        }

        public static PropertyIdentifier FromProperty(PropertyInfo property, Type type)
        {
            return new PropertyIdentifier(property.Name, property.CanRead, property.CanWrite, FromType(type));
        }

        public static FieldIdentifier FromField(FieldInfo field, Type type)
        {
            return new FieldIdentifier(field.Name, FromType(type));
        }

        public static EventIdentifier FromEvent(EventInfo ev, Type type)
        {
            return new EventIdentifier(ev.Name, FromType(type));
        }

        public static NamespaceIdentifier FromNamespace(string ns)
        {
            return new NamespaceIdentifier(ns);
        }

        public static Identifier FromString(string name)
        {
            var prefix = name.Substring(0, 1);
            var trimmedName = name.Substring(2);

            if (prefix == "T")
                return FromTypeString(trimmedName);
            if (prefix == "N")
                return FromNamespace(trimmedName);
            if (prefix == "M")
                return FromMethodName(trimmedName);
            if (prefix == "P")
                return FromPropertyName(trimmedName);
            if (prefix == "E")
                return FromEventName(trimmedName);
            if (prefix == "F")
                return FromFieldName(trimmedName);

            throw new UnsupportedDocumentationMemberException(name);
        }

        private static PropertyIdentifier FromPropertyName(string name)
        {
            string typeName = GetTypeName(name);
            string propertyName = GetMethodName(name);

            return new PropertyIdentifier(propertyName, false, false, FromTypeString(typeName));
        }

        private static EventIdentifier FromEventName(string name)
        {
            var typeName = GetTypeName(name);
            var eventName = GetMethodName(name);

            return new EventIdentifier(eventName, FromTypeString(typeName));
        }

        private static FieldIdentifier FromFieldName(string name)
        {
            var typeName = GetTypeName(name);
            var fieldName = GetMethodName(name);

            return new FieldIdentifier(fieldName, FromTypeString(typeName));
        }

        private static MethodIdentifier FromMethodName(string name)
        {
            string typeName = GetTypeName(name);
            string methodName = GetMethodName(name);
            List<TypeIdentifier> parameters = GetMethodParameters(name);

            return new MethodIdentifier(methodName, parameters.ToArray(), false, false, FromTypeString(typeName));
        }

        private static TypeIdentifier FromTypeString(string name)
        {
            string ns;
            string type;

            if (name.Contains("."))
            {
                // has namespace
                ns = name.Substring(0, name.LastIndexOf('.'));
                type = name.Substring(name.LastIndexOf('.') + 1);
            }
            else
            {
                // special case where no namespace is used
                ns = "Unknown";
                type = name;
            }
            

            return new TypeIdentifier(type, ns);
        }

        private static string GetTypeName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(0, name.LastIndexOf("."));
        }

        private static string GetMethodName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(name.LastIndexOf(".") + 1);
        }

        private static List<TypeIdentifier> GetMethodParameters(string fullName)
        {
            var parameters = new List<TypeIdentifier>();

            if(nameToType == null)
            {
                // build type lookup table
                nameToType = new Dictionary<string, Type>();
                foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach(Type type in assembly.GetTypes())
                    {
                        nameToType[type.FullName] = type;
                    }
                }
            }

            if (fullName.EndsWith(")"))
            {
                string paramList = fullName.Substring(fullName.IndexOf("(") + 1);
                paramList = paramList.Substring(0, paramList.Length - 1);

                foreach (string paramName in paramList.Split(','))
                {
                    Type paramType;
                    if(nameToType.TryGetValue(paramName, out paramType))
                    {
                        parameters.Add(FromType(paramType));
                    }
                }
            }

            return parameters;
        }

        public abstract NamespaceIdentifier CloneAsNamespace();
        public abstract TypeIdentifier CloneAsType();

        public static bool operator ==(Identifier first, Identifier second)
        {
            return (((object)first) != null) && first.Equals(second);
        }

        public static bool operator !=(Identifier first, Identifier second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identifier);
        }

        public abstract bool Equals(Identifier obj);

        public abstract int CompareTo(Identifier other);

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return name; // HACK
        }
    }
}