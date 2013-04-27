using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Docu.Documentation;
using Docu.Documentation.Generators;
using Docu.Events;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class DocumentationModelBuilder
    {
        readonly ICommentParser _commentParser;
        readonly EventAggregator _eventAggregator;

        public DocumentationModelBuilder(ICommentParser commentParser, EventAggregator eventAggregator)
        {
            _commentParser = commentParser;
            _eventAggregator = eventAggregator;
        }

        public IList<Namespace> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xmlDocumentContents)
        {
            var reflectedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(assemblies.SelectMany(a => a.GetExportedTypes()));
            var xmlDocumentationSnippets = GetXmlDocumentationSnippets(xmlDocumentContents);

            List<IDocumentationMember> documentedMembers = DocumentationXmlMatcher.MatchDocumentationToMembers(reflectedMembers, xmlDocumentationSnippets);

            return CombineToTypeHierarchy(documentedMembers);
        }

        IEnumerable<XmlNode> GetXmlDocumentationSnippets(IEnumerable<string> xmlDocumentContents)
        {
            return xmlDocumentContents
                .Where(x => !string.IsNullOrEmpty(x))
                .SelectMany(x =>
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(x);
                        return doc.SelectNodes("doc/members/*").Cast<XmlNode>();
                    });
        }

        public IList<Namespace> CombineToTypeHierarchy(IList<IDocumentationMember> documentedMembers)
        {
            var matchedAssociations = new Dictionary<Identifier, IReferencable>();
            var hierarchy = new List<Namespace>();

            Generate(documentedMembers, new NamespaceGenerator(matchedAssociations), hierarchy);
            Generate(documentedMembers, new TypeGenerator(matchedAssociations, _commentParser), hierarchy);
            Generate(documentedMembers, new MethodGenerator(matchedAssociations, _commentParser), hierarchy);
            Generate(documentedMembers, new PropertyGenerator(matchedAssociations, _commentParser), hierarchy);
            Generate(documentedMembers, new EventGenerator(matchedAssociations, _commentParser), hierarchy);
            Generate(documentedMembers, new FieldGenerator(matchedAssociations, _commentParser), hierarchy);

            foreach (Namespace ns in hierarchy)
            {
                if (!ns.IsResolved)
                {
                    ns.Resolve(matchedAssociations);
                }
            }

            hierarchy.Sort((x, y) => x.Name.CompareTo(y.Name));
            foreach (Namespace ns in hierarchy)
            {
                ns.Sort();
            }

            return hierarchy;
        }

        void Generate<T>(IEnumerable<IDocumentationMember> documentedMembers, IGenerator<T> generator, List<Namespace> hierarchy) where T : class
        {
            foreach (T member in documentedMembers.OfType<T>())
            {
                try
                {
                    generator.Add(hierarchy, member);
                }
                catch (UnsupportedDocumentationMemberException ex)
                {
                    _eventAggregator.Publish(EventType.Warning, "Unsupported documentation member found: '" + ex.MemberName + "'");
                }
            }
        }
    }
}
