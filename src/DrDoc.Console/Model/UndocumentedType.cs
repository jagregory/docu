using System;
using DrDoc.Model;

namespace DrDoc.Model
{
    public class UndocumentedType : DocumentedType
    {
        public UndocumentedType(Identifier name, Type type)
            : base(name, null, type)
        {
            Type = type;
        }
    }
}