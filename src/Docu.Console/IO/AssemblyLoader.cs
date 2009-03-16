using System.Reflection;

namespace Docu.IO
{
    public class AssemblyLoader : IAssemblyLoader
    {
        public Assembly LoadFrom(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }
    }
}