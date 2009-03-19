using System;
using System.IO;
using Docu.IO;
using Docu.IO;
using NUnit.Framework;

namespace Docu.Tests.IO
{
    [TestFixture]
    public class AssemblyLoaderTests
    {
        [Test]
        // hard to test this without shipping an unloaded assembly with the tests
        public void should_load_assembly_by_name()
        {
            var assembly = typeof(DocumentationGenerator).Assembly;

            new AssemblyLoader()
                .LoadFrom(assembly.Location)
                .ShouldEqual(assembly);
        }

        [Test]
        // hard to test this without shipping an unloaded assembly with the tests
        public void should_fire_assmbly_load_failed_event_if_assembly_isnt_found()
        {
            var assemblyCouldntBeLoaded = false;
            var assembly = typeof(DocumentationGenerator).Assembly;

            var loader = new AssemblyLoader();

            try
            {
                loader.LoadFrom(assembly.Location + "kjdkjdhsfjksahk"); //Just garbage :)
            }
            catch (FileNotFoundException err)
            {
                assemblyCouldntBeLoaded = true;
            }
            
            assemblyCouldntBeLoaded.ShouldBeTrue();
        }
    }
}
