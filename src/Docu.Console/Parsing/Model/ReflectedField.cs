using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedField : DocumentedField
    {
        public ReflectedField(Identifier name, FieldInfo field, Type targetType)
            : base(name, null, field, targetType)
        {}
    }
}