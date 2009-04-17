using System.Text.RegularExpressions;

namespace Docu.Parsing.Comments
{
    internal abstract class CommentParserBase
    {
        protected string PrepareText(string text, bool first, bool last)
        {
            var regexp = new Regex(@"[\s]{0,}\r\n[\s]{0,}");

            string prepared = regexp.Replace(text, "\r\n");

            if(first)
                prepared = prepared.TrimStart(' ', '\t', '\r', '\n');
            else
                prepared = prepared.TrimStart('\t', '\r', '\n');

            if(last)
                prepared = prepared.TrimEnd(' ', '\t', '\r', '\n');
            else
                prepared = prepared.TrimEnd('\t', '\r', '\n');

            return prepared;
        }
    }
}