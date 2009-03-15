using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using DrDoc.Documentation;
using DrDoc.Parsing.Model;
using DrDoc.Parsing;

namespace DrDoc
{
    public class Parser
    {
        private readonly IDocumentationXmlMatcher xmlMatcher;
        private readonly IDocumentModel documentModel;
        private readonly IDocumentableMemberFinder documentableMembers;

        public Parser(IDocumentationXmlMatcher xmlMatcher, IDocumentModel documentModel, IDocumentableMemberFinder documentableMembers)
        {
            this.xmlMatcher = xmlMatcher;
            this.documentModel = documentModel;
            this.documentableMembers = documentableMembers;
        }

        public IList<Namespace> CreateDocumentModel(Assembly[] assemblies, string[] xml)
        {
            var members = GetAssociations(assemblies, xml);

            return documentModel.Create(members);
        }

        private IEnumerable<IDocumentationMember> GetAssociations(Assembly[] assemblies, string[] xml)
        {
            var undocumentedMembers = GetUndocumentedMembers(assemblies);
            var members = GetMembersXml(xml);

            return xmlMatcher.DocumentMembers(undocumentedMembers, members);
        }

        private XmlNode[] GetMembersXml(string[] xml)
        {
            var nodes = new List<XmlNode>();

            foreach (var chunk in xml)
            {
                if (string.IsNullOrEmpty(chunk)) continue;

                var doc = new XmlDocument();
                doc.LoadXml(chunk);

                foreach (var node in doc.SelectNodes("doc/members/*"))
                {
                    nodes.Add((XmlNode)node);
                }
            }

            return nodes.ToArray();
        }

        private IList<IDocumentationMember> GetUndocumentedMembers(Assembly[] assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetExportedTypes());
            }

            return documentableMembers.GetMembersForDocumenting(types);
        }
    }
}