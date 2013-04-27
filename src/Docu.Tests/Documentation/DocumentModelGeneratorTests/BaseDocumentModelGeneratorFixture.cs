using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    public abstract class BaseDocumentModelGeneratorFixture : BaseFixture
    {
        protected ICommentParser StubParser;
        public ICommentParser RealParser { get; private set; }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            RealParser = new CommentParser(new ICommentNodeParser[]
                {
                    new InlineCodeCommentParser(),
                    new InlineListCommentParser(),
                    new InlineTextCommentParser(),
                    new MultilineCodeCommentParser(),
                    new ParagraphCommentParser(),
                    new ParameterReferenceParser(),
                    new SeeCodeCommentParser(),
                });
        }

        [SetUp]
        public void CreateStubs()
        {
            StubParser = MockRepository.GenerateStub<ICommentParser>();
            StubParser.Stub(x => x.Parse(null))
                      .IgnoreArguments()
                      .Return(new List<Comment>());
        }

        protected DocumentedType Type<T>(string xml)
        {
            return new DocumentedType(IdentifierFor.Type(typeof(T)), xml.ToNode(), typeof(T));
        }

        protected DocumentedType Type(Type type, string xml)
        {
            return new DocumentedType(IdentifierFor.Type(type), xml.ToNode(), type);
        }

        protected DocumentedMethod Method<T>(string xml, Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression) methodAction.Body).Method;
            return new DocumentedMethod(IdentifierFor.Method(method, typeof(T)), xml.ToNode(), method, typeof(T));
        }

        protected DocumentedProperty Property<T>(string xml, Expression<Func<T, object>> propertyAction)
        {
            var property = ((MemberExpression) propertyAction.Body).Member as PropertyInfo;
            return new DocumentedProperty(IdentifierFor.Property(property, typeof(T)), xml.ToNode(), property, typeof(T));
        }

        protected DocumentedField Field<T>(string xml, Expression<Func<T, object>> fieldAction)
        {
            var field = ((MemberExpression) fieldAction.Body).Member as FieldInfo;
            return new DocumentedField(IdentifierFor.Field(field, typeof(T)), xml.ToNode(), field, typeof(T));
        }

        protected DocumentedEvent Event<T>(string xml, string eventName)
        {
            var ev = typeof (T).GetEvent(eventName);
            return new DocumentedEvent(IdentifierFor.Event(ev, typeof(T)), xml.ToNode(), ev, typeof(T));
        }
    }
}
