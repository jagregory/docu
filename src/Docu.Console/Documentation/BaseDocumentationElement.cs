namespace Docu.Documentation
{
    using Docu.Documentation.Comments;
    using Docu.Parsing.Model;

    public abstract class BaseDocumentationElement : IDocumentationElement
    {
        protected readonly Identifier identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDocumentationElement"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        protected BaseDocumentationElement(Identifier identifier)
        {
            this.Name = identifier.ToString();
            this.identifier = identifier;
            this.IsResolved = true;
            this.Summary = new Summary();
            this.Remarks = new Remarks();
            this.Value = new Value();
            this.Example = new MultilineCode();
        }

        public MultilineCode Example { get; set; }

        public virtual bool HasDocumentation
        {
            get
            {
                return !(this.Summary.IsEmpty && this.Remarks.IsEmpty && this.Value.IsEmpty);
            }
        }

        public bool IsExternal { get; private set; }

        public bool IsResolved { get; protected set; }

        public string Name { get; private set; }

        public Remarks Remarks { get; set; }

        public Summary Summary { get; set; }

        public Value Value { get; set; }

        public virtual void ConvertToExternalReference()
        {
            this.IsExternal = true;
        }

        public bool IsIdentifiedBy(Identifier otherIdentifier)
        {
            return this.identifier.Equals(otherIdentifier);
        }
    }
}