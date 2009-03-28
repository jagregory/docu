using NUnit.Framework;

namespace Docu.Tests.Utils
{
    public class TestTemplateAttribute : TestAttribute
    {
        public TestTemplateAttribute(string content)
        {
            Content = content;
        }

        public string Content { get; private set; }
    }
}