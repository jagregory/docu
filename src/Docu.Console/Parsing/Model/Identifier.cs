using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Docu.Console;

namespace Docu.Parsing.Model
{
    public abstract class Identifier : IComparable<Identifier>, IEquatable<Identifier>
    {
        private const string GENERIC_PARAMATER_PREFIX = "``";
        private const string GENERIC_RANK_PREFIX = "`";
        private const string ARRAY_TYPE_SUFFIX = "[]";
        private static readonly string GENERIC_TYPE_NAMESPACE = string.Empty;
        private readonly string name;
        private static Dictionary<string, Type> nameToType;
        private static char START_GENERIC_ARGUMENTS = '{';
        private static char END_GENERIC_ARGUMENTS = '}';
        private static IScreenWriter screenWriter = new ConsoleScreenWriter();

        protected Identifier(string name)
        {
            this.name = name;
        }

        protected string Name
        {
            get { return name; }
        }

        public static TypeIdentifier FromType(Type type)
        {
            return type.IsGenericParameter
                ? new TypeIdentifier(GENERIC_PARAMATER_PREFIX + type.GenericParameterPosition, GENERIC_TYPE_NAMESPACE) 
                : new TypeIdentifier(type.Name, type.Namespace);
        }

        public static MethodIdentifier FromMethod(MethodInfo method, Type type)
        {
            string name = method.Name;
            var parameters = new List<TypeIdentifier>();

            if (method.IsGenericMethod)
                name += GENERIC_PARAMATER_PREFIX + method.GetGenericArguments().Length;

			try {
				foreach (ParameterInfo param in method.GetParameters()) {
					parameters.Add(FromType(param.ParameterType));
				}
			} catch (IOException ex) {
				return (MethodIdentifier.FromException(name, FromType(type), ex));
			}

        	return new MethodIdentifier(name, parameters.ToArray(), method.IsStatic, method.IsPublic, FromType(type));
        }

        public static PropertyIdentifier FromProperty(PropertyInfo property, Type type)
        {
            return new PropertyIdentifier(property.Name, property.CanRead, property.CanWrite, FromType(type));
        }

        public static FieldIdentifier FromField(FieldInfo field, Type type)
        {
            return new FieldIdentifier(field.Name, FromType(type));
        }

        public static EventIdentifier FromEvent(EventInfo ev, Type type)
        {
            return new EventIdentifier(ev.Name, FromType(type));
        }

        public static NamespaceIdentifier FromNamespace(string ns)
        {
            return new NamespaceIdentifier(ns);
        }

        public static Identifier FromString(string name)
        {
            var prefix = name[0];
            var trimmedName = name.Substring(2);

            if (prefix == 'T')
                return FromTypeString(trimmedName);
            if (prefix == 'N')
                return FromNamespace(trimmedName);
            if (prefix == 'M')
                return FromMethodName(trimmedName);
            if (prefix == 'P')
                return FromPropertyName(trimmedName);
            if (prefix == 'E')
                return FromEventName(trimmedName);
            if (prefix == 'F')
                return FromFieldName(trimmedName);

            throw new UnsupportedDocumentationMemberException(name);
        }

        private static PropertyIdentifier FromPropertyName(string name)
        {
            string typeName = GetTypeName(name);
            string propertyName = GetMethodName(name);

            return new PropertyIdentifier(propertyName, false, false, FromTypeString(typeName));
        }

        private static EventIdentifier FromEventName(string name)
        {
            var typeName = GetTypeName(name);
            var eventName = GetMethodName(name);

            return new EventIdentifier(eventName, FromTypeString(typeName));
        }

        private static FieldIdentifier FromFieldName(string name)
        {
            var typeName = GetTypeName(name);
            var fieldName = GetMethodName(name);

            return new FieldIdentifier(fieldName, FromTypeString(typeName));
        }

        private static MethodIdentifier FromMethodName(string name)
        {
            string typeName = GetTypeName(name);
            string methodName = GetMethodName(name);
            List<TypeIdentifier> parameters = GetMethodParameters(name);

            return new MethodIdentifier(methodName, parameters.ToArray(), false, false, FromTypeString(typeName));
        }

        private static TypeIdentifier FromTypeString(string name)
        {
            string ns;
            string type;

            if (name.Contains("."))
            {
                // has namespace
                ns = name.Substring(0, name.LastIndexOf('.'));
                type = name.Substring(name.LastIndexOf('.') + 1);
            }
            else
            {
                // special case where no namespace is used
                ns = "Unknown";
                type = name;
            }
            

            return new TypeIdentifier(type, ns);
        }

        private static string GetTypeName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(0, name.LastIndexOf("."));
        }

        private static string GetMethodName(string fullName)
        {
            string name = fullName;

            if (name.EndsWith(")"))
            {
                // has parameters, so strip them off
                name = name.Substring(0, name.IndexOf("("));
            }

            return name.Substring(name.LastIndexOf(".") + 1);
        }

