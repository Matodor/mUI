using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIComponent : ITicking
    {
        protected readonly UIView _parentView;
        protected readonly GameObject _gameObject;

        protected UIComponent(UIView parentView)
        {
            _parentView = parentView
            _gameObject = new GameObject("UIComponent");
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void LateTick()
        {
        }
    }
}
