using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    internal interface IGenerationStep
    {
        Func<IDocumentationMember, bool> Criteria { get; }
        Action<List<Namespace>, IDocumentationMember> Action { get; }
    }
}