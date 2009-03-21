using System.Collections.Generic;

namespace Docu.Console
{
    public class InvalidArgumentMessage : IScreenMessage
    {
        private readonly string argument;

        public InvalidArgumentMessage(string argument)
        {
            this.argument = argument;
        }

        public IEnumerable<string> GetBody()
        {
            yield return "Invalid argument '" + argument + "': what am I supposed to do with this?";
            yield return "";
            yield return "Use --help to see usage and switches";
        }
    }
}