using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    internal class GenerationStep<T> : IGenerationStep
    {
        public GenerationStep(Action<List<Namespace>, T> action)
        {
            Action = action;
        }

        public Func<IDocumentationMember, bool> Criteria
        {
            get { return x => x is T; }
        }

        public Action<List<Namespace>, T> Action { get; private set; }
            
        Action<List<Namespace>, IDocumentationMember> IGenerationStep.Action
        {
            get { return (x, y) => Action(x, (T)y); }
        }
    }
}