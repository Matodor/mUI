using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public class UIToggleProps : UIButtonProps
    {
        public virtual bool DefaultSelected { get; set; } = false;
    }
    
    public class UIToggle : UIButton, IUIToggle
    {
        public bool IsSelected { get; protected set; }

        public event UIToggleAllowChangeState CanSelect
        {
            add => _canSelectEvents.Add(value);
            remove => _canSelectEvents.Remove(value);
        }

        public event UIToggleAllowChangeState CanDeselect
        {
            add => _canDeselectEvents.Add(value);
            remove => _canDeselectEvents.Remove(value);
        }

        public event UIToggleStateChangedEvent Selected;
        public event UIToggleStateChangedEvent Deselected;
        public event UIToggleStateChangedEvent Changed;

        private List<UIToggleAllowChangeState> _canSelectEvents;
        private List<UIToggleAllowChangeState> _canDeselectEvents;

        protected override void OnBeforeDestroy()
        {
            Selected = null;
            Deselected = null;
            Changed = null;

            Clicked -= OnToggleClick;
            MouseUp -= OnUIMouseUp;

            _canSelectEvents.Clear();
            _canDeselectEvents.Clear();
            base.OnBeforeDestroy();
        }

        protected override void AfterAwake()
        {
            _canSelectEvents = new List<UIToggleAllowChangeState>();
            _canDeselectEvents = new List<UIToggleAllowChangeState>();

            Clicked += OnToggleClick;
            MouseUp += OnUIMouseUp;

            IsSelected = false;
            base.AfterAwake();
        }

        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UIToggleProps toggleSettings))
                throw new ArgumentException("UIToggle: The given settings is not UIToggleSettings");

            if (toggleSettings.DefaultSelected)
                Select();
            
            base.ApplyProps(props);
        }

        private void OnUIMouseUp(IUIClickable sender, Vector2 vector2)
        {
            if (IsSelected)
                StateableSprite.SetSelected();
            else
                StateableSprite.SetDefault();
        }

        public IUIToggle Toggle()
        {
            if (IsSelected)
                Deselect();
            else
                Select();

            return this;
        }

        public IUIToggle Select()
        {
            if (_canSelectEvents.Count != 0 && !_canSelectEvents.TrueForAll(e => e(this)))
                return this;

            IsSelected = true;
            StateableSprite.SetSelected();

            Selected?.Invoke(this);
            Changed?.Invoke(this);
            return this;
        }

        public IUIToggle Deselect()
        {
            if (_canDeselectEvents.Count != 0 && !_canDeselectEvents.TrueForAll(e => e(this)))
                return this;

            IsSelected = false;
            StateableSprite.SetDefault();

            Deselected?.Invoke(this);
            Changed?.Invoke(this);
            return this;
        }

        private void OnToggleClick(IUIButton sender)
        {
            Toggle();
        }
    }
}
