using System;
using Docu.Tests.Utils;

namespace Example
{
    public class Second
    {
        public string aField;
        public void SecondMethod()
        {
            
        }

        public void SecondMethod2(string one, int two)
        {
            
        }

        public void SecondMethod3(string one, First two)
        {

        }

        public string ReturnType()
        {
            return "";
        }

        public string SecondProperty { get; set; }

        [Custom]
        public string SecondProperty2 { get; set; }

        public event EventHandler<EventArgs> AnEvent;
    }
}
