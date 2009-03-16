using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class UndocumentedProperty : DocumentedProperty
    {
        public UndocumentedProperty(Identifier name, PropertyInfo property, Type targetType)
            : base(name, null, property, targetType)
        {
        }
    }
}