using System;
using UnityEngine;

namespace mFramework
{
    public class RepeatAction : IGlobalUniqueIdentifier
    {
        public ulong GUID { get; }
        public float Interval { get; set; }

        private readonly Action _action;
        private float _nextInvoke;

        private static ulong _guid;

        private RepeatAction(Action action, float interval)
        {
            Interval = interval;
            _action = action;
            _nextInvoke = Time.time + interval;

            GUID = ++_guid;
            mCore.AddRepeatAction(this);
        }

        public void Remove()
        {
            mCore.RemoveRepeatAction(this);
        }

        internal void Tick()
        {
            if (Time.time >= _nextInvoke)
            {
                _nextInvoke = Time.time + Interval;
                _action.Invoke();
            }
        }

        public static RepeatAction Create(Action action, float repeatTime)
        {
            return new RepeatAction(action, repeatTime);
        }
    }
}
