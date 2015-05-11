using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpProject
{
    public interface ITestInterface
    {

        void SampleFunction();

        /*int SampleProperty
        {

            get;

            set;

        }
        */
    }
    public class TestClass : ITestInterface
    {
        private int testVariable = 80;
        public void SampleFunction()
        {
            Console.WriteLine("sample variable = " + testVariable);
        }
        static void Main(string[] args)
        {
            ITestInterface obj = new TestClass();
            obj.SampleFunction();

            //hold window
            while (true) ;
        }
    }
}
   
