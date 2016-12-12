using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Other
{
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
