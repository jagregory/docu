using System.Text.RegularExpressions;

namespace Docu.Parsing.Comments
{
    internal abstract class CommentParserBase
    {
        protected string PrepareText(string text)
        {
            var regexp = new Regex(@"[\s]{0,}\r\n[\s]{0,}");

            return regexp.Replace(text, "\r\n");;
        }
    }
}