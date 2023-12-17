using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Global
{
    enum TaskWorkerState
    {
        None,
        Running,
        Pause,
        Stopped
    }

    class TaskWorker
    {
        protected ManualResetEvent ShutdownEvent = new(false);
        protected ManualResetEvent PauseEvent = new(true);
        protected Thread Worker;

        private readonly ThreadStart Starter;

        public TaskWorker(Action action)
        {
            Starter = () =>
            {
                while (true)
                {
                    PauseEvent.WaitOne(Timeout.Infinite);

                    if (ShutdownEvent.WaitOne(0)) break;

                    int StartingCursorTop = System.Console.CursorTop + 1;
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception);

                        Thread.Sleep(5 * 1000);
                        
                        Logger.ClearFromLine(StartingCursorTop);
                    }
                }
            };

            Worker = new Thread(Starter);
        }

        private TaskWorkerState WorkerState = TaskWorkerState.None;

        public void Start()
        {
            switch (WorkerState)
            {
                case TaskWorkerState.None:
                    {
                        Worker.Start();
                        break;
                    }
                case TaskWorkerState.Pause:
                    {
                        PauseEvent.Set();
                        break;
                    }
                case TaskWorkerState.Stopped:
                    {
                        ShutdownEvent.Reset();
                        PauseEvent.Set();

                        Worker = new Thread(Starter);

                        Worker.Start();

                        break;
                    }
            }

            WorkerState = TaskWorkerState.Running;
        }

        public void Pause()
        {
            PauseEvent.Reset();

            WorkerState = TaskWorkerState.Pause;
        }

        public void Stop()
        {
            ShutdownEvent.Set();
            PauseEvent.Set();

            Worker.Join();

            WorkerState = TaskWorkerState.Stopped;
        }
    }
}
