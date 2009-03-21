using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class DocumentModel : IDocumentModel
    {
        private readonly ICommentContentParser commentContentParser;
        private readonly IEventAggregator eventAggregator;
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations = new Dictionary<Identifier, IReferencable>();

        private readonly IList<IGenerationStep> steps;

        public DocumentModel(ICommentContentParser commentContentParser, IEventAggregator eventAggregator)
        {
            this.commentContentParser = commentContentParser;
            this.eventAggregator = eventAggregator;

            steps = new List<IGenerationStep>
            {
                new GenerationStep<DocumentedType>(AddNamespace),
                new GenerationStep<DocumentedType>(AddType),
                new GenerationStep<DocumentedMethod>(AddMethod),
                new GenerationStep<DocumentedProperty>(AddProperty),
                new GenerationStep<DocumentedEvent>(AddEvent),
            };
        }

        public IList<Namespace> Create(IEnumerable<IDocumentationMember> members)
        {
            var namespaces = new List<Namespace>();

            matchedAssociations.Clear();

            foreach (var step in steps)
            {
                foreach (var member in members.Where(step.Criteria))
                {
                    try
                    {
                        step.Action(namespaces, member);
                    }
                    catch (UnsupportedDocumentationMemberException ex)
                    {
                        RaiseCreationWarning(ex);
                    }
                }
            }

            foreach (var ns in namespaces)
            {
                if (!ns.IsResolved)
                    ns.Resolve(matchedAssociations);
            }

            Sort(namespaces);

            return namespaces;
        }

        private void RaiseCreationWarning(UnsupportedDocumentationMemberException exception)
        {
            eventAggregator
                .GetEvent<WarningEvent>()
                .Publish("Unsupported documentation member found: '" + exception.MemberName + "'");
        }

        private void Sort(List<Namespace> namespaces)
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (var ns in namespaces)
            {
                ns.Sort();
            }
        }

        private Namespace FindOrCreateNamespace(IDocumentationMember member, List<Namespace> namespaces)
        {
            var identifier = Identifier.FromNamespace(member.TargetType.Namespace);
            var ns = namespaces.Find(x => x.IsIdentifiedBy(identifier));

            if (ns == null)
            {
                AddNamespace(namespaces, new DocumentedType(member.Name.CloneAsNamespace(), null, member.TargetType));
                ns = namespaces.Find(x => x.IsIdentifiedBy(identifier));
            }

            return ns;
        }

        private DeclaredType FindOrCreateType(IDocumentationMember member, Namespace ns, List<Namespace> namespaces)
        {
            var typeName = Identifier.FromType(member.TargetType);
            var type = ns.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));

            if (type == null)
            {
                AddType(namespaces, new DocumentedType(member.Name.CloneAsType(), null, member.TargetType));
                type = ns.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
            }

            return type;
        }

        private void AddMethod(List<Namespace> namespaces, DocumentedMethod association)
        {
            if (association.Method == null) return;

            var ns = FindOrCreateNamespace(association, namespaces);
            var type = FindOrCreateType(association, ns, namespaces);

            DeclaredType methodReturnType = DeclaredType.Unresolved(
                Identifier.FromType(association.Method.ReturnType),
                association.Method.ReturnType,
                Namespace.Unresolved(Identifier.FromNamespace(association.Method.ReturnType.Namespace)));
            Method doc = Method.Unresolved(Identifier.FromMethod(association.Method, association.TargetType),
                                           association.Method, methodReturnType);

            ParseSummary(association, doc);

            foreach (var parameter in association.Method.GetParameters())
            {
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
            type.AddMethod(doc);
        }

        private void AddProperty(List<Namespace> namespaces, DocumentedProperty association)
        {
            if (association.Property == null) return;

            var ns = FindOrCreateNamespace(association, namespaces);
            var type = FindOrCreateType(association, ns, namespaces);

            DeclaredType propertyReturnType =
                DeclaredType.Unresolved(Identifier.FromType(association.Property.PropertyType),
                                        association.Property.PropertyType,
                                        Namespace.Unresolved(
                                            Identifier.FromNamespace(association.Property.PropertyType.Namespace)));
            Property doc = Property.Unresolved(Identifier.FromProperty(association.Property, association.TargetType),
                                               propertyReturnType);

            ParseSummary(association, doc);

            type.AddProperty(doc);
        }

        private void AddEvent(List<Namespace> namespaces, DocumentedEvent association)
        {
            if (association.Event == null) return;

            var ns = FindOrCreateNamespace(association, namespaces);
            var type = FindOrCreateType(association, ns, namespaces);

            var doc = Event.Unresolved(Identifier.FromEvent(association.Event, association.TargetType));

            ParseSummary(association, doc);

            type.AddEvent(doc);
        }

        private void AddNamespace(List<Namespace> namespaces, DocumentedType association)
        {
            var ns = Identifier.FromNamespace(association.TargetType.Namespace);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(ns)))
            {
                var doc = Namespace.Unresolved(ns);
                matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }

        private void AddType(List<Namespace> namespaces, DocumentedType association)
        {
            var ns = FindOrCreateNamespace(association, namespaces);
            DeclaredType doc = DeclaredType.Unresolved((TypeIdentifier)association.Name, association.TargetType, ns);

            ParseSummary(association, doc);

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            matchedAssociations.Add(association.Name, doc);
            ns.AddType(doc);
        }

        private void ParseComment(XmlNode node, IDocumentationElement doc)
        {
            if (node != null)
                doc.Summary = commentContentParser.Parse(node);
        }

        private void ParseParamSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("param[@name='" + doc.Name + "']");

            ParseComment(node, doc);
        }

        private void ParseSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("summary");

            ParseComment(node, doc);
        }
    }
}