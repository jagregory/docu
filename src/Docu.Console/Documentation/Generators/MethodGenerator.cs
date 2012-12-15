namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;
    using System.Reflection;

    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    internal class MethodGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodGenerator"/> class.
        /// </summary>
        /// <param name="matchedAssociations">
        /// The matched associations.
        /// </param>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
        public MethodGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedMethod association)
        {
            if (association.Method == null)
            {
                return;
            }

            Namespace ns = this.FindNamespace(association, namespaces);
            DeclaredType type = this.FindType(ns, association);

            DeclaredType methodReturnType = DeclaredType.Unresolved(
                Identifier.FromType(association.Method.ReturnType), 
                association.Method.ReturnType, 
                Namespace.Unresolved(Identifier.FromNamespace(association.Method.ReturnType.Namespace)));
            Method doc = Method.Unresolved(
                Identifier.FromMethod(association.Method, association.TargetType), 
                type, 
                association.Method, 
                methodReturnType);

            this.ParseSummary(association, doc);
            this.ParseRemarks(association, doc);
            this.ParseValue(association, doc);
            this.ParseReturns(association, doc);
            this.ParseExample(association, doc);

            foreach (ParameterInfo parameter in association.Method.GetParameters())
            {
                DeclaredType reference = DeclaredType.Unresolved(
                    Identifier.FromType(parameter.ParameterType), 
                    parameter.ParameterType, 
                    Namespace.Unresolved(Identifier.FromNamespace(parameter.ParameterType.Namespace)));
                var docParam = new MethodParameter(parameter.Name, reference);

                this.ParseParamSummary(association, docParam);

                doc.AddParameter(docParam);
            }

            if (this.matchedAssociations.ContainsKey(association.Name))
            {
                return; // weird case when a type has the same method declared twice
            }

            this.matchedAssociations.Add(association.Name, doc);

            if (type == null)
            {
                return;
            }

            type.AddMethod(doc);
        }
    }
}