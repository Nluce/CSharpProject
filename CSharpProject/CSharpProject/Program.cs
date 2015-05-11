using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpProject
{
    public interface ILoggable
    {
        void Log();
    }
    public interface ITestInterface
    {

        void SampleFunction();

        int SampleProperty
        {

            get;

            set;

        }
        
    }
    public class TestClass2 : ILoggable
    {
        public void Log()
        {
            Console.WriteLine("This is test class 2");
        }
    }
    public class TestClass : ITestInterface, ILoggable
    {
        private int testVariable = 80;
        public void SampleFunction()
        {
            Console.WriteLine("sample variable = " + testVariable);
        }
        public int SampleProperty
        {

            get
            {
                return testVariable;
            }

            set
            {
                Console.WriteLine("SETTEST");
                testVariable = value;
            }

        }
        public void Log()
        {

            Console.WriteLine("Type = " + this.GetType());
            var interfaces = this.GetType().GetInterfaces();
            foreach(var i in interfaces)
            {
                Console.WriteLine("Interfaces = " + i);
            }
            Console.WriteLine("This is test class 1");
        }
        static void Main(string[] args)
        {
            //test
            TestClass obj = new TestClass();
            obj.SampleFunction();
            obj.SampleProperty = 10;
            obj.SampleFunction();
            ILoggable tc2 = new TestClass2();
            ILoggable tc1 = obj;
            tc1.Log();
            tc2.Log();
            
            //hold window
            while (true) ;
        }
    }
}
   
