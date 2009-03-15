using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests.Parsing
{
    [TestFixture]
    public class CommentContentParserTests
    {
        private CommentContentParser parser;

        [SetUp]
        public void CreateParser()
        {
            parser = new CommentContentParser();
        }

        [Test]
        public void ShouldParseSimple()
        {
            var blocks = parser.Parse("<x>Hello world!</x>".ToNode());

            blocks.CountShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseSimpleTrimLeadingWhitespace()
        {
            var blocks = parser.Parse("<x>  Hello world!</x>".ToNode());

            blocks.CountShouldEqual(1);
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

            blocks.CountShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseCodeInline()
        {
            var blocks = parser.Parse("<x><c>code</c></x>".ToNode());

            blocks.CountShouldEqual(1);
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

            blocks.CountShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineCode>();
            ((InlineCode)blocks[0]).Text.ShouldEqual("Some multiline\r\ncode");
        }

        [Test]
        public void ShouldParseSee()
        {
            var blocks = parser.Parse("<x><see cref=\"T:Example.First\" /></x>".ToNode());

            blocks.CountShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.ShouldBeOfType<UnresolvedReference>();
            ((See)blocks[0]).Reference.Name.ShouldEqual("First");
        }

        [Test]
        public void ShouldParsePara()
        {
            var blocks = parser.Parse("<x><para>some text</para></x>".ToNode());

            blocks.CountShouldEqual(1);
            blocks[0].ShouldBeOfType<Paragraph>();
            ((Paragraph)blocks[0]).Text.ShouldEqual("some text");
        }
    }
}