using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace mFramework
{
    public static class SynchronizeInvokeExtension
    {
        public static void SyncInvoke(this MonoBehaviour behaviour, Action action)
        {
            SynchronizeInvoke.Instance.Invoke(action);
        }

        public static void SyncInvoke<T1>(this MonoBehaviour behaviour, Action<T1> action, T1 arg1)
        {
            SynchronizeInvoke.Instance.Invoke(action, arg1);
        }

        public static void SyncInvoke<T1, T2>(this MonoBehaviour behaviour, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            SynchronizeInvoke.Instance.Invoke(action, arg1, arg2);
        }

        public static void SyncInvoke<T1, T2, T3>(this MonoBehaviour behaviour, Action<T1, T2, T3> action,
            T1 arg1, T2 arg2, T3 arg3)
        {
            SynchronizeInvoke.Instance.Invoke(action, arg1, arg2, arg3);
        }

        public static void SyncInvoke<T1, T2, T3, T4>(this MonoBehaviour behaviour, Action<T1, T2, T3, T4> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            SynchronizeInvoke.Instance.Invoke(action, arg1, arg2, arg3);
        }
    }

    public class SynchronizeInvoke
    {
        public static SynchronizeInvoke Instance { get; } = new SynchronizeInvoke();
        public bool InvokeRequired => _syncThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId;

        private readonly Queue<AsyncInvoke> _toExecuteQueue;
        private readonly Thread _syncThread;

        private class AsyncInvoke
        {
            private readonly Delegate _delegate;
            private readonly object[] _args;

            public AsyncInvoke(Delegate @delegate, params object[] args)
            {
                _delegate = @delegate;
                _args = args;
            }

            public void Invoke()
            {
                _delegate.DynamicInvoke(_args);
            }
        }

        public SynchronizeInvoke()
        {
            _syncThread = Thread.CurrentThread;
            _toExecuteQueue = new Queue<AsyncInvoke>();
        }

        public void Invoke(Delegate method, params object[] args)
        {
            if (InvokeRequired)
            {
                lock (_toExecuteQueue)
                {
                    _toExecuteQueue.Enqueue(new AsyncInvoke(method, args));
                }
            }
            else
            {
                method.DynamicInvoke(args);
            }
        }

        public void ProcessQueue()
        {
            if (_syncThread != Thread.CurrentThread)
            {
                throw new Exception("must be called from the same thread it was created on");
            }

            AsyncInvoke data = null;
            while (true)
            {
                lock (_toExecuteQueue)
                {
                    if (_toExecuteQueue.Count == 0)
                        break;

                    data = _toExecuteQueue.Dequeue();
                }

                data.Invoke();
            }
        }
    }
}