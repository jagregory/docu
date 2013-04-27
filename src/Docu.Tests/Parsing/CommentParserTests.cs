using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using NUnit.Framework;
using System.Linq;

namespace Docu.Tests.Parsing
{
    [TestFixture]
    public class CommentParserTests
    {
        private CommentParser parser;

        [SetUp]
        public void CreateParser()
        {
            parser = new CommentParser(new ICommentNodeParser[]
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

        [Test]
        public void ShouldParseSimple()
        {
            var blocks = parser.ParseNode("<x>Hello world!</x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseSimpleTrimLeadingWhitespace()
        {
            var blocks = parser.ParseNode("<x>  Hello world!</x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineText>();
            ((InlineText)blocks[0]).Text.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldParseSimpleTrimLeadingWhitespaceNewlines()
        {
            var blocks = parser.ParseNode(@"
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
            var blocks = parser.ParseNode("<x><c>code</c></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<InlineCode>();
            ((InlineCode)blocks[0]).Text.ShouldEqual("code");
        }

        [Test]
        public void ShouldParseCodeMultiline()
        {
            var blocks = parser.ParseNode(@"
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
            var blocks = parser.ParseNode("<x><see cref=\"N:Example\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("Example");
        }

        [Test]
        public void ShouldParseSeeForType()
        {
            var blocks = parser.ParseNode("<x><see cref=\"T:Example.First\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("First");
        }

        [Test]
        public void ShouldParseSeeForMethod()
        {
            var blocks = parser.ParseNode("<x><see cref=\"M:Example.Second.SecondMethod\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("SecondMethod");
        }

        [Test]
        public void ShouldParseSeeForProperty()
        {
            var blocks = parser.ParseNode("<x><see cref=\"P:Example.Second.SecondProperty\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("SecondProperty");
        }

        [Test]
        public void ShouldParseSeeForEvent()
        {
            var blocks = parser.ParseNode("<x><see cref=\"E:Example.Second.AnEvent\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("AnEvent");
        }

        [Test]
        public void ShouldParseSeeForField()
        {
            var blocks = parser.ParseNode("<x><see cref=\"F:Example.Second.aField\" /></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            blocks[0].ShouldBeOfType<See>();
            ((See)blocks[0]).Reference.IsResolved.ShouldBeFalse();
            ((See)blocks[0]).Reference.Name.ShouldEqual("aField");
        }

        [Test]
        public void ShouldParseSeeNestedInPara()
        {
            var blocks = parser.ParseNode("<x><para><see cref=\"N:Example\" /></para></x>".ToNode());

            blocks.Count.ShouldEqual(1);

            var para = blocks.First() as Paragraph;
            var see = para.Children.First() as See;

            see.Reference.IsResolved.ShouldBeFalse();
            see.Reference.Name.ShouldEqual("Example");
        }

        [Test]
        public void should_parse_single_para_as_paragraph()
        {
            var blocks = parser.ParseNode("<x><para>some text</para></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            var para = blocks.First().ShouldBeOfType<Paragraph>();
            para.Children.First().ShouldBeOfType<InlineText>().Text.ShouldEqual("some text");
        }

        [Test]
        public void should_parse_nested_para_as_paragraphs()
        {
            var blocks = parser.ParseNode("<x><para>some text <para>some more text</para></para></x>".ToNode());

            blocks.Count.ShouldEqual(1);
            var para = blocks.First().ShouldBeOfType<Paragraph>();
            // (cdrnet, 2009-04-17) the white space in the first test should not be stripped.
            // Maybe a nested <see>, <b> or <a> tag would be a better example here.
            //para.Text.ShouldEqual("some text ");

            var childPara = para.Children.Second().ShouldBeOfType<Paragraph>();
            childPara.Children.First().ShouldBeOfType<InlineText>().Text.ShouldEqual("some more text");
        }

        [Test]
        public void ShouldParseParamref()
        {
            var blocks = parser.ParseNode("<x>Returns the <paramref name=\"inputString\" /></x>".ToNode());
            blocks.Count.ShouldEqual(2);
            blocks[1].ShouldBeOfType<ParameterReference>();
            var paramRef = (ParameterReference)blocks[1];
            paramRef.Parameter.ShouldEqual("inputString");
        }

        [Test]
        public void should_default_to_definition_list_if_no_type_specified()
        {
            var blocks = parser.ParseNode("<x><list><item><term>IV</term><description>Four</description></item></list></x>".ToNode());
            blocks.First().ShouldBeOfType<DefinitionList>();
        }

        [Test]
        public void should_parse_bulleted_list_type()
        {
            var blocks = parser.ParseNode("<x><list type=\"bullet\"><item><description>Four</description></item></list></x>".ToNode());
            blocks.First().ShouldBeOfType<BulletList>();
        }

        [Test]
        public void should_parse_numbered_list_type()
        {
            var blocks = parser.ParseNode("<x><list type=\"number\"><item><description>Four</description></item></list></x>".ToNode());
            blocks.First().ShouldBeOfType<NumberList>();
        }

        [Test]
        public void should_parse_table_list_type()
        {
            var blocks = parser.ParseNode("<x><list type=\"table\"><item><description>Four</description></item></list></x>".ToNode());
            blocks.First().ShouldBeOfType<TableList>();
        }

        [Test]
        public void should_parse_definition_list_with_multiple_items()
        {
            var blocks = parser.ParseNode("<x>See <list type=\"definition\"><item><term>IV</term><description>Four</description></item><item><term>IX</term><description>Nine</description></item></list></x>".ToNode());
            blocks.Count.ShouldEqual(2);
            blocks[1].ShouldBeOfType<DefinitionList>();
            var list = (InlineList)blocks[1];
            list.Items.Count.ShouldEqual(2);
            list.Items[0].Term.Children.First().ShouldBeOfType<InlineText>().Text.ShouldEqual("IV");
            list.Items[0].Definition.Children.First().ShouldBeOfType<InlineText>().Text.ShouldEqual("Four");
        }

        [Test]
        public void should_parse_definition_list_with_nested_comments()
        {
            var blocks = parser.ParseNode("<x>See <list type=\"definition\"><item><term>IV</term><description>Four <see cref=\"N:Example\" /></description></item></list></x>".ToNode());
            blocks.Count.ShouldEqual(2);
            blocks[1].ShouldBeOfType<DefinitionList>();
            var list = (InlineList)blocks[1];
            list.Items[0].Definition.Children.First().ShouldBeOfType<InlineText>().Text.ShouldEqual("Four ");
            list.Items[0].Definition.Children.Skip(1).First().ShouldBeOfType<See>().Reference.Name.ShouldEqual("Example");
        }
    }

}