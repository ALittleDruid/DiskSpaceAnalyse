using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace DiskSpaceAnalyse
{
    public class UIThreadHelper
    {
        private static UIThreadHelper threadHelper;

        private readonly Thread thread;
        private readonly object obj = new object();
        private readonly Queue<Action> works = new Queue<Action>();

        private bool sleep;

        private UIThreadHelper()
        {
            if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
            {
                throw new Exception("This object must on main thread");
            }
            thread = new Thread(() =>
            {
                while (true)
                {
                    Action action = null;
                    lock (obj)
                    {
                        if (works.Count > 0)
                        {
                            action = works.Dequeue();
                        }
                        else
                        {
                            sleep = true;
                            Monitor.Wait(obj);
                            sleep = false;
                        }
                    }
                    if (action != null)
                    {
                        action.Invoke();
                    }
                }
            })
            {
                Name = "WatchDog",
                IsBackground = true
            };
            thread.Start();
        }

        public static UIThreadHelper GetInstance()
        {
            if (threadHelper == null)
            {
                threadHelper = new UIThreadHelper();
            }
            return threadHelper;
        }

        public void AddWork(Action action)
        {
            lock (obj)
            {
                works.Enqueue(action);
                if (sleep)
                {
                    Monitor.Pulse(obj);
                }
            }
        }
    }
}
