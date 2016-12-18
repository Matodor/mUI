using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Other
{
    public class ActionRepeat
    {
        private readonly Action<object> _action;
        private readonly object _data;
        private readonly float _repeatTime;
        private float _nextActionTime;
        
        public ActionRepeat(float repeatTime, Action<object> action, object data = null)
        {
            _nextActionTime = Time.time + repeatTime;
            _repeatTime = repeatTime;
            _data = data;
            _action = action;
            mUI.OnTick += OnTick;
        }

        ~ActionRepeat()
        {
            mUI.Log("Destroy ActionRepeat");
        }

        private void OnTick()
        {
            if (Time.time >= _nextActionTime)
            {
                _nextActionTime = Time.time + _repeatTime;
                _action?.Invoke(_data);
            }
        }

        public void Cancel()
        {
            mUI.OnTick -= OnTick;
        }
    }
    
    public class ActionTimer
    {
        private readonly float _end;
        private readonly Action<object> _action;
        private readonly object _data;

        public ActionTimer(float timeToAction, Action<object> action, object data = null)
        {
            _end = Time.time + timeToAction;
            _data = data;
            _action = action;
            mUI.OnTick += OnTick;
        }

        ~ActionTimer()
        {
            mUI.Log("Destroy ActionTimer");
        }

        private void OnTick()
        {
            if (Time.time >= _end)
            {
                _action?.Invoke(_data);
                mUI.OnTick -= OnTick;
            }
        }

        public void Cancel()
        {
            mUI.OnTick -= OnTick;
        }
    }
}
