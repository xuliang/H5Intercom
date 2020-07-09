using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class TaskClass
    {
        string name;
        public TaskClass(string name)
        {
            this.name = name;
        }
        public void m1()
        {
            Console.WriteLine("m1");
        }
        public void m2()
        {
            Console.WriteLine("m2-"+name);
        }
        public void m3()
        {
            Console.WriteLine("m3");
        }
    }
}
