using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docu.Documentation;
using Docu.Parsing.Model;
using Docu.UI;
using Example;
using NUnit.Framework;

namespace Docu.Tests.UI
{
    [TestFixture]
    public class HtmlOutputFormatterTests
    {
        [Test]
        public void OutputsTypeReferenceLink()
        {
            var formatter = new HtmlOutputFormatter();
            var type = Type<First>();

            formatter.FormatReferencable(type)
                .ShouldEqual("<a href=\"Example/First.htm\">First</a>");
        }

        [Test]
        public void OutputsMethodReferenceLink()
        {
            var formatter = new HtmlOutputFormatter();
            var method = Method<Second>("SecondMethod");

            formatter.FormatReferencable(method)
                .ShouldEqual("<a href=\"Example/Second.htm#SecondMethod\">SecondMethod</a>");
        }

        [Test]
        public void OutputsPropertyReferenceLink()
        {
            var formatter = new HtmlOutputFormatter();
            var method = Property<Second>("SecondProperty");

            formatter.FormatReferencable(method)
                .ShouldEqual("<a href=\"Example/Second.htm#SecondProperty\">SecondProperty</a>");
        }

        [Test]
        public void OutputsFieldReferenceLink()
        {
            var formatter = new HtmlOutputFormatter();
            var method = Field<Second>("aField");

            formatter.FormatReferencable(method)
                .ShouldEqual("<a href=\"Example/Second.htm#aField\">aField</a>");
        }

        [Test]
        public void OutputsEventReferenceLink()
        {
            var formatter = new HtmlOutputFormatter();
            var method = Event<Second>("AnEvent");

            formatter.FormatReferencable(method)
                .ShouldEqual("<a href=\"Example/Second.htm#AnEvent\">AnEvent</a>");
        }

        private DeclaredType Type<T>()
        {
            return new DeclaredType(Identifier.FromType(typeof(T)), Namespace.Unresolved(Identifier.FromNamespace(typeof(T).Namespace)));
        }

        private Method Method<T>(string name)
        {
            return new Method(Identifier.FromMethod(typeof(T).GetMethod(name), typeof(T)), Type<T>());
        }

        private Property Property<T>(string name)
        {
            return new Property(Identifier.FromProperty(typeof(T).GetProperty(name), typeof(T)), Type<T>());
        }

        private Field Field<T>(string name)
        {
            return new Field(Identifier.FromField(typeof(T).GetField(name), typeof(T)), Type<T>());
        }

        private Event Event<T>(string name)
        {
            return new Event(Identifier.FromEvent(typeof(T).GetEvent(name), typeof(T)), Type<T>());
        }
    }
}
