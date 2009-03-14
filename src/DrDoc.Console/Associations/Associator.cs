using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            PrePopulate(types, associations);

            foreach (var node in snippets)
            {
                var name = node.Attributes["name"].Value;

                if (name.StartsWith("T"))
                    ParseType(associations, types, node);
                else if (name.StartsWith("M"))
                    ParseMethod(associations, types, node);
                else if (name.StartsWith("P"))
                    ParseProperty(associations, types, node);
            }

            return associations;
        }

        private void PrePopulate(Type[] types, List<Association> associations)
        {
            foreach (var type in types)
            {
                if (type.IsSpecialName) continue;

                associations.Add(new UndocumentedTypeAssociation(MemberName.FromType(type), type));
                
                foreach (var method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue;

                    associations.Add(new UndocumentedMethodAssociation(MemberName.FromMethod(method, type), method, type));
                }

                foreach (var property in type.GetProperties())
                {
                    associations.Add(new UndocumentedPropertyAssociation(MemberName.FromProperty(property, type), property));
                }
            }
        }

        private void ParseProperty(List<Association> associations, Type[] types, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var propertyName = name.Substring(name.LastIndexOf(".") + 1);
            var typeName = name.Substring(0, name.LastIndexOf("."));
            var member = MemberName.FromString(node.Attributes["name"].Value);

            foreach (var type in types)
            {
                if (type.FullName == typeName)
                {
                    var property = type.GetProperty(propertyName);
                    var index = associations.FindIndex(x => x.Name == member);

                    if (index == -1) return; // TODO: Privates

                    associations[index] = new PropertyAssociation(member, node, property);
                }
            }
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

        private void ParseMethod(List<Association> associations, Type[] types, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var member = MemberName.FromString(node.Attributes["name"].Value);
            var methodName = GetMethodName(name);
            var typeName = GetTypeName(name);
            var parameters = GetMethodParameters(name);
            var index = associations.FindIndex(x => x.Name == member);

            if (index == -1) return; // TODO: Privates
            if (methodName == "#ctor") return; // TODO: Fix constructors

            foreach (var type in types)
            {
                if (type.FullName == typeName)
                {
                    var method = Method.Find(type, methodName, parameters);
                    associations[index] = new MethodAssociation(member, node, method, type);
                }
            }
        }

        private void ParseType(List<Association> associations, Type[] types, XmlNode node)
        {
            var name = node.Attributes["name"].Value.Substring(2);
            var member = MemberName.FromString(node.Attributes["name"].Value);
            var index = associations.FindIndex(x => x.Name == member);

            if (index == -1) return; // TODO: Privates

            foreach (var type in types)
            {
                if (type.FullName == name)
                    associations[index] = new TypeAssociation(member, node, type);
            }
        }
    }
}