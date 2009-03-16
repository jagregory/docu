using System.Reflection;

namespace Docu.IO
{
    public interface IAssemblyLoader
    {
        Assembly LoadFrom(string assemblyPath);
    }
}