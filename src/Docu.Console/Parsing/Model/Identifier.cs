using System;
using System.Collections.Generic;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public abstract class Identifier : IComparable<Identifier>
    {
        private readonly string name;

        protected Identifier(string name)
        {
            this.name = name;
        }

        public abstract int CompareTo(Identifier other);

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

        public static NamespaceIdentifier FromNamespace(string ns)
        {
            return new NamespaceIdentifier(ns);
        }

        public static Identifier FromString(string name)
        {
            string trimmedName = name.Substring(2);

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
            string typeName = GetTypeName(name);
            string propertyName = GetMethodName(name);

            return new PropertyIdentifier(propertyName, false, false, FromTypeString(typeName));
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
            string ns = name.Substring(0, name.LastIndexOf('.'));
            string type = name.Substring(name.LastIndexOf('.') + 1);

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
                string paramList = fullName.Substring(fullName.IndexOf("(") + 1);
                paramList = paramList.Substring(0, paramList.Length - 1);

                foreach (string paramName in paramList.Split(','))
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type type in assembly.GetTypes())
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

        public override string ToString()
        {
            return name; // HACK
        }
    }
}