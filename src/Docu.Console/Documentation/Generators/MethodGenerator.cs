using System.Collections.Generic;
using System.IO;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class MethodGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public MethodGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedMethod association)
        {
            if (association.Method == null) return;

            var ns = FindNamespace(association, namespaces);
            var type = FindType(ns, association);

			try {
				var methodReturnType = DeclaredType.Unresolved(
					Identifier.FromType(association.Method.ReturnType),
					association.Method.ReturnType,
					Namespace.Unresolved(Identifier.FromNamespace(association.Method.ReturnType.Namespace)));
				var doc = Method.Unresolved(
					Identifier.FromMethod(association.Method, association.TargetType),
					type, association.Method, methodReturnType);

				ParseSummary(association, doc);
				ParseRemarks(association, doc);
				ParseValue(association, doc);
				ParseReturns(association, doc);
				ParseExample(association, doc);

				foreach (var parameter in association.Method.GetParameters()) {
					var reference = DeclaredType.Unresolved(
						Identifier.FromType(parameter.ParameterType),
						parameter.ParameterType,
						Namespace.Unresolved(Identifier.FromNamespace(parameter.ParameterType.Namespace)));
					var docParam = new MethodParameter(parameter.Name, reference);

					ParseParamSummary(association, docParam);

					doc.AddParameter(docParam);
				}

				if (matchedAssociations.ContainsKey(association.Name))
					return; // weird case when a type has the same method declared twice

				matchedAssociations.Add(association.Name, doc);
				if (type == null) return;
				type.AddMethod(doc);
			} catch (IOException ex) {
				var doc = Method.Unresolved(
					Identifier.FromMethod(association.Method, association.TargetType),
					type, association.Method, new NullReference());
				type.AddMethod(doc);
			}
        }
    }
}