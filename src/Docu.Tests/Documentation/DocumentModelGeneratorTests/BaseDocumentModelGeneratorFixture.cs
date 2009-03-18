using System;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation;
using Docu.Parsing;
using Docu.Parsing.Model;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    public abstract class BaseDocumentModelGeneratorFixture : BaseFixture
    {
        protected DocumentModel model;

        [SetUp]
        public void CreateTransformer()
        {
            model = new DocumentModel(new CommentContentParser());
        }

        protected DocumentedType Type<T>(string xml)
        {
            return new DocumentedType(Identifier.FromType(typeof(T)), xml.ToNode(), typeof(T));
        }

        protected DocumentedType Type(Type type, string xml)
        {
            return new DocumentedType(Identifier.FromType(type), xml.ToNode(), type);
        }

        protected DocumentedMethod Method<T>(string xml, Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return new DocumentedMethod(Identifier.FromMethod(method, typeof(T)), xml.ToNode(), method, typeof(T));
        }

        protected DocumentedProperty Property<T>(string xml, Expression<Func<T, object>> propertyAction)
        {
            var property = ((MemberExpression)propertyAction.Body).Member as PropertyInfo;

            return new DocumentedProperty(Identifier.FromProperty(property, typeof(T)), xml.ToNode(), property, typeof(T));
        }
    }
}