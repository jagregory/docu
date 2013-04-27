namespace Docu.Documentation.Generators
{
    using System;
    using System.Collections.Generic;

    using Parsing.Model;

    internal interface IGenerationStep
    {
        Func<IDocumentationMember, bool> Criteria { get; }
        Action<List<Namespace>, IDocumentationMember> Action { get; }
    }
}
