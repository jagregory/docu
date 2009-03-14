using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using DrDoc.Associations;
using DrDoc.Utils;

namespace DrDoc.Associations
{
    public class Associator : IAssociator
    {
        public IList<Association> Examine(Type[] types, XmlNode[] snippets)
        {
            var associations = new List<Association>();

            foreach (var type in types)
            {
                associations.Add(new UndocumentedTypeAssociation("T:" + type.FullName, type));
            }

            foreach (var node in snippets)
            {
                var name = node.Attributes["name"].Value;

                if (name.StartsWith("T"))
                    ParseType(associations, types, node);
                else if (name.StartsWith("M"))
                    associations.Add(ParseMethod(types, node));
                else if (name.StartsWith("P"))
                    associations.Add(ParseProperty(types, node));
            }

            return associations;
        }

        private Association ParseProperty(Type[] types, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var propertyName = name.Substring(name.LastIndexOf(".") + 1);
            var typeName = name.Substring(0, name.LastIndexOf("."));

            foreach (var type in types)
            {
                if (type.FullName == typeName)
                {
                    var property = type.GetProperty(propertyName);
                    return new PropertyAssociation(node.Attributes["name"].Value, node, property);
                }
            }

            return null;
        }

        private string GetTypeName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(0, name.LastIndexOf("."));
        }

        private string GetMethodName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(name.LastIndexOf(".") + 1);
        }

        private List<Type> GetMethodParameters(string fullName)
        {
            var parameters = new List<Type>();

            if (fullName.EndsWith(")"))
            {
                var paramList = fullName.Substring(fullName.IndexOf("(") + 1);
                paramList = paramList.Substring(0, paramList.Length - 1);

                foreach (var paramName in paramList.Split(','))
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type.FullName == paramName)
                                parameters.Add(type);
                        }
                    }
                }
            }

            return parameters;
        }

        private Association ParseMethod(Type[] types, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var methodName = GetMethodName(name);
            var typeName = GetTypeName(name);
            var parameters = GetMethodParameters(name);

            foreach (var type in types)
            {
                if (type.FullName == typeName)
                {
                    var method = Method.Find(type, methodName, parameters);
                    return new MethodAssociation(node.Attributes["name"].Value, node, method);
                }
            }

            return null;
        }

        private void ParseType(List<Association> associations, Type[] types, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var index = associations.FindIndex(x => x.Name == "T:" + name);

            foreach (var type in types)
            {
                if (type.FullName == name)
                    associations[index] = new TypeAssociation(node.Attributes["name"].Value, node, type);
            }
        }
    }
}