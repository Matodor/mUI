﻿using System;
using UnityEngine;

namespace mFramework
{
    public class TimerAction : IGlobalUniqueIdentifier
    {
        public ulong GUID { get; }

        private readonly Action _action;
        private readonly float _actionTime;

        private static ulong _guid;

        private TimerAction(Action action, float timeToAction)
        {
            _action = action;
            _actionTime = Time.realtimeSinceStartup + timeToAction;

            GUID = ++_guid;
            mCore.Instance.AddTimerAction(this);
        }

        public void Remove()
        {
            mCore.Instance.RemoveTimerAction(this);
        }

        internal void Tick()
        {
            if (_actionTime <= Time.realtimeSinceStartup)
            {
                _action?.Invoke();
                Remove();
            }
        }

        public static TimerAction Create(Action action, float timeToAction)
        {
            return new TimerAction(action, timeToAction);
        }
    }
}