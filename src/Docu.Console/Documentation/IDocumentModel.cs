using System;
using System.Collections.Generic;
using Docu.Parsing;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IDocumentModel
    {
        event EventHandler<DocumentModelWarningEventArgs> CreationWarning;
        IList<Namespace> Create(IEnumerable<IDocumentationMember> members);
    }
}