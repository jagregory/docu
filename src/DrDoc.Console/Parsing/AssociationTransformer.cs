using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrDoc.Associations;

namespace DrDoc.Parsing
{
    public class AssociationTransformer : IAssociationTransformer
    {
        private readonly ICommentContentParser commentContentParser;
        private readonly IDictionary<string, object> matchedAssociations = new Dictionary<string, object>();

        public AssociationTransformer(ICommentContentParser commentContentParser)
        {
            this.commentContentParser = commentContentParser;
        }

        public IList<DocNamespace> Transform(IEnumerable<Association> associations)
        {
            var namespaces = new List<DocNamespace>();
            var references = new List<DocReferenceBlock>();

            matchedAssociations.Clear();

            foreach (TypeAssociation association in associations.Where(x => x is TypeAssociation))
            {
                AddNamespace(namespaces, association);
            }

            foreach (TypeAssociation association in associations.Where(x => x is TypeAssociation))
            {
                AddType(namespaces, references, association);
            }

            foreach (MethodAssociation association in associations.Where(x => x is MethodAssociation))
            {
                AddMethod(namespaces, references, association);
            }

            foreach (PropertyAssociation association in associations.Where(x => x is PropertyAssociation))
            {
                if (association.Property == null) continue;

                var namespaceName = association.Property.DeclaringType.Namespace;
                var typeName = association.Property.DeclaringType.Name;
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

            return namespaces;
        }

        private void AddMethod(List<DocNamespace> namespaces, List<DocReferenceBlock> references, MethodAssociation association)
        {
            if (association.Method == null) return;

            var namespaceName = association.Method.DeclaringType.Namespace;
            var typeName = association.Method.DeclaringType.Name;
            var @namespace = namespaces.Find(x => x.Name == namespaceName);

            if (@namespace == null)
            {
                AddNamespace(namespaces, new TypeAssociation(association.Name.Replace("M:", "N:"), null, association.Method.DeclaringType));
                @namespace = namespaces.Find(x => x.Name == namespaceName);
            }

            var type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);

            if (type == null)
            {
                AddType(namespaces, references, new TypeAssociation(association.Name.Replace("M:", "T:"), null, association.Method.DeclaringType));
                type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);
            }

            var doc = new DocMethod(association.Method.Name);

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
                var docParam = new DocParameter(parameter.Name, parameter.ParameterType.FullName);

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

            matchedAssociations.Add(association.Name, doc);
            type.AddMethod(doc);
        }

        private void AddNamespace(List<DocNamespace> namespaces, TypeAssociation association)
        {
            var @namespace = association.Type.Namespace;

            if (!namespaces.Exists(x => x.Name == @namespace))
            {
                var doc = new DocNamespace(@namespace);
                matchedAssociations.Add("N:" + association.Name.Substring(2), doc);
                namespaces.Add(doc);
            }
        }

        private void AddType(List<DocNamespace> namespaces, List<DocReferenceBlock> references, TypeAssociation association)
        {
            var namespaceName = association.Type.Namespace;
            var @namespace = namespaces.Find(x => x.Name == namespaceName);
            var prettyName = GetPrettyName(association.Type);
            var doc = new DocType(association.Type.Name, prettyName, @namespace);

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

            matchedAssociations.Add(association.Name, doc);
            @namespace.AddType(doc);
        }

        private string GetPrettyName(Type type)
        {
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
    }
}