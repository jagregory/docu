using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using DrDoc.Model;

namespace DrDoc.Parsing
{
    public class AssociationTransformer : IAssociationTransformer
    {
        private readonly ICommentContentParser commentContentParser;
        private readonly IDictionary<Identifier, object> matchedAssociations = new Dictionary<Identifier, object>();

        public AssociationTransformer(ICommentContentParser commentContentParser)
        {
            this.commentContentParser = commentContentParser;
        }

        public IList<DocNamespace> Transform(IEnumerable<IDocumentationMember> members)
        {
            var namespaces = new List<DocNamespace>();
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
                var @namespace = namespaces.Find(x => x.Name == namespaceName);

                if (@namespace == null) continue;

                var type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);

                if (type == null) continue;

                type.AddProperty(new DocProperty(association.Property.Name));
            }

            foreach (var block in references)
            {
                if (matchedAssociations.ContainsKey(block.Reference.Name))
                    block.Reference = (IReferencable)matchedAssociations[block.Reference.Name];
                else
                    block.Reference = new ExternalReference(block.Reference.Name);
            }

            Sort(namespaces);

            return namespaces;
        }

        private void Sort(List<DocNamespace> namespaces)
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (var ns in namespaces)
            {
                ns.Sort();
            }
        }

        private void AddMethod(List<DocNamespace> namespaces, List<IReferrer> references, DocumentedMethod association)
        {
            if (association.Method == null) return;

            var namespaceName = Identifier.FromNamespace(association.TargetType.Namespace);
            var typeName = Identifier.FromType(association.TargetType);
            var @namespace = namespaces.Find(x => x.Name == namespaceName);

            if (@namespace == null)
            {
                AddNamespace(namespaces, new DocumentedType(association.Name.CloneAsNamespace(), null, association.TargetType));
                @namespace = namespaces.Find(x => x.Name == namespaceName);
            }

            var type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);

            if (type == null)
            {
                AddType(namespaces, references, new DocumentedType(association.Name.CloneAsType(), null, association.TargetType));
                type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);
            }

            var prettyName = GetPrettyName(association.Method);
            var doc = new DocMethod((MethodIdentifier)association.Name, prettyName);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                foreach (var block in doc.Summary)
                {
                    if (block is DocReferenceBlock)
                        references.Add((DocReferenceBlock)block);
                }
            }

            foreach (var parameter in association.Method.GetParameters())
            {
                var reference = new UnresolvedReference(Identifier.FromType(parameter.ParameterType));
                var docParam = new DocParameter(parameter.Name, reference);

                references.Add(docParam);

                if (association.Xml != null)
                {
                    var paramNode = association.Xml.SelectSingleNode("param[@name='" + parameter.Name + "']");

                    if (paramNode != null)
                        docParam.Summary = commentContentParser.Parse(paramNode);

                    foreach (var block in docParam.Summary)
                    {
                        if (block is DocReferenceBlock)
                            references.Add((DocReferenceBlock)block);
                    }
                }

                doc.AddParameter(docParam);
            }

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            matchedAssociations.Add(association.Name, doc);
            type.AddMethod(doc);
        }

        private void AddNamespace(List<DocNamespace> namespaces, DocumentedType association)
        {
            var @namespace = Identifier.FromNamespace(association.Type.Namespace);

            if (!namespaces.Exists(x => x.Name == @namespace))
            {
                var doc = new DocNamespace(@namespace);
                matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }

        private void AddType(List<DocNamespace> namespaces, List<IReferrer> references, DocumentedType association)
        {
            var namespaceName = Identifier.FromNamespace(association.Type.Namespace);
            var @namespace = namespaces.Find(x => x.Name == namespaceName);
            var prettyName = GetPrettyName(association.Type);
            var doc = new DocType(association.Name, prettyName, @namespace);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);

                foreach (var block in doc.Summary)
                {
                    if (block is DocReferenceBlock)
                        references.Add((DocReferenceBlock)block);
                }
            }

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            matchedAssociations.Add(association.Name, doc);
            @namespace.AddType(doc);
        }

        private string GetPrettyName(Type type)
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