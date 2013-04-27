namespace Docu.Documentation
{
    using System.Collections.Generic;
    using System.Linq;

    using Docu.Documentation.Generators;
    using Docu.Events;
    using Docu.Parsing;
    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    public class DocumentModel : IDocumentModel
    {
        private readonly EventGenerator Events;

        private readonly FieldGenerator Fields;

        private readonly MethodGenerator Methods;

        private readonly NamespaceGenerator Namespaces;

        private readonly PropertyGenerator Properties;

        private readonly TypeGenerator Types;

        private readonly IEventAggregator eventAggregator;

        private readonly IDictionary<Identifier, IReferencable> matchedAssociations =
            new Dictionary<Identifier, IReferencable>();

        private readonly IList<IGenerationStep> steps;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentModel"/> class.
        /// </summary>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        public DocumentModel(ICommentParser commentParser, IEventAggregator eventAggregator)
        {
            this.Namespaces = new NamespaceGenerator(this.matchedAssociations);
            this.Types = new TypeGenerator(this.matchedAssociations, commentParser);
            this.Methods = new MethodGenerator(this.matchedAssociations, commentParser);
            this.Properties = new PropertyGenerator(this.matchedAssociations, commentParser);
            this.Events = new EventGenerator(this.matchedAssociations, commentParser);
            this.Fields = new FieldGenerator(this.matchedAssociations, commentParser);

            this.eventAggregator = eventAggregator;

            this.steps = new List<IGenerationStep>
                {
                    new GenerationStep<IDocumentationMember>(this.Namespaces.Add), 
                    new GenerationStep<DocumentedType>(this.Types.Add), 
                    new GenerationStep<DocumentedMethod>(this.Methods.Add), 
                    new GenerationStep<DocumentedProperty>(this.Properties.Add), 
                    new GenerationStep<DocumentedEvent>(this.Events.Add), 
                    new GenerationStep<DocumentedField>(this.Fields.Add), 
                };
        }

        public IList<Namespace> Create(IEnumerable<IDocumentationMember> members)
        {
            var namespaces = new List<Namespace>();

            this.matchedAssociations.Clear();

            foreach (IGenerationStep step in this.steps)
            {
                foreach (IDocumentationMember member in members.Where(step.Criteria))
                {
                    try
                    {
                        step.Action(namespaces, member);
                    }
                    catch (UnsupportedDocumentationMemberException ex)
                    {
                        this.RaiseCreationWarning(ex);
                    }
                }
            }

            foreach (Namespace ns in namespaces)
            {
                if (!ns.IsResolved)
                {
                    ns.Resolve(this.matchedAssociations);
                }
            }

            this.Sort(namespaces);

            return namespaces;
        }

        private void RaiseCreationWarning(UnsupportedDocumentationMemberException exception)
        {
            this.eventAggregator.GetEvent<WarningEvent>().Publish(
                "Unsupported documentation member found: '" + exception.MemberName + "'");
        }

        private void Sort(List<Namespace> namespaces)
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (Namespace ns in namespaces)
            {
                ns.Sort();
            }
        }
    }
}