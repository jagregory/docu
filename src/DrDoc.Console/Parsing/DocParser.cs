using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using DrDoc.Associations;
using DrDoc.Parsing;

namespace DrDoc
{
    public class DocParser
    {
        private readonly IAssociator associator;
        private readonly IAssociationTransformer transformer;

        public DocParser(IAssociator associator, IAssociationTransformer transformer)
        {
            this.associator = associator;
            this.transformer = transformer;
        }

        public IList<DocNamespace> Parse(Assembly[] assemblies, string[] xml)
        {
            var assocations = GetAssociations(assemblies, xml);

            return transformer.Transform(assocations);
        }

        private IEnumerable<Association> GetAssociations(Assembly[] assemblies, string[] xml)
        {
            var types = GetTypes(assemblies);
            var members = GetMembers(xml);

            return associator.Examine(types, members);
        }

        private XmlNode[] GetMembers(string[] xml)
        {
            var nodes = new List<XmlNode>();

            foreach (var chunk in xml)
            {
                if (string.IsNullOrEmpty(chunk)) continue;

                var doc = new XmlDocument();
                doc.LoadXml(chunk);

                foreach (var node in doc.SelectNodes("doc/members/*"))
                {
                    nodes.Add((XmlNode)node);
                }
            }

            return nodes.ToArray();
        }

        private Type[] GetTypes(Assembly[] assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            return types.ToArray();
        }
    }
}