using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Docu.Documentation;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class AssemblyXmlParser
    {
        private readonly IDocumentModel documentModel;

        public AssemblyXmlParser(IDocumentModel documentModel)
        {
            this.documentModel = documentModel;
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

            return DocumentationXmlMatcher.DocumentMembers(undocumentedMembers, members);
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
                types.AddRange(assembly.GetExportedTypes());
            }

            return DocumentableMemberFinder.GetMembersForDocumenting(types);
        }
    }
}