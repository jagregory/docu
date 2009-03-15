using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using DrDoc.Model;
using DrDoc.Parsing;

namespace DrDoc
{
    public class DocParser
    {
        private readonly IAssociator associator;
        private readonly IAssociationTransformer transformer;
        private readonly IDocumentableMemberFinder documentableMembers;

        public DocParser(IAssociator associator, IAssociationTransformer transformer, IDocumentableMemberFinder documentableMembers)
        {
            this.associator = associator;
            this.transformer = transformer;
            this.documentableMembers = documentableMembers;
        }

        public IList<DocNamespace> Parse(Assembly[] assemblies, string[] xml)
        {
            var members = GetAssociations(assemblies, xml);

            return transformer.Transform(members);
        }

        private IEnumerable<IDocumentationMember> GetAssociations(Assembly[] assemblies, string[] xml)
        {
            var undocumentedMembers = GetUndocumentedMembers(assemblies);
            var members = GetMembersXml(xml);

            return associator.AssociateMembersWithXml(undocumentedMembers, members);
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