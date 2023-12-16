using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using wey.Console;

namespace wey.Global
{
    class TaskQueueInfo
    {
        public string Name { get; set; } = "default";
        public int Times { get; set; } = -1;
        public Action Action { get; set; } = () => { };
        public byte Priority { get; set; } = 0;

        public int Timeout { get; set; } = -1;
    }

    class TaskQueue
    {
        //class

        public List<TaskQueueInfo> Queues = new();

        public int[] Add(Action action)
        {
            return Add(new TaskQueueInfo()
            {
                Action = action,
            });
        }

        public int[] Add(params TaskQueueInfo[] queues)
        {
            int StartingIndex = Queues.Count;

            Queues.AddRange(queues);

            return Enumerable.Range(StartingIndex, Queues.Count - 1).ToArray();
        }

        public void RemoveAt(params int[] indexes)
        {
            foreach (int index in indexes)
            {
                Queues.RemoveAt(index);
            }
        }

        public void Remove(Func<TaskQueueInfo, bool> callback)
        {
            List<int> RemovingIndex = new();

            int index = 0;
            foreach (TaskQueueInfo queue in Queues)
            {
                if (callback(queue))
                {
                    RemovingIndex.Add(index);
                }
                else
                {
                    index++;
                }
            }

            this.RemoveAt(RemovingIndex.ToArray());
        }

        public void Next()
        {
            Queues = Queues.OrderByDescending(queue => queue.Priority).ToList();

            foreach (TaskQueueInfo queue in Queues)
            {
                if (queue.Times != -1) queue.Times--;

                CancellationTokenSource TokenSource = new();
                Task TaskRunner = new(queue.Action.Invoke, TokenSource.Token);

                TaskRunner.Start();
                if (queue.Timeout == -1)
                {
                    TaskRunner.Wait();
                }
                else
                {
                    TaskRunner.Wait(queue.Timeout);

                    TokenSource.Cancel();
                }
            }

            this.Remove(queue => queue.Times == 0);
        }

        public void Next(int times)
        {
            while (times != 0)
            {
                Next();

                times--;
            }
        }

        //global

        public static TaskQueue Global = new();
    }
}
