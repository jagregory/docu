namespace Docu.Console
{
    using System.Collections.Generic;

    public class NoXmlsFoundMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "No XML documents found: none were found for the assembly names you gave,";
            yield return "explicitly specify them if their names differ from their associated assembly.";
        }
    }
}