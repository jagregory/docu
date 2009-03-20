using System;

namespace Docu.Parsing
{
    public class UnsupportedDocumentationMemberException : Exception
    {
        public UnsupportedDocumentationMemberException(string name)
            : base("Unsupported document member found: '" + name + "'")
        {
            MemberName = name;
        }

        public string MemberName { get; private set; }
    }
}