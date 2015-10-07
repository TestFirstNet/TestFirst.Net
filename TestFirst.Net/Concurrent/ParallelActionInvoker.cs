using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TestFirst.Net.Concurrent
{
    /// <summary>
    /// A custom Async action invoker as the default Task.Factory and Parallel seem rather slow
    /// to begin the actions. Not great for testing contention with many threads.
    /// </summary>
    public static class ParallelActionInvoker
    {
        /// <summary>
        /// See <see cref="InvokeAllWaitingForCompletion(System.Collections.Generic.IEnumerable{System.Action}, TimeSpan, ThreadPriority)"/>
        /// <para>
        /// Uses a timeout currently set to 2 minutes
        /// </para>
        /// </summary>
        /// <param name="parallelActions">actions to complete in parallel</param>
        public static void InvokeAllWaitingForCompletion(IEnumerable<Action> parallelActions)
        {
            InvokeAllWaitingForCompletion(parallelActions, TimeSpan.FromMinutes(2));
        }

        /// <summary>
        /// Invoke all the given actions in parallel in their own thread. 
        /// <para>
        /// A memory barrier is used to line up all actions in an attempt to ensure they are all invoked at the same time. The idea is 
        /// to cause maximum contention. However this is very much up to the OS, number or cores, whether true threading is used etc.
        /// </para>
        /// <para>
        /// If you are performing multiple operations inside your action, it is suggested to to add a Thread.Yield() in suitable
        /// places to increase the chance you will get out of order execution between threads (rather than one thread executing all the
        /// way through without being switched out partway through operation)
        /// </para>
        /// </summary>
        /// <param name="parallelActions">actions to complete in parallel</param>
        /// <param name="timeout">timeout period after which to abort</param>
        /// <param name="priority">priority with which to run threads</param>
        public static void InvokeAllWaitingForCompletion(IEnumerable<Action> parallelActions, TimeSpan timeout, ThreadPriority priority = ThreadPriority.Normal)
        {
            var actionList = new List<Action>(parallelActions); // allow multiple iteration      
            var barrierToCauseContention = new Barrier(actionList.Count());
            var wrappedActions = new List<ActionWrapper>(actionList.Count());
            var threads = new List<Thread>(actionList.Count);

            // wrap passed in actions so we can monitor the state of their execution and collect any exceptions thrown
            // the wrapped actions will wait on the barrier
            foreach (var action in actionList)
            {
                var wrapper = new ActionWrapper(barrierToCauseContention, action);
                var thread = new Thread(wrapper.Invoke) { Priority = priority };

                threads.Add(thread);                
                wrappedActions.Add(wrapper);

                thread.Start();
            }

            // wait till all actions complete
            var timeoutInMs = timeout.TotalMilliseconds;
            var stopWatch = Stopwatch.StartNew();
            var allComplete = false;
            while (!allComplete)
            {
                // if we timed out
                if (stopWatch.ElapsedMilliseconds > timeoutInMs)
                {
                    // kill all the threads still running
                    foreach (var t in threads)
                    {
                        TerminateThreadQuietly(t);
                    }
                    TestFirstAssert.Fail(string.Format("Timed out waiting for all actions to complete. Waitied for {0} milliseonds (TimeSpan {1}). Aborted remaining threads", timeoutInMs, timeout));
                }

                // determine if all actions complete
                var complete = true;
                foreach (var wrapper in wrappedActions)
                {
                    if (!wrapper.IsComplete)
                    {
                        complete = false;
                        break;
                    }
                }
                allComplete = complete;
                if (!allComplete)
                {
                    Thread.Sleep(100); // give actions a bit of time to complete
                }
            }

            // fail if any of the actions threw an exception
            var exceptions = new List<string>();
            foreach (var wrapper in wrappedActions)
            {
                if (wrapper.HasFailed)
                {
                    exceptions.Add("thread[" + wrapper.ThreadId + "] : " + wrapper.Exception);
                }
            }
            if (exceptions.Count > 0)
            {               
                var msg = "===============Action Error===========\r\n" + string.Join("\r\n===============Action Error===========", exceptions);
                TestFirstAssert.Fail(msg);
            }
        }

        private static void TerminateThreadQuietly(Thread t)
        {
            if (t.IsAlive)
            {
                try
                {
                    t.Abort();
                }
                catch
                {
                    // ignoring errors
                }
            }
        }

        /// <summary>
        /// Use a barrier to line up action execution. Collects any exception thrown by the action and maintain a flag 
        /// to determine if the action has completed running
        /// </summary>
        private class ActionWrapper
        {
            private readonly Action m_action;
            private readonly Barrier m_barrier;
            private volatile bool m_complete;
            private volatile Exception m_actionException;
            private volatile int m_threadId;

            internal ActionWrapper(Barrier barrier, Action action)
            {
                m_barrier = barrier;
                m_action = action;
            }

            public bool IsComplete
            {
                get { return m_complete; }
            }

            public bool HasFailed
            {
                get { return m_actionException != null; }
            }

            public Exception Exception
            {
                get
                {
                    return m_actionException;
                }
            }

            /// <summary>
            /// Gets the thread id of the thread used to invoke the action. Useful in diagnostics
            /// </summary>
            public string ThreadId
            {
                get
                {
                    return m_threadId.ToString();
                }
            }
            public void Invoke()
            {
                try
                {
                    m_threadId = Thread.CurrentThread.ManagedThreadId;

                    // try to cause max contention                    
                    if (!m_barrier.SignalAndWait(TimeSpan.FromSeconds(30)))
                    {
                        throw new TestFirstException("Timed out waiting for start action synchronization barrier (to cause max contention)");
                    }                          
                    m_action.Invoke();
                }
                catch (Exception e)
                {
                    m_actionException = e;
                }
                finally
                {
                    m_complete = true;
                }
            }
        }
    }
}
