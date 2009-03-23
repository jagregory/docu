using System.IO;
using Docu.IO;
using NUnit.Framework;

namespace Docu.Tests.IO
{
    [TestFixture]
    public class XmlLoaderTests
    {
        private string xmlFile;

        [SetUp]
        public void create_dummy_xml_file()
        {
            destroy_dummy_xml_file();

            xmlFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            File.WriteAllText(xmlFile, "an xml file");
        }

        [TearDown]
        public void destroy_dummy_xml_file()
        {
            if (File.Exists(xmlFile))
                File.Delete(xmlFile);
        }

        [Test]
        public void should_load_xml_by_name()
        {
            new XmlLoader()
                .LoadFrom(xmlFile)
                .ShouldEqual("an xml file");
        }
    }
}
