using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Generators;
using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class DocumentModel : IDocumentModel
    {
        readonly EventGenerator Events;
        readonly FieldGenerator Fields;
        readonly MethodGenerator Methods;
        readonly NamespaceGenerator Namespaces;
        readonly PropertyGenerator Properties;
        readonly TypeGenerator Types;
        readonly EventAggregator eventAggregator;

        readonly IDictionary<Identifier, IReferencable> matchedAssociations = new Dictionary<Identifier, IReferencable>();
        readonly IList<IGenerationStep> steps;

        public DocumentModel(ICommentParser commentParser, EventAggregator eventAggregator)
        {
            Namespaces = new NamespaceGenerator(matchedAssociations);
            Types = new TypeGenerator(matchedAssociations, commentParser);
            Methods = new MethodGenerator(matchedAssociations, commentParser);
            Properties = new PropertyGenerator(matchedAssociations, commentParser);
            Events = new EventGenerator(matchedAssociations, commentParser);
            Fields = new FieldGenerator(matchedAssociations, commentParser);

            this.eventAggregator = eventAggregator;

            steps = new List<IGenerationStep>
                {
                    new GenerationStep<IDocumentationMember>(Namespaces.Add),
                    new GenerationStep<DocumentedType>(Types.Add),
                    new GenerationStep<DocumentedMethod>(Methods.Add),
                    new GenerationStep<DocumentedProperty>(Properties.Add),
                    new GenerationStep<DocumentedEvent>(Events.Add),
                    new GenerationStep<DocumentedField>(Fields.Add),
                };
        }

        public IList<Namespace> Create(IEnumerable<IDocumentationMember> members)
        {
            var namespaces = new List<Namespace>();

            matchedAssociations.Clear();

            foreach (IGenerationStep step in steps)
            {
                foreach (IDocumentationMember member in members.Where(step.Criteria))
                {
                    try
                    {
                        step.Action(namespaces, member);
                    }
                    catch (UnsupportedDocumentationMemberException ex)
                    {
                        RaiseCreationWarning(ex);
                    }
                }
            }

            foreach (Namespace ns in namespaces)
            {
                if (!ns.IsResolved)
                {
                    ns.Resolve(matchedAssociations);
                }
            }

            Sort(namespaces);

            return namespaces;
        }

        void RaiseCreationWarning(UnsupportedDocumentationMemberException exception)
        {
            eventAggregator.Publish(EventType.Warning, "Unsupported documentation member found: '" + exception.MemberName + "'");
        }

        void Sort(List<Namespace> namespaces)
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (Namespace ns in namespaces)
            {
                ns.Sort();
            }
        }
    }
}
