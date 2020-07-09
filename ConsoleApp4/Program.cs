using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskClass tc1 = PushManager.NewPublishTask("tc1", "taskName-tc1");
            //Task t1 = new Task(tc1.m1);
            Thread t1 = new Thread(tc1.m1);
            t1.Start();
            tc1.m2();

            
            Console.ReadLine();
        }
    }
}
