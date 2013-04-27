using Docu.Parsing.Model;
using System.Collections.Generic;
using System.Xml;

namespace Docu.Parsing
{
    public static class DocumentationXmlMatcher
    {
        public static IList<IDocumentationMember> DocumentMembers(IEnumerable<IDocumentationMember> undocumentedMembers, IEnumerable<XmlNode> snippets)
        {
            var members = new List<IDocumentationMember>(undocumentedMembers);

            foreach (XmlNode node in snippets)
            {
                string name = node.Attributes["name"].Value;

                if (name.StartsWith("T"))
                    ParseType(members, node);
                else if (name.StartsWith("M"))
                    ParseMethod(members, node);
                else if (name.StartsWith("P"))
                    ParseProperty(members, node);
                else if (name.StartsWith("E"))
                    ParseEvent(members, node);
                else if (name.StartsWith("F"))
                    ParseField(members, node);
            }

            return members;
        }

        static void ParseProperty(List<IDocumentationMember> members, XmlNode node)
        {
            Identifier member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedProperty;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedProperty(reflected.Name, node, reflected.Property, reflected.TargetType);
            }
        }

        static void ParseEvent(List<IDocumentationMember> members, XmlNode node)
        {
            var member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedEvent;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedEvent(reflected.Name, node, reflected.Event, reflected.TargetType);
            }
        }

        static void ParseField(List<IDocumentationMember> members, XmlNode node)
        {
            var member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedField;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedField(reflected.Name, node, reflected.Field, reflected.TargetType);
            }
        }

        static void ParseMethod(List<IDocumentationMember> members, XmlNode node)
        {
            Identifier member = Identifier.FromString(node.Attributes["name"].Value);

            int index = members.FindIndex(x => member.Equals(x.Name));
            if (index == -1) return; // TODO: Privates         

            for (int i = 0; i < members.Count; i++)
            {
                var reflected = members[i] as ReflectedMethod;
                if (reflected != null && reflected.Match(member))
                    members[i] = new DocumentedMethod(reflected.Name, node, reflected.Method, reflected.TargetType);
            }
        }

        static void ParseType(List<IDocumentationMember> members, XmlNode node)
        {
            var identifier = Identifier.FromString(node.Attributes["name"].Value);
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