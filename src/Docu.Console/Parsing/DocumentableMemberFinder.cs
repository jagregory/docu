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

            foreach (var type in types)
            {
                if (type.IsSpecialName) continue;
                if (type.Name.StartsWith("__")) continue; // probably a lambda generated class

                members.Add(new UndocumentedType(Identifier.FromType(type), type));

                foreach (var method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue;

                    members.Add(new UndocumentedMethod(Identifier.FromMethod(method, type), method, type));
                }

                foreach (var property in type.GetProperties())
                {
                    members.Add(new UndocumentedProperty(Identifier.FromProperty(property, type), property, type));
                }

                foreach (var ev in type.GetEvents())
                {
                    members.Add(new UndocumentedEvent(Identifier.FromEvent(ev, type), ev, type));
                }

                foreach (var field in type.GetFields())
                {
                    members.Add(new UndocumentedField(Identifier.FromField(field, type), field, type));
                }
            }

            return members;
        }
    }
}