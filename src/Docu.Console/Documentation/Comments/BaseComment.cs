using System.Collections.Generic;
using System.Linq;
using Docu.Parsing.Model;

namespace Docu.Documentation.Comments
{
    public abstract class BaseComment : IComment, IResolvable
    {
        private readonly IList<IComment> inner = new List<IComment>();

        public IEnumerable<IComment> Children
        {
            get { return inner; }
        }

        public void AddChild(IComment comment)
        {
            inner.Add(comment);
        }

        public virtual bool IsResolved { get; private set; }

        public virtual void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            IsResolved = true;

            foreach (IResolvable child in Children.Where(x => x is IResolvable))
            {
                if (!child.IsResolved)
                    child.Resolve(referencables);
            }
        }
    }
}