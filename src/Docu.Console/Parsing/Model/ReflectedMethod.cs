using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedMethod : DocumentedMethod
    {
        public ReflectedMethod(Identifier name, MethodBase method, Type targetType)
            : base(name, null, method, targetType)
        {
            DeclaringName = method.DeclaringType != targetType
                ? IdentifierFor.Method(method, method.DeclaringType)
                : name;
        }

        public Identifier DeclaringName { get; private set; }

        public bool Match(Identifier name)
        {
            return DeclaringName.Equals(name);
        }
    }
}
