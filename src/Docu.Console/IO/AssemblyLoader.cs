using System;
using System.IO;
using System.Reflection;

namespace Docu.IO
{
    public class AssemblyLoader : IAssemblyLoader
    {
        public Assembly LoadFrom(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                try
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                catch (BadImageFormatException err)
                {
                    var exception = new BadImageFormatException("This file is in a bad format and will not be loaded.", err);
                    throw exception;
                }
            }
            throw new FileNotFoundException(string.Format("The file could not be found at {0}", assemblyPath));
        }
    }
}