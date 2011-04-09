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

        public AssemblyXmlParser(IDocumentationXmlMatcher xmlMatcher, IDocumentModel documentModel, IDocumentableMemberFinder documentableMembers)
        {
            this.xmlMatcher = xmlMatcher;
            this.documentModel = documentModel;
            this.documentableMembers = documentableMembers;
        }

        public IList<Namespace> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xmlDocumentContents)
        {
            var members = GetAssociations(assemblies, xmlDocumentContents);

            return documentModel.Create(members);
        }

        private IEnumerable<IDocumentationMember> GetAssociations(IEnumerable<Assembly> assemblies, IEnumerable<string> xmlDocumentContents)
        {
            var undocumentedMembers = GetUndocumentedMembers(assemblies);
            var members = GetMembersXml(xmlDocumentContents);

            return xmlMatcher.DocumentMembers(undocumentedMembers, members);
        }

        private IEnumerable<XmlNode> GetMembersXml(IEnumerable<string> xmlDocumentContents)
        {
            foreach (var xmlDocumentContent in xmlDocumentContents)
            {
                if (string.IsNullOrEmpty(xmlDocumentContent)) continue;

                var doc = new XmlDocument();
                doc.LoadXml(xmlDocumentContent);

                foreach (XmlNode node in doc.SelectNodes("doc/members/*"))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IDocumentationMember> GetUndocumentedMembers(IEnumerable<Assembly> assemblies)
        {
            var types = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
				foreach (var type in assembly.GetExportedTypes()) {
					types.Add(type);
				}
            }

            return documentableMembers.GetMembersForDocumenting(types);
        }
    }
}