using System;

// ReSharper disable EventNeverInvoked
// ReSharper disable InconsistentNaming
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

        public event EventHandler<EventArgs> AnEvent;
    }
}
// ReSharper restore InconsistentNaming
// ReSharper restore EventNeverInvoked