namespace Docu.Console
{
    using System.Collections.Generic;

    /// <summary>
    /// A screen message that is displayed if an assembly can not be found.
    /// </summary>
    public class AssemblyNotFoundMessage : IScreenMessage
    {
        /// <summary>
        /// The name of the assembly that can not be loaded.
        /// </summary>
        private readonly string assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyNotFoundMessage"/> class.
        /// </summary>
        /// <param name="assembly">
        /// The name of the assembly that can not be loaded.
        /// </param>
        public AssemblyNotFoundMessage(string assembly)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// Accesses the content of the screen message
        /// </summary>
        /// <returns>
        /// The content of the message.
        /// </returns>
        public IEnumerable<string> GetBody()
        {
            yield return "Assembly not found '" + this.assembly + "': cannot continue.";
        }
    }
}