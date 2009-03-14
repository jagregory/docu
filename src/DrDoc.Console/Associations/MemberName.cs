using System;
using System.Collections.Generic;
using System.Reflection;

namespace DrDoc.Associations
{
    public class NamespaceMemberName : MemberName
    {
        public NamespaceMemberName(string name)
            : base(name)
        {}

        public override NamespaceMemberName CloneAsNamespace()
        {
            return this;
        }

        public override TypeMemberName CloneAsType()
        {
            throw new System.NotImplementedException();
        }

        public override int CompareTo(MemberName other)
        {
            if (other is NamespaceMemberName)
                return ToString().CompareTo(other.ToString());

            return -1;
        }
    }

    public class TypeMemberName : MemberName
    {
        private readonly string _namespace;

        public TypeMemberName(string name, string ns)
            : base(name)
        {
            _namespace = ns;
        }

        public override NamespaceMemberName CloneAsNamespace()
        {
            return new NamespaceMemberName(_namespace);
        }

        public override TypeMemberName CloneAsType()
        {
            throw new System.NotImplementedException();
        }

        public override int CompareTo(MemberName other)
        {
            if (other is TypeMemberName)
            {
                var t = (TypeMemberName)other;
                var comparison = ToString().CompareTo(t.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = _namespace.CompareTo(t._namespace);

                if (comparison != 0)
                    return comparison;

                return 0;
            }
            if (other is NamespaceMemberName)
                return 1;
         
            return -1;
        }
    }

    public class MethodMemberName : MemberName
    {
        private readonly TypeMemberName typeName;
        private TypeMemberName[] parameters;

        public MethodMemberName(string name, TypeMemberName[] parameters, TypeMemberName typeName)
            : base(name)
        {
            this.typeName = typeName;
            this.parameters = parameters;
        }

        public override NamespaceMemberName CloneAsNamespace()
        {
            return typeName.CloneAsNamespace();
        }

        public override TypeMemberName CloneAsType()
        {
            return typeName;
        }

        public override bool Equals(MemberName obj)
        {
            if (obj is MethodMemberName)
            {
                var other = (MethodMemberName)obj;
                var parametersSame = true;

                if (parameters.Length == other.parameters.Length)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (!parameters[i].Equals(other.parameters[i]))
                            parametersSame = false;
                    }
                }
                else
                    parametersSame = false;

                return base.Equals(obj) && typeName.Equals(other.typeName) && parametersSame;
            }
            return false;
        }

        public override int CompareTo(MemberName other)
        {
            if (other is MethodMemberName)
            {
                var m = (MethodMemberName)other;
                var comparison = ToString().CompareTo(m.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeName.ToString().CompareTo(m.typeName.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = parameters.Length.CompareTo(m.parameters.Length);

                if (comparison != 0)
                    return comparison;

                for (int i = 0; i < parameters.Length; i++)
                {
                    comparison = parameters[i].CompareTo(m.parameters[i]);

                    if (comparison != 0)
                        return comparison;
                }

                return 0;
            }

            if (other is TypeMemberName || other is NamespaceMemberName)
                return 1;
            
            return -1;
        }
    }

    public class PropertyMemberName : MemberName
    {
        private readonly MemberName typeName;

        public PropertyMemberName(string name, MemberName typeName)
            : base(name)
        {
            this.typeName = typeName;
        }

        public override NamespaceMemberName CloneAsNamespace()
        {
            throw new System.NotImplementedException();
        }

        public override TypeMemberName CloneAsType()
        {
            throw new System.NotImplementedException();
        }

        public override int CompareTo(MemberName other)
        {
            if (other is PropertyMemberName)
            {
                var p = (PropertyMemberName)other;
                var comparison = ToString().CompareTo(p.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeName.CompareTo(p.typeName);

                if (comparison != 0)
                    return comparison;

                return 0;
            }

            return -1;
        }
    }
    public abstract class MemberName : IComparable<MemberName>
    {
        private string name;

        protected MemberName(string name)
        {
            this.name = name;
        }

        public static TypeMemberName FromType(Type type)
        {
            return new TypeMemberName(type.Name, type.Namespace);
        }

        public static MemberName FromMethod(MethodInfo method, Type type)
        {
            var name = method.Name;
            var parameters = new List<TypeMemberName>();

            if (method.IsGenericMethod)
                name += "``" + method.GetGenericArguments().Length;

            foreach (var param in method.GetParameters())
            {
                parameters.Add(FromType(param.ParameterType));
            }

            return new MethodMemberName(name, parameters.ToArray(), FromType(type));
        }

        public static MemberName FromProperty(PropertyInfo property, Type type)
        {
            return new PropertyMemberName(property.Name, FromType(type));
        }

        public static NamespaceMemberName FromNamespace(string ns)
        {
            return new NamespaceMemberName(ns);
        }

        public static MemberName FromString(string name)
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

        private static PropertyMemberName FromPropertyName(string name)
        {
            var typeName = GetTypeName(name);
            var propertyName = GetMethodName(name);

            return new PropertyMemberName(propertyName, FromTypeString(typeName));
        }

        private static MethodMemberName FromMethodName(string name)
        {
            var typeName = GetTypeName(name);
            var methodName = GetMethodName(name);
            var parameters = GetMethodParameters(name);

            return new MethodMemberName(methodName, parameters.ToArray(), FromTypeString(typeName));
        }

        private static TypeMemberName FromTypeString(string name)
        {
            var ns = name.Substring(0, name.LastIndexOf('.'));
            var type = name.Substring(name.LastIndexOf('.') + 1);

            return new TypeMemberName(type, ns);
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

        private static List<TypeMemberName> GetMethodParameters(string fullName)
        {
            var parameters = new List<TypeMemberName>();

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






        public abstract NamespaceMemberName CloneAsNamespace();
        public abstract TypeMemberName CloneAsType();

        public static bool operator ==(MemberName first, MemberName second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(MemberName first, MemberName second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (obj is MemberName)
                return Equals((MemberName)obj);
            return base.Equals(obj);
        }

        public virtual bool Equals(MemberName obj)
        {
            if (GetType() != obj.GetType())
                return false;

            return Equals(obj.name, name);
        }

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        public abstract int CompareTo(MemberName other);

        public override string ToString()
        {
            return name; // HACK
        }
    }
}