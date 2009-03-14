using System;
using System.Reflection;

namespace DrDoc.Associations
{
    public class UndocumentedMethodAssociation : MethodAssociation
    {
        public UndocumentedMethodAssociation(MemberName name, MethodInfo method, Type targetType)
            : base(name, null, method, targetType)
        {}
    }
}