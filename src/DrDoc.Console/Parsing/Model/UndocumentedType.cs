using System;
using DrDoc.Parsing.Model;

namespace DrDoc.Parsing.Model
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