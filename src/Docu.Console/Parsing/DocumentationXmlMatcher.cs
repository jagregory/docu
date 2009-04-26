using System.Collections.Generic;
using System.Xml;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class DocumentationXmlMatcher : IDocumentationXmlMatcher
    {
        public IList<IDocumentationMember> DocumentMembers(IList<IDocumentationMember> undocumentedMembers, XmlNode[] snippets)
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

        private string GetMethodName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(name.LastIndexOf(".") + 1);
        }

        private void ParseProperty(List<IDocumentationMember> members, XmlNode node)
        {
            Identifier member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var propertyMember = members[i] as UndocumentedProperty;

                if (propertyMember != null && propertyMember.Match(member))
                    members[i] = new DocumentedProperty(member, node, propertyMember.Property, propertyMember.TargetType);
            }
        }

        private void ParseEvent(List<IDocumentationMember> members, XmlNode node)
        {
            var member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var eventMember = members[i] as UndocumentedEvent;

                if (eventMember != null && eventMember.Match(member))
                    members[i] = new DocumentedEvent(member, node, eventMember.Event, eventMember.TargetType);
            }
        }

        private void ParseField(List<IDocumentationMember> members, XmlNode node)
        {
            var member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var eventMember = members[i] as UndocumentedField;

                if (eventMember != null && eventMember.Match(member))
                    members[i] = new DocumentedField(member, node, eventMember.Field, eventMember.TargetType);
            }
        }

        private void ParseMethod(List<IDocumentationMember> members, XmlNode node)
        {
            string name = node.Attributes["name"].Value.Substring(2);
            Identifier member = Identifier.FromString(node.Attributes["name"].Value);
            string methodName = GetMethodName(name);
            int index = members.FindIndex(x => member.Equals(x.Name));

            if (index == -1) return; // TODO: Privates
            if (methodName == "#ctor") return; // TODO: Fix constructors

            for (int i = 0; i < members.Count; i++)
            {
                var methodMember = members[i] as UndocumentedMethod;

                if (methodMember != null && methodMember.Match(member))
                    members[i] = new DocumentedMethod(member, node, methodMember.Method, methodMember.TargetType);
            }
        }

        private void ParseType(IList<IDocumentationMember> members, XmlNode node)
        {
            Identifier member = Identifier.FromString(node.Attributes["name"].Value);

            for (int i = 0; i < members.Count; i++)
            {
                var typeMember = members[i] as UndocumentedType;

                if (typeMember != null && typeMember.Match(member))
                    members[i] = new DocumentedType(member, node, typeMember.TargetType);
            }
        }
    }
}