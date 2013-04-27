namespace Docu.Console
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface for a displayable message that allows to access the message content.
    /// </summary>
    public interface IScreenMessage
    {
        /// <summary>
        /// Accesses the content of the screen message
        /// </summary>
        /// <returns>
        /// The content of the message.
        /// </returns>
        IEnumerable<string> GetBody();
    }
}