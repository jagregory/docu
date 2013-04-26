using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class DocumentableMemberFinder : IDocumentableMemberFinder
    {
        public IEnumerable<IDocumentationMember> GetMembersForDocumenting(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (type.IsSpecialName) continue;
                if (type.Name.StartsWith("__")) continue; // probably a lambda generated class

                yield return new ReflectedType(Identifier.FromType(type), type);

                foreach (var method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue;

                    yield return new ReflectedMethod(Identifier.FromMethod(method, type), method, type);
                }

                foreach (var constructor in type.GetConstructors())
                {
                    yield return new ReflectedMethod(Identifier.FromMethod(constructor, type), constructor, type);    
                }

                foreach (var property in type.GetProperties())
                {
                    yield return new ReflectedProperty(Identifier.FromProperty(property, type), property, type);
                }

                foreach (var ev in type.GetEvents())
                {
                    yield return new ReflectedEvent(Identifier.FromEvent(ev, type), ev, type);
                }

                foreach (var field in type.GetFields())
                {
                    yield return new ReflectedField(Identifier.FromField(field, type), field, type);
                }
            }
        }
    }
}