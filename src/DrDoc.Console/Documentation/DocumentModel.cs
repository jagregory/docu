using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DrDoc.Documentation.Comments;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing;
using DrDoc.Parsing.Model;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class DocumentModel : IDocumentModel
    {
        private readonly ICommentContentParser commentContentParser;
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations = new Dictionary<Identifier, IReferencable>();

        public DocumentModel(ICommentContentParser commentContentParser)
        {
            this.commentContentParser = commentContentParser;
        }

        public IList<Namespace> Create(IEnumerable<IDocumentationMember> members)
        {
            var namespaces = new List<Namespace>();
            var references = new List<IReferencable>();

            matchedAssociations.Clear();

            foreach (DocumentedType association in members.Where(x => x is DocumentedType))
            {
                AddNamespace(namespaces, association);
            }

            foreach (DocumentedType association in members.Where(x => x is DocumentedType))
            {
                AddType(namespaces, references, association);
            }

            foreach (DocumentedMethod association in members.Where(x => x is DocumentedMethod))
            {
                AddMethod(namespaces, references, association);
            }

            foreach (DocumentedProperty association in members.Where(x => x is DocumentedProperty))
            {
                AddProperty(namespaces, references, association);
            }

            foreach (var referencable in references)
            {
                if (!referencable.IsResolved)
                    referencable.Resolve(matchedAssociations);
            }

            Sort(namespaces);

            return namespaces;
        }

        private void Sort(List<Namespace> namespaces)
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (var ns in namespaces)
            {
                ns.Sort();
            }
        }

        private void AddMethod(List<Namespace> namespaces, List<IReferencable> references, DocumentedMethod association)
        {
            if (association.Method == null) return;

            var namespaceName = Identifier.FromNamespace(association.TargetType.Namespace);
            var typeName = Identifier.FromType(association.TargetType);
            var @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));

            if (@namespace == null)
            {
                AddNamespace(namespaces, new DocumentedType(association.Name.CloneAsNamespace(), null, association.TargetType));
                @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            }

            var type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));

            if (type == null)
            {
                AddType(namespaces, references, new DocumentedType(association.Name.CloneAsType(), null, association.TargetType));
                type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
            }

            var methodReturnType = DeclaredType.Unresolved(Identifier.FromType(association.Method.ReturnType), association.Method.ReturnType, Namespace.Unresolved(Identifier.FromNamespace(association.Method.ReturnType.Namespace)));
            var doc = Method.Unresolved(Identifier.FromMethod(association.Method, association.TargetType), association.Method, methodReturnType);

            references.Add(methodReturnType);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                GetReferencesFromComment(references, doc.Summary);
            }

            foreach (var parameter in association.Method.GetParameters())
            {
                var reference = DeclaredType.Unresolved(Identifier.FromType(parameter.ParameterType), parameter.ParameterType, @namespace);
                var docParam = new MethodParameter(parameter.Name, reference);

                references.Add(reference);

                if (association.Xml != null)
                {
                    var paramNode = association.Xml.SelectSingleNode("param[@name='" + parameter.Name + "']");

                    if (paramNode != null)
                        docParam.Summary = commentContentParser.Parse(paramNode);

                    GetReferencesFromComment(references, docParam.Summary);
                }

                doc.AddParameter(docParam);
            }

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            references.Add(doc);
            matchedAssociations.Add(association.Name, doc);
            type.AddMethod(doc);
        }

        private void GetReferencesFromComment(List<IReferencable> references, IList<IComment> comments)
        {
            foreach (var comment in comments)
            {
                if (comment is IReferrer)
                    references.Add(((IReferrer)comment).Reference);
            }
        }

        private void AddProperty(List<Namespace> namespaces, List<IReferencable> references, DocumentedProperty association)
        {
            if (association.Property == null) return;

            var namespaceName = Identifier.FromNamespace(association.TargetType.Namespace);
            var typeName = Identifier.FromType(association.TargetType);
            var @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));

            if (@namespace == null)
            {
                AddNamespace(namespaces, new DocumentedType(association.Name.CloneAsNamespace(), null, association.TargetType));
                @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            }

            var type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));

            if (type == null)
            {
                AddType(namespaces, references, new DocumentedType(association.Name.CloneAsType(), null, association.TargetType));
                type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
            }

            var propertyReturnType = DeclaredType.Unresolved(Identifier.FromType(association.Property.PropertyType), association.Property.PropertyType, Namespace.Unresolved(Identifier.FromNamespace(association.Property.PropertyType.Namespace)));
            var doc = Property.Unresolved(Identifier.FromProperty(association.Property, association.TargetType), propertyReturnType);

            references.Add(propertyReturnType);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                GetReferencesFromComment(references, doc.Summary);
            }

            references.Add(doc);
            type.AddProperty(doc);
        }

        private void AddNamespace(List<Namespace> namespaces, DocumentedType association)
        {
            var @namespace = Identifier.FromNamespace(association.Type.Namespace);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(@namespace)))
            {
                var doc = new Namespace(@namespace);
                matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }

        private void AddType(List<Namespace> namespaces, List<IReferencable> references, DocumentedType association)
        {
            var namespaceName = Identifier.FromNamespace(association.Type.Namespace);
            var @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            var doc = DeclaredType.Unresolved((TypeIdentifier)association.Name, association.Type, @namespace);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                GetReferencesFromComment(references, doc.Summary);
            }

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            references.Add(doc);
            matchedAssociations.Add(association.Name, doc);
            @namespace.AddType(doc);
        }
    }
}