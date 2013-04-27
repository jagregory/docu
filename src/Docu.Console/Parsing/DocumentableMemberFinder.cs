using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public static class DocumentableMemberFinder
    {
        public static IEnumerable<IDocumentationMember> ReflectMembersForDocumenting(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (type.IsSpecialName) continue;
                if (type.Name.StartsWith("__")) continue; // probably a lambda generated class

                yield return new ReflectedType(IdentifierFor.Type(type), type);

                foreach (var method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue;

                    yield return new ReflectedMethod(IdentifierFor.Method(method, type), method, type);
                }

                foreach (var constructor in type.GetConstructors())
                {
                    yield return new ReflectedMethod(IdentifierFor.Method(constructor, type), constructor, type);
                }

                foreach (var property in type.GetProperties())
                {
                    yield return new ReflectedProperty(IdentifierFor.Property(property, type), property, type);
                }

                foreach (var ev in type.GetEvents())
                {
                    yield return new ReflectedEvent(IdentifierFor.Event(ev, type), ev, type);
                }

                foreach (var field in type.GetFields())
                {
                    yield return new ReflectedField(IdentifierFor.Field(field, type), field, type);
                }
            }
        }
    }
}