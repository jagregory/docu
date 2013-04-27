using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedField : DocumentedField
    {
        public ReflectedField(Identifier name, FieldInfo field, Type targetType)
            : base(name, null, field, targetType)
        {
            DeclaringName = field.DeclaringType != targetType
                ? IdentifierFor.Field(field, field.DeclaringType)
                : name;
        }

        public Identifier DeclaringName { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}
