using System.Collections.Generic;
using System.Reflection;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class MethodGenerator : BaseGenerator, IGenerator<DocumentedMethod>
    {
        readonly IDictionary<Identifier, IReferencable> _matchedAssociations;

        public MethodGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            _matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedMethod association)
        {
            if (association.Method == null)
            {
                return;
            }

            DeclaredType type = FindType(association, namespaces);

            DeclaredType methodReturnType = null;
            if (association.Method.MemberType == MemberTypes.Method)
            {
                methodReturnType = DeclaredType.Unresolved(
                    IdentifierFor.Type(((MethodInfo)association.Method).ReturnType),
                    ((MethodInfo) association.Method).ReturnType,
                    Namespace.Unresolved(IdentifierFor.Namespace(((MethodInfo)association.Method).ReturnType.Namespace)));
            }

            Method doc = Method.Unresolved(
                IdentifierFor.Method(association.Method, association.TargetType),
                type,
                association.Method,
                methodReturnType);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseValue(association, doc);
            ParseReturns(association, doc);
            ParseExample(association, doc);

            foreach (ParameterInfo parameter in association.Method.GetParameters())
            {
                DeclaredType reference = DeclaredType.Unresolved(
                    IdentifierFor.Type(parameter.ParameterType),
                    parameter.ParameterType,
                    Namespace.Unresolved(IdentifierFor.Namespace(parameter.ParameterType.Namespace)));
                var docParam = new MethodParameter(parameter.Name, reference);

                ParseParamSummary(association, docParam);

                doc.AddParameter(docParam);
            }

            if (_matchedAssociations.ContainsKey(association.Name))
            {
                return; // weird case when a type has the same method declared twice
            }

            _matchedAssociations.Add(association.Name, doc);

            if (type == null)
            {
                return;
            }

            type.AddMethod(doc);
        }
    }
}
