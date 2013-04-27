namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;
    using System.Linq;

    using Docu.Parsing.Model;

    public abstract class BaseComment : IComment, IResolvable
    {
        private readonly IList<IComment> inner = new List<IComment>();

        public IEnumerable<IComment> Children
        {
            get
            {
                return this.inner;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return !this.Children.Any();
            }
        }

        public virtual bool IsResolved { get; private set; }

        public void AddChild(IComment comment)
        {
            this.inner.Add(comment);
        }

        public virtual void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            this.IsResolved = true;

            foreach (IResolvable child in this.Children.Where(x => x is IResolvable))
            {
                if (!child.IsResolved)
                {
                    child.Resolve(referencables);
                }
            }
        }
    }
}