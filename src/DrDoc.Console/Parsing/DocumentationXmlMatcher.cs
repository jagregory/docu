using System.Collections.Generic;
using System.Xml;
using DrDoc.Parsing.Model;
using DrDoc.Parsing.Model;

namespace DrDoc.Parsing
{
    public class DocumentationXmlMatcher : IDocumentationXmlMatcher
    {
        public IList<IDocumentationMember> DocumentMembers(IList<IDocumentationMember> undocumentedMembers, XmlNode[] snippets)
        {
            var members = new List<IDocumentationMember>(undocumentedMembers);

            foreach (var node in snippets)
            {
                var name = node.Attributes["name"].Value;

                if (name.StartsWith("T"))
                    ParseType(members, node);
                else if (name.StartsWith("M"))
                    ParseMethod(members, node);
                else if (name.StartsWith("P"))
                    ParseProperty(members, node);
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
            var member = Identifier.FromString(node.Attributes["name"].Value);

            for (var i = 0; i < members.Count; i++)
            {
                var propertyMember = members[i] as UndocumentedProperty;

                if (propertyMember != null && propertyMember.Match(member))
                    members[i] = new DocumentedProperty(member, node, propertyMember.Property, propertyMember.TargetType);
            }
        }

        private void ParseMethod(List<IDocumentationMember> members, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var member = Identifier.FromString(node.Attributes["name"].Value);
            var methodName = GetMethodName(name);
            var index = members.FindIndex(x => x.Name == member);

            if (index == -1) return; // TODO: Privates
            if (methodName == "#ctor") return; // TODO: Fix constructors

            for (var i = 0; i < members.Count; i++)
            {
                var methodMember = members[i] as UndocumentedMethod;

                if (methodMember != null && methodMember.Match(member))
                    members[i] = new DocumentedMethod(member, node, methodMember.Method, methodMember.TargetType);
            }
        }

        private void ParseType(IList<IDocumentationMember> members, XmlNode node)
        {
            var member = Identifier.FromString(node.Attributes["name"].Value);

            for (var i = 0; i < members.Count; i++)
            {
                var typeMember = members[i] as UndocumentedType;

                if (typeMember != null && typeMember.Match(member))
                    members[i] = new DocumentedType(member, node, typeMember.Type);
            }
        }
    }
}