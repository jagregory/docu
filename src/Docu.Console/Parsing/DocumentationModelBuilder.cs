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

            var documentedMembers = DocumentationXmlMatcher.DocumentMembers(reflectedMembers, xmlDocumentationSnippets);

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

            var namespaces = new NamespaceGenerator(matchedAssociations);
            var types = new TypeGenerator(matchedAssociations, _commentParser);
            var methods = new MethodGenerator(matchedAssociations, _commentParser);
            var properties = new PropertyGenerator(matchedAssociations, _commentParser);
            var events = new EventGenerator(matchedAssociations, _commentParser);
            var fields = new FieldGenerator(matchedAssociations, _commentParser);

            var steps = new List<IGenerationStep>
                {
                    new GenerationStep<IDocumentationMember>(namespaces.Add),
                    new GenerationStep<DocumentedType>(types.Add),
                    new GenerationStep<DocumentedMethod>(methods.Add),
                    new GenerationStep<DocumentedProperty>(properties.Add),
                    new GenerationStep<DocumentedEvent>(events.Add),
                    new GenerationStep<DocumentedField>(fields.Add),
                };

            var hierarchy = new List<Namespace>();

            foreach (IGenerationStep step in steps)
            {
                foreach (IDocumentationMember member in documentedMembers.Where(step.Criteria))
                {
                    try
                    {
                        step.Action(hierarchy, member);
                    }
                    catch (UnsupportedDocumentationMemberException ex)
                    {
                        _eventAggregator.Publish(EventType.Warning, "Unsupported documentation member found: '" + ex.MemberName + "'");
                    }
                }
            }

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
    }
}