        private static List<TypeIdentifier> GetMethodParameters(string fullName)
        {
            var parameters = new List<TypeIdentifier>();
            if (!fullName.EndsWith(")")) return parameters;

            buildTypeLookup();

            var firstCharAfterParen = fullName.IndexOf("(") + 1;
            var paramList = fullName.Substring(firstCharAfterParen, fullName.Length - firstCharAfterParen - 1) ;

            foreach (var paramName in ParseMethodParameterList(paramList))
            {
                if (IsGenericArgument(paramName))
                {
                    parameters.Add(new TypeIdentifier(paramName, GENERIC_TYPE_NAMESPACE));
                    continue;
                }
                var typeNameToFind = paramName;
                var startOfGenericArguments = paramName.IndexOf(START_GENERIC_ARGUMENTS);
                if (startOfGenericArguments > 0)
                {
                    var nonGenericPartOfTypeName = paramName.Substring(0, startOfGenericArguments);
                    var endOfGenericArguments = paramName.LastIndexOf(END_GENERIC_ARGUMENTS);
                    var lengthOfGenericArgumentsSection = endOfGenericArguments - startOfGenericArguments - 1;
                    var genericArgumentsSection = paramName.Substring(startOfGenericArguments + 1, lengthOfGenericArgumentsSection);
                    var countOfGenericParametersForType = countOfGenericArguments(genericArgumentsSection);
                    typeNameToFind = nonGenericPartOfTypeName + GENERIC_RANK_PREFIX + countOfGenericParametersForType;
                }
                Type paramType;
                var isArray = typeNameToFind.EndsWith(ARRAY_TYPE_SUFFIX);
                if (isArray) typeNameToFind = typeNameToFind.Substring(0, typeNameToFind.Length - 2);
                if (nameToType.TryGetValue(typeNameToFind, out paramType))
                {
                    if (isArray) paramType = paramType.MakeArrayType();
                    parameters.Add(FromType(paramType));
                }
            }

            return parameters;
        }

        private static int countOfGenericArguments(string genericArguments)
        {
            var count = 1;
            var startPosition = 0;
            while (startPosition < genericArguments.Length)
            {
                var positionOfInterestingChar = genericArguments.IndexOfAny(new[] {START_GENERIC_ARGUMENTS, ','}, startPosition);
                if (positionOfInterestingChar < 0)
                {
                    return count;
                }
                if (genericArguments[positionOfInterestingChar] == START_GENERIC_ARGUMENTS)
                {
                    startPosition = indexAfterGenericArguments(genericArguments, positionOfInterestingChar);
                }
                else
                {
                    ++count;
                    startPosition = positionOfInterestingChar + 1;
                }
            }
            return count;
        }

        private static bool IsGenericArgument(string parameter)
        {
            return parameter.StartsWith(GENERIC_PARAMATER_PREFIX);
        }

        private static void buildTypeLookup()
        {
            if (nameToType != null) return;
            nameToType = new Dictionary<string, Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.FullName != null)
                            nameToType[type.FullName] = type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    screenWriter.WriteLine(string.Format("Could not load types of assembly '{0}'.{1}{2}",
                                                         assembly.FullName, Environment.NewLine, ex.InnerException));
                }
            }
        }

        public static IEnumerable<string> ParseMethodParameterList(string methodParameters)
        {
            var startPosition = 0;
            while (startPosition < methodParameters.Length)
            {
                var positionOfInterestingChar = methodParameters.IndexOfAny(new[] {START_GENERIC_ARGUMENTS, ','}, startPosition);
                if (positionOfInterestingChar < 0)
                {
                    if (startPosition == 0)
                    {
                        yield return methodParameters;
                    }
                    else
                    {
                        yield return methodParameters.Substring(startPosition);
                    }
                    startPosition = methodParameters.Length;
                }
                else
                {
                    if (methodParameters[positionOfInterestingChar] == START_GENERIC_ARGUMENTS)
                    {
                        //Generic parameter 
                        positionOfInterestingChar = indexAfterGenericArguments(methodParameters, positionOfInterestingChar);
                    }
                    yield return methodParameters.Substring(startPosition, positionOfInterestingChar - startPosition);
                    startPosition = positionOfInterestingChar + 1;
                }

            }
        }

        private static int indexAfterGenericArguments(string parameterList, int startPosition)
        {
            // - may contain ',' for multiple generic arguments for the single parameter type: IDictionary<KEY,VALUE>
            // - may contain '{' for generics of generics: IEnumerable<Nullable<int>>
            var genericNesting = 1;
            while (genericNesting > 0)
            {
                startPosition = parameterList.IndexOfAny(new[] { START_GENERIC_ARGUMENTS, END_GENERIC_ARGUMENTS }, startPosition + 1);
                genericNesting += (parameterList[startPosition] == START_GENERIC_ARGUMENTS) ? 1 : -1;
            }
            //position needs to be the index AFTER the complete parameter string
            startPosition = startPosition + 1;
            return startPosition;
        }

        public abstract NamespaceIdentifier CloneAsNamespace();
        public abstract TypeIdentifier CloneAsType();

        public static bool operator ==(Identifier first, Identifier second)
        {
            return (((object)first) != null) && first.Equals(second);
        }

        public static bool operator !=(Identifier first, Identifier second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identifier);
        }

        public abstract bool Equals(Identifier obj);

        public abstract int CompareTo(Identifier other);

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return name; // HACK
        }
    }
}