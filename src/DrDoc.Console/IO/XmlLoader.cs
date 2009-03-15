using System;
using System.IO;
using DrDoc.IO;

namespace DrDoc.IO
{
    public class XmlLoader : IXmlLoader
    {
        public string LoadFrom(string xmlFileName)
        {
            return File.ReadAllText(xmlFileName);
        }
    }
}