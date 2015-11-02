using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    /// <summary>
    /// This is a public class
    /// </summary>
    public class Class1
    {

        /// <summary>
        /// My property
        /// </summary>
        public int Prop1 { get; set; }


        /// <summary>
        /// A public method
        /// </summary>
        public void Foo()
        {

        }

        /// <summary>
        /// A public method with a return type
        /// </summary>
        /// <returns>A number</returns>
        public int Baz()
        {
            return 0;
        }

        /// <summary>
        /// A public method with a parameter
        /// </summary>
        /// <param name="name">A name</param>
        public void Bar(string name)
        {

        }
    }
}
