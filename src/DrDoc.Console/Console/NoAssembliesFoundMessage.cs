using System.Collections.Generic;

namespace DrDoc.Console
{
    public class NoAssembliesFoundMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "No Assemblies specified: please give me some assemblies!";
        }
    }
}