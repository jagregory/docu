using System;
using System.Collections.Generic;
using System.Reflection;

namespace DrDoc.Parsing.Model
{
    public abstract class Identifier : IComparable<Identifier>
    {
        private string name;

        protected Identifier(string name)
        {
            this.name = name;
        }

        public static TypeIdentifier FromType(Type type)
        {
            return new TypeIdentifier(type.Name, type.Namespace);
        }

        public static MethodIdentifier FromMethod(MethodInfo method, Type type)
        {
            var name = method.Name;
            var parameters = new List<TypeIdentifier>();

            if (method.IsGenericMethod)
                name += "``" + method.GetGenericArguments().Length;

            foreach (var param in method.GetParameters())
            {
                parameters.Add(FromType(param.ParameterType));
            }

            return new MethodIdentifier(name, parameters.ToArray(), FromType(type));
        }

        public static Identifier FromProperty(PropertyInfo property, Type type)
        {
            return new PropertyIdentifier(property.Name, FromType(type));
        }

        public static NamespaceIdentifier FromNamespace(string ns)
        {
            return new NamespaceIdentifier(ns);
        }

        public static Identifier FromString(string name)
        {
            var trimmedName = name.Substring(2);

            if (name.StartsWith("T:"))
                return FromTypeString(trimmedName);
            if (name.StartsWith("N:"))
                return FromNamespace(trimmedName);
            if (name.StartsWith("M:"))
                return FromMethodName(trimmedName);
            if (name.StartsWith("P:"))
                return FromPropertyName(trimmedName);

            throw new InvalidOperationException("Unexpected name type");
        }

        private static PropertyIdentifier FromPropertyName(string name)
        {
            var typeName = GetTypeName(name);
            var propertyName = GetMethodName(name);

            return new PropertyIdentifier(propertyName, FromTypeString(typeName));
        }

        private static MethodIdentifier FromMethodName(string name)
        {
            var typeName = GetTypeName(name);
            var methodName = GetMethodName(name);
            var parameters = GetMethodParameters(name);

            return new MethodIdentifier(methodName, parameters.ToArray(), FromTypeString(typeName));
        }

        private static TypeIdentifier FromTypeString(string name)
        {
            var ns = name.Substring(0, name.LastIndexOf('.'));
            var type = name.Substring(name.LastIndexOf('.') + 1);

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

            if (fullName.EndsWith(")"))
            {
                var paramList = fullName.Substring(fullName.IndexOf("(") + 1);
                paramList = paramList.Substring(0, paramList.Length - 1);

                foreach (var paramName in paramList.Split(','))
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type.FullName == paramName)
                                parameters.Add(FromType(type));
                        }
                    }
                }
            }

            return parameters;
        }

        public abstract NamespaceIdentifier CloneAsNamespace();
        public abstract TypeIdentifier CloneAsType();

        public static bool operator ==(Identifier first, Identifier second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Identifier first, Identifier second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (obj is Identifier)
                return Equals((Identifier)obj);
            return base.Equals(obj);
        }

        public virtual bool Equals(Identifier obj)
        {
            if (GetType() != obj.GetType())
                return false;

            return Equals(obj.name, name);
        }

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        public abstract int CompareTo(Identifier other);

        public override string ToString()
        {
            return name; // HACK
        }
    }
}