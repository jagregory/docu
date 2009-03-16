using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Docu.Documentation;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class AssemblyXmlParser : IAssemblyXmlParser
    {
        private readonly IDocumentableMemberFinder documentableMembers;
        private readonly IDocumentModel documentModel;
        private readonly IDocumentationXmlMatcher xmlMatcher;

        public AssemblyXmlParser(IDocumentationXmlMatcher xmlMatcher, IDocumentModel documentModel,
                                 IDocumentableMemberFinder documentableMembers)
        {
            this.xmlMatcher = xmlMatcher;
            this.documentModel = documentModel;
            this.documentableMembers = documentableMembers;
        }

        public IList<Namespace> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xml)
        {
            IEnumerable<IDocumentationMember> members = GetAssociations(assemblies, xml);

            return documentModel.Create(members);
        }

        private IEnumerable<IDocumentationMember> GetAssociations(IEnumerable<Assembly> assemblies,
                                                                  IEnumerable<string> xml)
        {
            IList<IDocumentationMember> undocumentedMembers = GetUndocumentedMembers(assemblies);
            XmlNode[] members = GetMembersXml(xml);

            return xmlMatcher.DocumentMembers(undocumentedMembers, members);
        }

        private XmlNode[] GetMembersXml(IEnumerable<string> xml)
        {
            var nodes = new List<XmlNode>();

            foreach (string chunk in xml)
            {
                if (string.IsNullOrEmpty(chunk)) continue;

                var doc = new XmlDocument();
                doc.LoadXml(chunk);

                foreach (object node in doc.SelectNodes("doc/members/*"))
                {
                    nodes.Add((XmlNode)node);
                }
            }

            return nodes.ToArray();
        }

        private IList<IDocumentationMember> GetUndocumentedMembers(IEnumerable<Assembly> assemblies)
        {
            var types = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.GetExportedTypes());
            }

            return documentableMembers.GetMembersForDocumenting(types);
        }
    }
}