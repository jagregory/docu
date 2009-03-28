using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using NUnit.Framework;

namespace Docu.Tests.Parsing
{
    [TestFixture]
    public class CommentParserTests
    {
        private CommentParser parser;

        [SetUp]
        public void CreateParser()
        {
            parser = new CommentParser();
        }

        [Test]
        public void ShouldParseSimple()
        {
            var blocks = parser.Parse("<x>Hello world!</x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseSimpleTrimLeadingWhitespace()
        {
            var blocks = parser.Parse("<x>  Hello world!</x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseSimpleTrimLeadingWhitespaceNewlines()
        {
            var blocks = parser.Parse(@"
            <x>
                Hello world!
            </x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseCodeInline()
        {
            var blocks = parser.Parse("<x><c>code</c></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineCode>();
            ((InlineCode)blocks[0]).Text.ShouldEqual("code");
        }

        [Test]
        public void ShouldParseCodeMultiline()
        {
            var blocks = parser.Parse(@"
<x>
    <code>
        Some multiline
        code
    </code>
</x>        ".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineCode>();
            ((InlineCode)blocks[0]).Text.ShouldEqual("Some multiline\r\ncode");
        }

        [Test]
        public void ShouldParseSeeForNamespace()
        {
            var blocks = parser.Parse("<x><see cref=\"N:Example\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("Example");
        }

        [Test]
        public void ShouldParseSeeForType()
        {
            var blocks = parser.Parse("<x><see cref=\"T:Example.First\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("First");
        }

        [Test]
        public void ShouldParseSeeForMethod()
        {
            var blocks = parser.Parse("<x><see cref=\"M:Example.Second.SecondMethod\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("SecondMethod");
        }

        [Test]
        public void ShouldParseSeeForProperty()
        {
            var blocks = parser.Parse("<x><see cref=\"P:Example.Second.SecondProperty\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("SecondProperty");
        }

        [Test]
        public void ShouldParseSeeForEvent()
        {
            var blocks = parser.Parse("<x><see cref=\"E:Example.Second.AnEvent\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("AnEvent");
        }

        [Test]
        public void ShouldParseSeeForField()
        {
            var blocks = parser.Parse("<x><see cref=\"F:Example.Second.aField\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("aField");
        }

        [Test]
        public void ShouldParseSeeNestedInPara()
        {
            var blocks = parser.Parse("<x><para><see cref=\"N:Example\" /></para></x>".ToNode());

            blocks.Count.ShouldEqual(1);

            var para = blocks.First() as Paragraph;
            var see = para.Children.First() as See;

            see.Reference.IsResolved.ShouldBeFalse();
            see.Reference.Name.ShouldEqual("Example");
        }

        [Test]
        public void ShouldParsePara()
        {
            var blocks = parser.Parse("<x><para>some text</para></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks.First().ShouldBeOfType<Paragraph>();

            var para = (Paragraph)blocks.First();

            para.Children.First().ShouldBeOfType<InlineText>();

            var text = (InlineText)para.Children.First();

            text.Text.ShouldEqual("some text");
        }

        [Test]
        public void ShouldParseNestedPara()
        {
            var blocks = parser.Parse("<x><para>some text <para>some more text</para></para></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks.First().ShouldBeOfType<Paragraph>();

            var para = (Paragraph)blocks.First();
            var paraText = para.Children.First() as InlineText;
            var childPara = (Paragraph)para.Children.Second();
            var childParaText = childPara.Children.First() as InlineText;

            paraText.Text.ShouldEqual("some text");
            childParaText.Text.ShouldEqual("some more text");
        }
    }
}