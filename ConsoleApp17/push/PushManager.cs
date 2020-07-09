using System;
using System.Collections.Generic;

namespace ConsoleApp17
{
    public class PushManager
    {

        public static IDictionary<string, RTMPTask> tasks = new Dictionary<string, RTMPTask>();

        public static RTMPTask NewPublishTask(string key, string name)
        {
            try
            {
                RTMPTask task = new RTMPTask(name);
                tasks.Add(key, task);
                return task;
            }
            catch (Exception e)
            {
               // e.printStackTrace();
            }
            return null;
        }

        public static RTMPTask Get(String name)
        {
            tasks.TryGetValue(name,out RTMPTask v);
            return v;
        }

        public static void Remove(String key)
        {
            tasks.Remove(key);
        }
    }
}