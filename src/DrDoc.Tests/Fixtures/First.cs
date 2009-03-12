using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Example
{
    /// <summary>
    /// First summary
    /// </summary>
    public class First
    {
    }

    public class GenericDefinition<T>
    {
        public void AMethod()
        {
            
        }

        public void AMethod<C>()
        {
            
        }
    }

    public class GenericDefinition<T,C>
    {
        
    }

    namespace Deep
    {
        public class DeepFirst
        {
            
        }
    }
}
