using System;

namespace Docu.Tests.Utils
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CustomAttribute : Attribute
    {
    }
}
