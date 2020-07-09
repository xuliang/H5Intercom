using System;
using System.Collections.Generic;

namespace ConsoleApp3
{
    public class PushManager
    {

        public static IDictionary<string, PushTask> tasks = new Dictionary<string, PushTask>();

        public static PushTask NewPublishTask(string key, string name)
        {
            try
            {
                PushTask task = new PushTask(name);
                tasks.Add(key, task);
                return task;
            }
            catch (Exception e)
            {
               // e.printStackTrace();
            }
            return null;
        }

        public static PushTask Get(String name)
        {
            tasks.TryGetValue(name,out PushTask v);
            return v;
        }

        public static void Remove(String key)
        {
            tasks.Remove(key);
        }
    }
}