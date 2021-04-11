using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCSharp.Misc
{
    public class ScheduledThread : IDisposable
    {
        private bool currentlyExecuting = false;
        private EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private EventWaitHandle clearHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private class ActionEntry
        {
            public ScheduledAction Action;
            public object[] Args;
            public ActionEntry(ScheduledAction Action, object[] Args)
            {
                this.Action = Action;
                this.Args = Args;
            }
        }
        private static ConcurrentBag<int> threadIds = new ConcurrentBag<int>();
        private static ConcurrentBag<ScheduledThread> schedulers = new ConcurrentBag<ScheduledThread>();
        private ConcurrentQueue<ActionEntry> entries = new ConcurrentQueue<ActionEntry>();
        private Thread actionThread;
        public Thread UnderlyingThread
        {
            get
            {
                return actionThread;
            }
        }
        private ConcurrentStack<ActionEntry> currentInterrupt = new ConcurrentStack<ActionEntry>();
        private ConcurrentStack<bool> clearingRequests = new ConcurrentStack<bool>();
        private bool allowInterrupts = false;
        /// <summary>
        /// Gets a value indicating whether this ScheduledThread allows interrupts.
        /// </summary>
        public bool AllowInterrupts
        {
            get
            {
                return allowInterrupts;
            }
        }
        /// <summary>
        /// Creates a new ScheduledThread.
        /// </summary>
        /// <param name="AllowInterrupts">Whether or not this ScheduledThread should allow interrupts.</param>
        /// <param name="IsAnonymous">If true, this instance will not be retrievable from ScheduledThread.CurrentThread.</param>
        public ScheduledThread(bool AllowInterrupts = false, bool IsAnonymous = false)
        {
            allowInterrupts = AllowInterrupts;
            actionThread = new Thread(actionLoop);
            threadIds.Add(actionThread.ManagedThreadId);
            if (IsAnonymous)
            {
                schedulers.Add(null);
            }
            else
            {
                schedulers.Add(this);
            }
            actionThread.Name = "Scheduled Thread";
            actionThread.Start();
        }
        /// <summary>
        /// Get the current ScheduledThread that this code is running under. Throws an exception if the current code is not Scheduled or the ScheduledThread is anonymous.
        /// </summary>
        public static ScheduledThread CurrentThread
        {
            get
            {
                List<int> ids = threadIds.ToList();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i] == Thread.CurrentThread.ManagedThreadId)
                    {
                        ScheduledThread result = schedulers.ToList()[i];
                        if (result != null)
                        {
                            return result;
                        }
                        else
                        {
                            throw new InvalidOperationException("The currently executing code is running under an anonymous ScheduledThread.");
                        }
                    }
                }
                throw new InvalidOperationException("The currently executing code is not scheduled code.");
            }
        }
        /// <summary>
        /// Schedules a ScheduledAction to be run after all of the currently scheduled ScheduledActions have completed.
        /// </summary>
        /// <param name="action">The action to schedule.</param>
        public void Queue(ScheduledAction action, params object[] args)
        {
            if (clearingRequests.IsEmpty)
            {
                bool wasEmpty = entries.IsEmpty;
                entries.Enqueue(new ActionEntry(action, args));
                if (wasEmpty)
                {
                    waitHandle.Set();
                }
            }
        }
        /// <summary>
        /// If interrupts are allowed, stops the current ScheduledAction and runs this action. Once complete, the thread will continue through the queue like normal but the interrupted action will never complete.
        /// </summary>
        /// <param name="action">The action to interrupt with.</param>
        public void Interrupt(ScheduledAction action, params object[] args)
        {
            if (!allowInterrupts)
            {
                throw new InvalidOperationException("This ScheduledThread does not allow interrupts.");
            }
            currentInterrupt.Push(new ActionEntry(action, args));
            skip();
        }
        /// <summary>
        /// Temporarily starts ignoring ScheduledThread.Queue both internally and externally, then blocks until the thread queue is empty, then re-enables ScheduledThread.Queue.
        /// </summary>
        public void FinishUp()
        {
            if (entries.IsEmpty && !currentlyExecuting)
            {
                return;
            }
            clearingRequests.Push(true);
            while (!(entries.IsEmpty && !currentlyExecuting))
            {
                clearHandle.WaitOne(5);
            }
            bool throwaway;
            while (!clearingRequests.TryPop(out throwaway)) { }
        }
        /// <summary>
        /// Stops the current ScheduledAction then clears the queue. The action will never complete and all previously scheduled actions will never start.
        /// </summary>
        public void ForceClear()
        {
            ActionEntry throwaway;
            while (!entries.IsEmpty)
            {
                while (!entries.TryDequeue(out throwaway)) { }
            }
            skip();
        }
        /// <summary>
        /// Generate an event handler that redirects to your Scheduled event handler. When the handler is called the ScheduledAction will be queued, not used to interrupt.
        /// </summary>
        /// <param name="Redirect">The ScheduledAction to redirect to.</param>
        /// <returns></returns>
        public EventHandler GenerateEventHandler(ScheduledAction Redirect)
        {
            return new EventHandler((object sender, EventArgs e) => {
                this.Queue(Redirect, sender, e);
            });
        }
        private void skip()
        {
            actionThread.Abort();
            while (actionThread.ThreadState == ThreadState.AbortRequested || actionThread.ThreadState == ThreadState.Running) { }
            actionThread = new Thread(actionLoop);
            actionThread.Start();
        }
        public void Dispose()
        {
            actionThread.Abort();
        }
        public delegate void ScheduledAction(params object[] args);
        private void actionLoop()
        {
            if (!currentInterrupt.IsEmpty)
            {
                ActionEntry action;
                while (!currentInterrupt.TryPop(out action)) { }
                currentlyExecuting = true;
                action.Action(action.Args);
                currentlyExecuting = false;
            }
            while (true)
            {
                ActionEntry action;
                if (entries.IsEmpty)
                {
                    waitHandle.WaitOne();
                }
                while (!entries.TryDequeue(out action)) { }
                currentlyExecuting = true;
                action.Action(action.Args);
                currentlyExecuting = false;
                if (entries.IsEmpty && !clearingRequests.IsEmpty)
                {
                    clearHandle.Set();
                }
            }
        }
    }
}
