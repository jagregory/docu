using System.IO;

namespace Docu.IO
{
    public class XmlLoader : IXmlLoader
    {
        public string LoadFrom(string xmlFileName)
        {
            return File.ReadAllText(xmlFileName);
        }
    }
}