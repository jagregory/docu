using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private readonly IDictionary<Identifier, object> matchedAssociations = new Dictionary<Identifier, object>();

        public DocumentModel(ICommentContentParser commentContentParser)
        {
            this.commentContentParser = commentContentParser;
        }

        public IList<Namespace> Create(IEnumerable<IDocumentationMember> members)
        {
            var namespaces = new List<Namespace>();
            var references = new List<IReferrer>();

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
                if (association.Property == null) continue;

                var namespaceName = Identifier.FromNamespace(association.Property.DeclaringType.Namespace);
                var typeName = Identifier.FromType(association.Property.DeclaringType);
                var @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));

                if (@namespace == null) continue;

                var type = @namespace.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));

                if (type == null) continue;

                type.AddProperty(new Property(association.Property.Name));
            }

            foreach (var block in references)
            {
                var set = false;

                foreach (var identifier in matchedAssociations.Keys)
                {
                    if (block.Reference.IsIdentifiedBy(identifier))
                    {
                        block.Reference = (IReferencable)matchedAssociations[identifier];
                        set = true;
                        break;
                    }
                }

                if (!set)
                    block.Reference = block.Reference.ToExternalReference();
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

        private void AddMethod(List<Namespace> namespaces, List<IReferrer> references, DocumentedMethod association)
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

            var prettyName = GetPrettyName(association.Method);
            var doc = new Method((MethodIdentifier)association.Name, prettyName);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                foreach (var block in doc.Summary)
                {
                    if (block is See)
                        references.Add((See)block);
                }
            }

            foreach (var parameter in association.Method.GetParameters())
            {
                var reference = new UnresolvedReference(Identifier.FromType(parameter.ParameterType));
                var docParam = new MethodParameter(parameter.Name, reference);

                references.Add(docParam);

                if (association.Xml != null)
                {
                    var paramNode = association.Xml.SelectSingleNode("param[@name='" + parameter.Name + "']");

                    if (paramNode != null)
                        docParam.Summary = commentContentParser.Parse(paramNode);

                    foreach (var block in docParam.Summary)
                    {
                        if (block is See)
                            references.Add((See)block);
                    }
                }

                doc.AddParameter(docParam);
            }

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            matchedAssociations.Add(association.Name, doc);
            type.AddMethod(doc);
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

        private void AddType(List<Namespace> namespaces, List<IReferrer> references, DocumentedType association)
        {
            var namespaceName = Identifier.FromNamespace(association.Type.Namespace);
            var @namespace = namespaces.Find(x => x.IsIdentifiedBy(namespaceName));
            var prettyName = GetPrettyName(association.Type);
            var doc = new DeclaredType(association.Name, prettyName, @namespace);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                foreach (var block in doc.Summary)
                {
                    if (block is See)
                        references.Add((See)block);
                }
            }

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            matchedAssociations.Add(association.Name, doc);
            @namespace.AddType(doc);
        }

        private string GetPrettyName(System.Type type)
        {
            if (type.IsNested) return type.Name;
            if (type.IsGenericType)
            {
                var sb = new StringBuilder();

                sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
                sb.Append("<");

                foreach (var argument in type.GetGenericArguments())
                {
                    sb.Append(argument.Name);
                    sb.Append(", ");
                }

                sb.Length -= 2;
                sb.Append(">");

                return sb.ToString();
            }

            return type.Name;
        }

        private string GetPrettyName(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                var sb = new StringBuilder();
                var name = method.Name;

                if (name.Contains("`"))
                    name = method.Name.Substring(0, method.Name.IndexOf('`'));

                sb.Append(name);
                sb.Append("<");

                foreach (var argument in method.GetGenericArguments())
                {
                    sb.Append(argument.Name);
                    sb.Append(", ");
                }

                sb.Length -= 2;
                sb.Append(">");

                return sb.ToString();
            }

            return method.Name;
        }
    }
}