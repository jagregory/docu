using System;
using System.Collections.Generic;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public static class IdentifierFor
    {
        const string GenericParamaterPrefix = "``";
        const string GenericRankPrefix = "`";
        const string ArrayTypeSuffix = "[]";
        const char StartGenericArguments = '{';
        const char EndGenericArguments = '}';
        const char RefoutParameterSuffix = '@';
        const string GenericTypeNamespace = "";

        static Dictionary<string, Type> _nameToType;

        public static TypeIdentifier Type(Type type)
        {
            return type.IsGenericParameter
                ? new TypeIdentifier(GenericParamaterPrefix + type.GenericParameterPosition, GenericTypeNamespace)
                : new TypeIdentifier(type.Name, type.Namespace);
        }

        public static MethodIdentifier Method(MethodBase method, Type type)
        {
            string name = method.Name;
            var parameters = new List<TypeIdentifier>();

            if (method.IsGenericMethod)
                name += GenericParamaterPrefix + method.GetGenericArguments().Length;

            foreach (ParameterInfo param in method.GetParameters())
            {
                parameters.Add(Type(param.ParameterType));
            }

            return new MethodIdentifier(name, parameters.ToArray(), method.IsStatic, method.IsPublic, method.IsConstructor, Type(type));
        }

        public static PropertyIdentifier Property(PropertyInfo property, Type type)
        {
            return new PropertyIdentifier(property.Name, property.CanRead, property.CanWrite, Type(type));
        }

        public static FieldIdentifier Field(FieldInfo field, Type type)
        {
            return new FieldIdentifier(field.Name, Type(type));
        }

        public static EventIdentifier Event(EventInfo ev, Type type)
        {
            return new EventIdentifier(ev.Name, Type(type));
        }

        public static NamespaceIdentifier Namespace(string ns)
        {
            return new NamespaceIdentifier(ns);
        }

        public static Identifier XmlString(string name)
        {
            char prefix = name[0];
            string trimmedName = name.Substring(2);

            if (prefix == 'T') return TypeString(trimmedName);
            if (prefix == 'N') return Namespace(trimmedName);
            if (prefix == 'M') return MethodName(trimmedName);
            if (prefix == 'P') return new PropertyIdentifier(GetMethodName(trimmedName), false, false, TypeString(GetTypeName(trimmedName)));
            if (prefix == 'E') return new EventIdentifier(GetMethodName(trimmedName), TypeString(GetTypeName(trimmedName)));
            if (prefix == 'F') return new FieldIdentifier(GetMethodName(trimmedName), TypeString(GetTypeName(trimmedName)));

            throw new UnsupportedDocumentationMemberException(name);
        }

        static MethodIdentifier MethodName(string name)
        {
            string typeName = GetTypeName(name);
            string methodName = GetMethodName(name);

            // Constructors in XML has name #ctor but in assembly .ctor
            if (methodName == "#ctor")
            {
                methodName = ".ctor";
            }

            List<TypeIdentifier> parameters = GetMethodParameters(name);
            return new MethodIdentifier(methodName, parameters.ToArray(), false, false, methodName == ".ctor", TypeString(typeName));
        }

        static TypeIdentifier TypeString(string name)
        {
            return name.Contains(".")
                ? new TypeIdentifier(name.Substring(name.LastIndexOf('.') + 1), name.Substring(0, name.LastIndexOf('.')))
                : new TypeIdentifier(name, "Unknown");
        }

        static string GetTypeName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(0, name.LastIndexOf("."));
        }

        static string GetMethodName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(name.LastIndexOf(".") + 1);
        }

        static List<TypeIdentifier> GetMethodParameters(string fullName)
        {
            var parameters = new List<TypeIdentifier>();
            if (!fullName.EndsWith(")")) return parameters;

            BuildTypeLookup();

            int firstCharAfterParen = fullName.IndexOf("(") + 1;
            string paramList = fullName.Substring(firstCharAfterParen, fullName.Length - firstCharAfterParen - 1);

            foreach (string paramName in ExtractMethodArgumentTypes(paramList))
            {
                if (IsGenericArgument(paramName))
                {
                    parameters.Add(new TypeIdentifier(paramName, GenericTypeNamespace));
                    continue;
                }
                string typeNameToFind = paramName;
                int startOfGenericArguments = paramName.IndexOf(StartGenericArguments);
                if (startOfGenericArguments > 0)
                {
                    string nonGenericPartOfTypeName = paramName.Substring(0, startOfGenericArguments);
                    int endOfGenericArguments = paramName.LastIndexOf(EndGenericArguments);
                    int lengthOfGenericArgumentsSection = endOfGenericArguments - startOfGenericArguments - 1;
                    string genericArgumentsSection = paramName.Substring(startOfGenericArguments + 1, lengthOfGenericArgumentsSection);
                    int countOfGenericParametersForType = CountOfGenericArguments(genericArgumentsSection);
                    typeNameToFind = nonGenericPartOfTypeName + GenericRankPrefix + countOfGenericParametersForType;
                }
                Type paramType;
                bool isArray = typeNameToFind.EndsWith(ArrayTypeSuffix);
                if (isArray) typeNameToFind = typeNameToFind.Substring(0, typeNameToFind.Length - 2);
                if (_nameToType.TryGetValue(typeNameToFind, out paramType))
                {
                    if (isArray) paramType = paramType.MakeArrayType();
                    parameters.Add(Type(paramType));
                }
            }

            return parameters;
        }

        static int CountOfGenericArguments(string genericArguments)
        {
            int count = 1;
            int startPosition = 0;
            while (startPosition < genericArguments.Length)
            {
                int positionOfInterestingChar = genericArguments.IndexOfAny(new[] { StartGenericArguments, ',' }, startPosition);
                if (positionOfInterestingChar < 0)
                {
                    return count;
                }
                if (genericArguments[positionOfInterestingChar] == StartGenericArguments)
                {
                    startPosition = IndexAfterGenericArguments(genericArguments, positionOfInterestingChar);
                }
                else
                {
                    ++count;
                    startPosition = positionOfInterestingChar + 1;
                }
            }
            return count;
        }

        static bool IsGenericArgument(string parameter)
        {
            return parameter.StartsWith(GenericParamaterPrefix);
        }

        static void BuildTypeLookup()
        {
            if (_nameToType != null) return;
            _nameToType = new Dictionary<string, Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.FullName != null)
                            _nameToType[type.FullName] = type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine("Could not load types of assembly '{0}'.{1}{2}",
                        assembly.FullName, Environment.NewLine, ex.InnerException);
                }
            }
        }

        public static IEnumerable<string> ExtractMethodArgumentTypes(string methodParameters)
        {
            int startPosition = 0;
            while (startPosition < methodParameters.Length)
            {
                int positionOfInterestingChar = methodParameters.IndexOfAny(new[] { StartGenericArguments, ',' }, startPosition);
                if (positionOfInterestingChar < 0)
                {
                    if (startPosition == 0)
                    {
                        yield return methodParameters.TrimEnd(RefoutParameterSuffix);
                    }
                    else
                    {
                        yield return methodParameters.Substring(startPosition).TrimEnd(RefoutParameterSuffix);
                    }
                    startPosition = methodParameters.Length;
                }
                else
                {
                    if (methodParameters[positionOfInterestingChar] == StartGenericArguments)
                    {
                        //Generic parameter 
                        positionOfInterestingChar = IndexAfterGenericArguments(methodParameters, positionOfInterestingChar);
                    }
                    yield return methodParameters.Substring(startPosition, positionOfInterestingChar - startPosition).TrimEnd(RefoutParameterSuffix);
                    startPosition = positionOfInterestingChar + 1;
                }
            }
        }

        static int IndexAfterGenericArguments(string parameterList, int startPosition)
        {
            // - may contain ',' for multiple generic arguments for the single parameter type: IDictionary<KEY,VALUE>
            // - may contain '{' for generics of generics: IEnumerable<Nullable<int>>
            int genericNesting = 1;
            while (genericNesting > 0)
            {
                startPosition = parameterList.IndexOfAny(new[] { StartGenericArguments, EndGenericArguments }, startPosition + 1);
                genericNesting += (parameterList[startPosition] == StartGenericArguments) ? 1 : -1;
            }
            //position needs to be the index AFTER the complete parameter string
            startPosition = startPosition + 1;
            return startPosition;
        }
    }
}