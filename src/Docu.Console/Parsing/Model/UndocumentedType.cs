using System;

namespace Docu.Parsing.Model
{
    public class UndocumentedType : DocumentedType
    {
        public UndocumentedType(Identifier name, Type type)
            : base(name, null, type)
        {}
    }
}