using System.Collections.Generic;

namespace DrDoc.Console
{
    public class AssemblyNotFoundMessage : IScreenMessage
    {
        private readonly string assembly;

        public AssemblyNotFoundMessage(string assembly)
        {
            this.assembly = assembly;
        }

        public IEnumerable<string> GetBody()
        {
            yield return "Assembly not found '" + assembly + "': cannot continue.";
        }
    }
}