using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class PushManager
    {
        public static IDictionary<string, TaskClass> tasks = new Dictionary<string, TaskClass>();

        public static TaskClass NewPublishTask(string key, string name)
        {
            try
            {
                TaskClass task = new TaskClass(name);
                tasks.Add(key, task);
                return task;
            }
            catch (Exception e)
            {
                // e.printStackTrace();
            }
            return null;
        }

        public static TaskClass Get(String name)
        {
            tasks.TryGetValue(name, out TaskClass v);
            return v;
        }

        public static void Remove(String key)
        {
            tasks.Remove(key);
        }

    }
}
