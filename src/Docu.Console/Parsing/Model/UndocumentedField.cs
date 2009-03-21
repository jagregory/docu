using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class UndocumentedField : DocumentedField
    {
        public UndocumentedField(Identifier name, FieldInfo field, Type targetType)
            : base(name, null, field, targetType)
        {}
    }
}