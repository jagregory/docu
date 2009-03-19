using System;
using System.Collections.Generic;
using System.Reflection;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class DocumentableMemberFinder : IDocumentableMemberFinder
    {
        public IList<IDocumentationMember> GetMembersForDocumenting(IEnumerable<Type> types)
        {
            var members = new List<IDocumentationMember>();

            foreach (Type type in types)
            {
                if (type.IsSpecialName) continue;
                if (type.Name.StartsWith("__")) continue; // probably a lambda generated class

                members.Add(new UndocumentedType(Identifier.FromType(type), type));

                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue;

                    members.Add(new UndocumentedMethod(Identifier.FromMethod(method, type), method, type));
                }

                foreach (PropertyInfo property in type.GetProperties())
                {
                    members.Add(new UndocumentedProperty(Identifier.FromProperty(property, type), property, type));
                }
            }

            return members;
        }
    }
}