namespace Docu.Console
{
    using System.Collections.Generic;

    public class WarningMessage : IScreenMessage
    {
        private readonly string message;

        public WarningMessage(string message)
        {
            this.message = message;
        }

        public IEnumerable<string> GetBody()
        {
            yield return "WARNING: " + message;
        }
    }
}