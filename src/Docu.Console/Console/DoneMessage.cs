namespace Docu.Console
{
    using System.Collections.Generic;

    /// <summary>
    /// A screen message that is displayed when the documentation generation is completed.
    /// </summary>
    public class DoneMessage : IScreenMessage
    {
        /// <summary>
        /// Accesses the content of the screen message
        /// </summary>
        /// <returns>
        /// The content of the message.
        /// </returns>
        public IEnumerable<string> GetBody()
        {
            yield return string.Empty;
            yield return "Generation complete";
        }
    }
}