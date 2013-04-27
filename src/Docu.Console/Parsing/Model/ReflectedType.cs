using System;

namespace Docu.Parsing.Model
{
    public class ReflectedType : DocumentedType
    {
        public ReflectedType(Identifier name, Type type)
            : base(name, null, type)
        {
        }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}
