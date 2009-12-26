using System;
using Rhino.Mocks;

namespace Docu.Tests.Utils
{
    public static class Stub
    {
        public static T Create<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

        public static T Create<T>(Action<T> setup) where T : class
        {
            var stub = Create<T>();

            setup(stub);

            return stub;
        }
    }
}