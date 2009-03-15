using System;
using System.Collections.Generic;
using DrDoc.Parsing.Model;

namespace DrDoc.Parsing
{
    public class DocumentableMemberFinder : IDocumentableMemberFinder
    {
        public IList<IDocumentationMember> GetMembersForDocumenting(IEnumerable<Type> types)
        {
            var members = new List<IDocumentationMember>();

            foreach (var type in types)
            {
                if (type.IsSpecialName) continue;

                members.Add(new UndocumentedType(Identifier.FromType(type), type));

                foreach (var method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue;

                    members.Add(new UndocumentedMethod(Identifier.FromMethod(method, type), method, type));
                }

                foreach (var property in type.GetProperties())
                {
                    members.Add(new UndocumentedProperty(Identifier.FromProperty(property, type), property));
                }
            }

            return members;
        }
    }
}