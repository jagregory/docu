using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Docu.Documentation.Comments;
using Docu.Parsing;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class DocumentModel : IDocumentModel
    {
        private readonly ICommentContentParser commentContentParser;
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations = new Dictionary<Identifier, IReferencable>();

        public event EventHandler<DocumentModelWarningEventArgs> CreationWarning = (sender, e) => {};

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
                try
                {
                    AddType(namespaces, references, association);
                }
                catch (UnsupportedDocumentationMemberException ex)
                {
                    RaiseCreationWarning(ex);
                }
            }

            foreach (DocumentedMethod association in members.Where(x => x is DocumentedMethod))
            {
                try
                {
                    AddMethod(namespaces, references, association);
                }
                catch (UnsupportedDocumentationMemberException ex)
                {
                    RaiseCreationWarning(ex);
                }
            }

            foreach (DocumentedProperty association in members.Where(x => x is DocumentedProperty))
            {
                try
                {
                    AddProperty(namespaces, references, association);
                }
                catch (UnsupportedDocumentationMemberException ex)
                {
                    RaiseCreationWarning(ex);
                }
            }

            foreach (IReferencable referencable in references)
            {
                if (!referencable.IsResolved)
                    referencable.Resolve(matchedAssociations);
            }

            Sort(namespaces);

            return namespaces;
        }

        private void RaiseCreationWarning(UnsupportedDocumentationMemberException exception)
        {
            CreationWarning(this, new DocumentModelWarningEventArgs("Unsupported documentation member found: '" + exception.MemberName + "'"));
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

            NamespaceIdentifier namespaceName = Identifier.FromNamespace(association.TargetType.Namespace);
            TypeIdentifier typeName = Identifier.FromType(association.TargetType);
            Namespace @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));

            if (@namespace == null)
            {
                AddNamespace(namespaces, new DocumentedType(association.Name.CloneAsNamespace(), null, association.TargetType));
                @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            }

            DeclaredType type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));

            if (type == null)
            {
                AddType(namespaces, references,
                        new DocumentedType(association.Name.CloneAsType(), null, association.TargetType));
                type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
            }

            DeclaredType methodReturnType = DeclaredType.Unresolved(
                Identifier.FromType(association.Method.ReturnType),
                association.Method.ReturnType,
                Namespace.Unresolved(Identifier.FromNamespace(association.Method.ReturnType.Namespace)));
            Method doc = Method.Unresolved(Identifier.FromMethod(association.Method, association.TargetType),
                                           association.Method, methodReturnType);

            references.Add(methodReturnType);

            if (association.Xml != null)
            {
                XmlNode summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                GetReferencesFromComment(references, doc.Summary);
            }

            foreach (var parameter in association.Method.GetParameters())
            {
                var reference = DeclaredType.Unresolved(
                    Identifier.FromType(parameter.ParameterType),
                    parameter.ParameterType,
                    Namespace.Unresolved(Identifier.FromNamespace(parameter.ParameterType.Namespace)));
                var docParam = new MethodParameter(parameter.Name, reference);

                references.Add(reference);

                if (association.Xml != null)
                {
                    XmlNode paramNode = association.Xml.SelectSingleNode("param[@name='" + parameter.Name + "']");

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
            foreach (IComment comment in comments)
            {
                if (comment is IReferrer)
                    references.Add(((IReferrer)comment).Reference);
            }
        }

        private void AddProperty(List<Namespace> namespaces, List<IReferencable> references, DocumentedProperty association)
        {
            if (association.Property == null) return;

            NamespaceIdentifier namespaceName = Identifier.FromNamespace(association.TargetType.Namespace);
            TypeIdentifier typeName = Identifier.FromType(association.TargetType);
            Namespace @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));

            if (@namespace == null)
            {
                AddNamespace(namespaces, new DocumentedType(association.Name.CloneAsNamespace(), null, association.TargetType));
                @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            }

            DeclaredType type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));

            if (type == null)
            {
                AddType(namespaces, references,
                        new DocumentedType(association.Name.CloneAsType(), null, association.TargetType));
                type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
            }

            DeclaredType propertyReturnType =
                DeclaredType.Unresolved(Identifier.FromType(association.Property.PropertyType),
                                        association.Property.PropertyType,
                                        Namespace.Unresolved(
                                            Identifier.FromNamespace(association.Property.PropertyType.Namespace)));
            Property doc = Property.Unresolved(Identifier.FromProperty(association.Property, association.TargetType),
                                               propertyReturnType);

            references.Add(propertyReturnType);

            if (association.Xml != null)
            {
                XmlNode summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                GetReferencesFromComment(references, doc.Summary);
            }

            references.Add(doc);
            type.AddProperty(doc);
        }

        private void AddNamespace(List<Namespace> namespaces, DocumentedType association)
        {
            var ns = Identifier.FromNamespace(association.Type.Namespace);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(ns)))
            {
                var doc = new Namespace(ns);
                matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }

        private void AddType(List<Namespace> namespaces, List<IReferencable> references, DocumentedType association)
        {
            NamespaceIdentifier namespaceName = Identifier.FromNamespace(association.Type.Namespace);
            Namespace @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            DeclaredType doc = DeclaredType.Unresolved((TypeIdentifier)association.Name, association.Type, @namespace);

            if (association.Xml != null)
            {
                XmlNode summaryNode = association.Xml.SelectSingleNode("summary");

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