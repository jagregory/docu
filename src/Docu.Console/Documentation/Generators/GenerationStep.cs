namespace Docu.Documentation.Generators
{
    using System;
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    internal class GenerationStep<T> : IGenerationStep
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationStep{T}"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public GenerationStep(Action<List<Namespace>, T> action)
        {
            this.Action = action;
        }

        public Action<List<Namespace>, T> Action { get; private set; }

        public Func<IDocumentationMember, bool> Criteria
        {
            get
            {
                return x => x is T;
            }
        }

        Action<List<Namespace>, IDocumentationMember> IGenerationStep.Action
        {
            get
            {
                return (x, y) => this.Action(x, (T)y);
            }
        }
    }
}