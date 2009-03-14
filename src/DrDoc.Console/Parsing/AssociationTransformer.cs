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

        public AssociationTransformer(ICommentContentParser commentContentParser)
        {
            this.commentContentParser = commentContentParser;
        }

        public IList<DocNamespace> Transform(IEnumerable<Association> associations)
        {
            var namespaces = new List<DocNamespace>();

            foreach (TypeAssociation association in associations.Where(x => x is TypeAssociation))
            {
                AddNamespace(namespaces, association);
            }

            foreach (TypeAssociation association in associations.Where(x => x is TypeAssociation))
            {
                AddType(namespaces, association);
            }

            foreach (MethodAssociation association in associations.Where(x => x is MethodAssociation))
            {
                if (association.Method == null) continue; // BUG

                var namespaceName = association.Method.DeclaringType.Namespace;
                var typeName = association.Method.DeclaringType.Name;
                var @namespace = namespaces.Find(x => x.Name == namespaceName);

                if (@namespace == null)
                {
                    AddNamespace(namespaces, new TypeAssociation(null, association.Method.DeclaringType));
                    @namespace = namespaces.Find(x => x.Name == namespaceName);
                }

                var type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);

                if (type == null)
                {
                    AddType(namespaces, new TypeAssociation(null, association.Method.DeclaringType));
                    type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);
                }

                var doc = new DocMethod(association.Method.Name);

                foreach (var parameter in association.Method.GetParameters())
                {
                    doc.AddParameter(new DocParameter(parameter.Name, parameter.ParameterType.FullName));
                }

                type.AddMethod(doc);
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

            return namespaces;
        }

        private void AddNamespace(List<DocNamespace> namespaces, TypeAssociation association)
        {
            var @namespace = association.Type.Namespace;

            if (!namespaces.Exists(x => x.Name == @namespace))
                namespaces.Add(new DocNamespace(@namespace));
        }

        private void AddType(List<DocNamespace> namespaces, TypeAssociation association)
        {
            var namespaceName = association.Type.Namespace;
            var @namespace = namespaces.Find(x => x.Name == namespaceName);
            var prettyName = GetPrettyName(association.Type);
            var doc = new DocType(association.Type.Name, prettyName);

            if (association.Xml != null)
            {
                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = commentContentParser.Parse(summaryNode);
            }

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