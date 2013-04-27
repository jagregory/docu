using System;
using System.Collections.Generic;
using System.Linq.Expressions;

// ReSharper disable EventNeverInvoked
// ReSharper disable InconsistentNaming
namespace Example
{
    public class EmptyType
    { }

    public class SingleMethodType
    {
        public void Method()
        { }
    }

    public class PropertyType
    {
        public string Property { get; set; }
    }

    public class ClassWithOverload
    {
        public void Method() { }
        public void Method(string one) { }

        public string MethodWithReturn()
        {
            return "";
        }

        public void MethodWithArray(string[] strings)
        {
            
        }
    }

    public class ClassWithInterfaces : EmptyInterface, IDisposable
    {
        public void Dispose()
        {
            
        }
    }

    public class ClassWithBaseWithInterfaces : ClassWithInterfaces
    {}

    public class ClassWithBaseWithInterfacesAndDirect : ClassWithInterfaces, IExample
    { }

    public interface IExample
    {
    }

    public class ClassWithExplicitMethodImplementation : InterfaceForExplicitImplementation
    {
        void InterfaceForExplicitImplementation.Method()
        {

        }
    }

    public interface EmptyInterface
    { }

    public interface SingleMethodInterface
    {
        void Method();
    }

    public interface InterfaceForExplicitImplementation
    {
        void Method();
    }

    public class StaticMethodClass
    {
        public static void Method()
        {}
    }

    public class EventTypeEx
    {
        public event EventHandler<EventArgs> AnEvent;
    }

    public class FieldType
    {
        public string aField;
    }

    public class ReturnMethodClass
    {
        public string Method()
        {
            return "";
        }
    }

    /// <summary>
    /// First summary
    /// </summary>
    public class First
    {
    }

    public class FirstChild : First
    {
    }

    public class GenericDefinition<T>
    {
        public void AMethod()
        {
            
        }

        public void BMethod<C>()
        {
            
        }
    }

    public class HasGenericMethods
    {
        public void Do<T>(T item)
        {
            
        }

        public void DoWithLookup<K,V>(IDictionary<K,V> lookup, K key)
        {
            
        }

        public void Evaluate<MODEL>(IDictionary<string, Expression<Func<MODEL, object>>> expressions, int maxCount)
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
// ReSharper restore InconsistentNaming
// ReSharper restore EventNeverInvoked