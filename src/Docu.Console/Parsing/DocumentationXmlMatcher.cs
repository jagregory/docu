using Docu.Parsing.Model;
using System.Collections.Generic;
using System.Xml;

namespace Docu.Parsing
{
    public static class DocumentationXmlMatcher
    {
        public static List<IDocumentationMember> MatchDocumentationToMembers(IEnumerable<IDocumentationMember> reflectedMembers, IEnumerable<XmlNode> xmlDocumentationSnippets)
        {
            var members = new List<IDocumentationMember>(reflectedMembers);

            foreach (XmlNode node in xmlDocumentationSnippets)
            {
                string name = node.Attributes["name"].Value;

                if (name.StartsWith("T"))
                    MatchType(members, node);
                else if (name.StartsWith("M"))
                    MatchMethod(members, node);
                else if (name.StartsWith("P"))
                    MatchProperty(members, node);
                else if (name.StartsWith("E"))
                    MatchEvent(members, node);
                else if (name.StartsWith("F"))
                    MatchField(members, node);
            }

            return members;
        }

        static void MatchProperty(List<IDocumentationMember> members, XmlNode node)
        {
            Identifier member = IdentifierFor.XmlString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedProperty;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedProperty(reflected.Name, node, reflected.Property, reflected.TargetType);
            }
        }

        static void MatchEvent(List<IDocumentationMember> members, XmlNode node)
        {
            var member = IdentifierFor.XmlString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedEvent;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedEvent(reflected.Name, node, reflected.Event, reflected.TargetType);
            }
        }

        static void MatchField(List<IDocumentationMember> members, XmlNode node)
        {
            var member = IdentifierFor.XmlString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedField;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedField(reflected.Name, node, reflected.Field, reflected.TargetType);
            }
        }

        static void MatchMethod(List<IDocumentationMember> members, XmlNode node)
        {
            Identifier member = IdentifierFor.XmlString(node.Attributes["name"].Value);   

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedMethod;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedMethod(reflected.Name, node, reflected.Method, reflected.TargetType);
            }
        }

        static void MatchType(List<IDocumentationMember> members, XmlNode node)
        {
            var identifier = IdentifierFor.XmlString(node.Attributes["name"].Value);
            var positionOfUndocumentedType = members.FindIndex(m =>
                {
                    var reflected = m as ReflectedType;
                    return reflected != null && reflected.Match(identifier);
                });

            if (positionOfUndocumentedType >= 0)
            {
                members[positionOfUndocumentedType] = new DocumentedType(identifier, node, members[positionOfUndocumentedType].TargetType);
            }
        }
    }
}
