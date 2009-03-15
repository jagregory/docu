using System.Reflection;

namespace DrDoc.IO
{
    public interface IAssemblyLoader
    {
        Assembly LoadFrom(string assemblyPath);
    }
}