using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp11
{
    public class TaskManager
    {
        public static IDictionary<string, Tasker> tasks = new Dictionary<string, Tasker>();
        public static Tasker NewTask(string key, string name)
        {
            try
            {
                Tasker task = new Tasker(name);
                tasks.Add(key, task);
                return task;
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
            return null;
        }

        public static Tasker Get(String name)
        {
            tasks.TryGetValue(name, out Tasker v);
            return v;
        }

        public static void Remove(String key)
        {
            tasks.Remove(key);
        }
    }
}
