using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    internal class GenerationStep<T> : IGenerationStep
    {
        public GenerationStep(Action<List<Namespace>, List<IReferencable>, T> action)
        {
            Action = action;
        }

        public Func<IDocumentationMember, bool> Criteria
        {
            get { return x => x is T; }
        }

        public Action<List<Namespace>, List<IReferencable>, T> Action { get; private set; }
            
        Action<List<Namespace>, List<IReferencable>, IDocumentationMember> IGenerationStep.Action
        {
            get { return (x, y, z) => Action(x, y, (T)z); }
        }
    }
}