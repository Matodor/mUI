using System;
using UnityEngine;

namespace mFramework
{
    public class RepeatAction : IGlobalUniqueIdentifier
    {
        public ulong GUID { get; }

        private readonly Action _action;
        private readonly float _repeatTime;
        private float _nextInvoke;

        private static ulong _guid;

        private RepeatAction(Action action, float repeatTime)
        {
            _repeatTime = repeatTime;
            _action = action;
            _nextInvoke = Time.time + repeatTime;

            GUID = ++_guid;
            mCore.Instance.AddRepeatAction(this);
        }

        public void Remove()
        {
            mCore.Instance.RemoveRepeatAction(this);
        }

        internal void Tick()
        {
            if (Time.time >= _nextInvoke)
            {
                _nextInvoke = Time.time + _repeatTime;
                _action?.Invoke();
            }
        }

        public static RepeatAction Create(Action action, float repeatTime)
        {
            return new RepeatAction(action, repeatTime);
        }
    }
}
