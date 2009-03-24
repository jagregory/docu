using System;
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
        private readonly IEventAggregator eventAggregator;
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations = new Dictionary<Identifier, IReferencable>();

        private readonly IList<IGenerationStep> steps;
        private readonly NamespaceGenerator Namespaces;
        private readonly TypeGenerator Types;
        private readonly MethodGenerator Methods;
        private readonly PropertyGenerator Properties;
        private readonly EventGenerator Events;
        private readonly FieldGenerator Fields;

        public DocumentModel(ICommentParser commentParser, IEventAggregator eventAggregator)
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
                new GenerationStep<IDocumentationMember>(Types.PrePopulate),
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

            foreach (var step in steps)
            {
                foreach (var member in members.Where(step.Criteria))
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

            foreach (var ns in namespaces)
            {
                if (!ns.IsResolved)
                    ns.Resolve(matchedAssociations);
            }

            Sort(namespaces);

            return namespaces;
        }

        private void RaiseCreationWarning(UnsupportedDocumentationMemberException exception)
        {
            eventAggregator
                .GetEvent<WarningEvent>()
                .Publish("Unsupported documentation member found: '" + exception.MemberName + "'");
        }

        private void Sort(List<Namespace> namespaces)
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (var ns in namespaces)
            {
                ns.Sort();
            }
        }
    }
}