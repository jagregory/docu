using Docu.Documentation;
using Docu.Documentation.Comments;
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
        public void OutputsParameterReferencesUsingTheVarTag()
        {
            var formatter = new HtmlOutputFormatter();
            formatter.Format(new ParameterReference("myParam")).ShouldEqual("<var>myParam</var>");
        }

        [Test]
        public void OutputsDefinitionListUsingDLTag()
        {
            var formatter = new HtmlOutputFormatter();
            var list = new DefinitionList();
            list.Items.Add(listItem("a", "first"));
            list.Items.Add(listItem("b", "second"));
            formatter.Format(list).ShouldEqual("<dl><dt>a</dt><dd>first</dd><dt>b</dt><dd>second</dd></dl>");
        }

        [Test]
        public void OutputsNumberListUsingOLTag()
        {
            var formatter = new HtmlOutputFormatter();
            var list = new NumberList();
            list.Items.Add(listItem("a"));
            list.Items.Add(listItem("b"));
            formatter.Format(list).ShouldEqual("<ol><li>a</li><li>b</li></ol>");
        }

        [Test]
        public void OutputsBulletListUsingULTag()
        {
            var formatter = new HtmlOutputFormatter();
            var list = new BulletList();
            list.Items.Add(listItem("a"));
            list.Items.Add(listItem("b"));
            formatter.Format(list).ShouldEqual("<ul><li>a</li><li>b</li></ul>");
        }

        [Test]
        public void OutputsTableListUsingTABLETag()
        {
            var formatter = new HtmlOutputFormatter();
            var list = new TableList();
            list.Items.Add(listItem("a", "first"));
            list.Items.Add(listItem("b", "second"));
            formatter.Format(list).ShouldEqual("<table><tr><td>a</td><td>first</td></tr><tr><td>b</td><td>second</td></tr></table>");
        }

        private InlineListItem listItem(string definition)
        {
            return new InlineListItem(null, paragraph(definition));
        }


        private InlineListItem listItem(string term, string definition)
        {
            return new InlineListItem(paragraph(term), paragraph(definition));
        }
        private Paragraph paragraph(string text)
        {
            return new Paragraph(new[]{new InlineText(text)});
        }

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
