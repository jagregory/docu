using System.Collections.Generic;
using System.Linq;
using DrDoc.Associations;

namespace DrDoc.Parsing
{
    public class AssociationTransformer : IAssociationTransformer
    {
        public IList<DocNamespace> Transform(IEnumerable<Association> associations)
        {
            var namespaces = new List<DocNamespace>();

            foreach (TypeAssociation association in associations.Where(x => x is TypeAssociation))
            {
                var @namespace = association.Type.Namespace;

                if (!namespaces.Exists(x => x.Name == @namespace))
                    namespaces.Add(new DocNamespace(@namespace));
            }

            foreach (TypeAssociation association in associations.Where(x => x is TypeAssociation))
            {
                var namespaceName = association.Type.Namespace;
                var @namespace = namespaces.Find(x => x.Name == namespaceName);
                var doc = new DocType(association.Type.Name);

                var summaryNode = association.Xml.SelectSingleNode("summary");

                if (summaryNode != null)
                    doc.Summary = summaryNode.InnerText;

                @namespace.AddType(doc);
            }

            foreach (MethodAssociation association in associations.Where(x => x is MethodAssociation))
            {
                if (association.Method == null) continue; // BUG

                var namespaceName = association.Method.DeclaringType.Namespace;
                var typeName = association.Method.DeclaringType.Name;
                var @namespace = namespaces.Find(x => x.Name == namespaceName);

                if (@namespace == null) continue;
                var type = @namespace.Types.FirstOrDefault(x => x.Name == typeName);

                if (type == null) continue;
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
    }
}