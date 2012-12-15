namespace Docu.Documentation
{
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    public interface IDocumentModel
    {
        IList<Namespace> Create(IEnumerable<IDocumentationMember> members);
    }
}