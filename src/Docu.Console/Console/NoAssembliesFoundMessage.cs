namespace Docu.Console
{
    using System.Collections.Generic;

    public class NoAssembliesFoundMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "No Assemblies specified: please give me some assemblies!";
        }
    }
}