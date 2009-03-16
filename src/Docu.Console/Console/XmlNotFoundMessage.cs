using System.Collections.Generic;

namespace Docu.Console
{
    public class XmlNotFoundMessage : IScreenMessage
    {
        private readonly string xml;

        public XmlNotFoundMessage(string xml)
        {
            this.xml = xml;
        }

        public IEnumerable<string> GetBody()
        {
            yield return "XML file not found '" + xml + "': cannot continue.";
        }
    }
}