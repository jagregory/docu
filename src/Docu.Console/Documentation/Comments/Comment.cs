namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;
    using Parsing.Model;

    public abstract class Comment : IResolvable
    {
        readonly IList<Comment> children = new List<Comment>();

        public IEnumerable<Comment> Children
        {
            get { return children; }
        }

        public bool IsEmpty
        {
            get { return children.Count == 0; }
        }

        public virtual bool IsResolved { get; private set; }

        public void AddChild(Comment comment)
        {
            children.Add(comment);
        }

        public virtual void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            IsResolved = true;
            foreach (IResolvable child in children)
            {
                if (!child.IsResolved)
                {
                    child.Resolve(referencables);
                }
            }
        }
    }
}
