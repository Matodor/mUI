using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRectAreaButtonProps : UIComponentProps, ISizeable
    {
        public virtual ClickCondition ClickCondition { get; set; } = ClickCondition.BUTTON_UP;
        public float SizeX { get; set; }
        public float SizeY { get; set; }
    }

    public class UIRectAreaButton : UIComponent, IUIButton
    {
        public event UIMouseEvent MouseDown;
        public event UIMouseEvent MouseUp;

        public event UIMouseAllowEvent CanMouseDown
        {
            add => _canMouseDownEvents.Add(value);
            remove => _canMouseDownEvents.Remove(value);
        }

        public event UIMouseAllowEvent CanMouseUp
        {
            add => _canMouseUpEvents.Add(value);
            remove => _canMouseUpEvents.Remove(value);
        }

        public event UIButtonClickEvent Clicked;
        public event UIButtonAllowClick CanClick
        {
            add => _canClickButtonEvents.Add(value);
            remove => _canClickButtonEvents.Remove(value);
        }

        public bool IgnoreByHandler { get; set; }
        public bool IsPressed { get; protected set; }

        public IAreaChecker AreaChecker { get; set; }
        public ClickCondition ClickCondition { get; set; }

        private List<UIMouseAllowEvent> _canMouseDownEvents;
        private List<UIMouseAllowEvent> _canMouseUpEvents;
        private List<UIButtonAllowClick> _canClickButtonEvents;

        protected override void OnBeforeDestroy()
        {
            MouseDown = null;
            MouseUp = null;
            Clicked = null;

            _canMouseDownEvents.Clear();
            _canMouseUpEvents.Clear();
            _canClickButtonEvents.Clear();
            base.OnBeforeDestroy();
        }

        protected override void AfterAwake()
        {
            _canMouseDownEvents = new List<UIMouseAllowEvent>();
            _canMouseUpEvents = new List<UIMouseAllowEvent>();
            _canClickButtonEvents = new List<UIButtonAllowClick>();
            
            IsPressed = false;
            ClickCondition = ClickCondition.BUTTON_UP;

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);

            base.AfterAwake();
        }

        protected override void ApplyProps(UIComponentProps props)
        { 
            if (!(props is UIRectAreaButtonProps buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            ClickCondition = buttonSettings.ClickCondition;
            SizeX = buttonSettings.SizeX;
            SizeY = buttonSettings.SizeY;
            
            base.ApplyProps(buttonSettings);
        }

        public UIRectAreaButton SetWidth(float width)
        {
            SizeX = width;
            return this;
        }

        public UIRectAreaButton SetHeight(float height)
        {
            SizeY = height;
            return this;
        }

        public bool Click()
        {
            if (_canClickButtonEvents.Count != 0 &&
                !_canClickButtonEvents.TrueForAll(e => e(this)))
            {
                return false;
            }

            Clicked?.Invoke(this);
            return true;
        }

        private void OnUIMouseDown()
        {
            if (ClickCondition == ClickCondition.BUTTON_DOWN)
                Click();
        }

        private void OnUIMouseUp(Vector2 worldPos)
        {
            if (ClickCondition == ClickCondition.BUTTON_UP && AreaChecker.InAreaShape(this, worldPos))
                Click();
        }
        
        public void DoMouseDown(Vector2 worldPos)
        {
            if (!IsActive || IsPressed || !AreaChecker.InAreaShape(this, worldPos))
                return;

            if (_canMouseDownEvents.Count != 0 &&
                !_canMouseDownEvents.TrueForAll(e => e(this, ref worldPos)))
            {
                return;
            }

            IsPressed = true;
            OnUIMouseDown();
            MouseDown?.Invoke(this, worldPos);
        }

        public void DoMouseUp(Vector2 worldPos)
        {
            if (!IsPressed)
                return;

            if (_canMouseUpEvents.Count != 0 &&
                !_canMouseUpEvents.TrueForAll(e => e(this, ref worldPos)))
            {
                return;
            }

            OnUIMouseUp(worldPos);
            MouseUp?.Invoke(this, worldPos);
            IsPressed = false;
        }
    }
}