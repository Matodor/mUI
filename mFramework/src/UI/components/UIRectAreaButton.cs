using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRectAreaButtonSettings : UIComponentSettings
    {
        public virtual ClickCondition ClickCondition { get; set; } = ClickCondition.BUTTON_UP;
        public virtual float AreaWidth { get; set; } = 0f;
        public virtual float AreaHeight { get; set; } = 0f;
        public virtual Vector2 AreaOffset { get; set; } = Vector2.zero;
    }

    public class UIRectAreaButton : UIComponent, IUIButton
    {
        public Vector2 AreaOffset = Vector2.zero;

        public ClickCondition ClickCondition { get; set; }
        public IAreaChecker AreaChecker { get; set; }

        public event UIEventHandler<IUIButton> OnClick = delegate { };
        public event Func<IUIButton, bool> CanClick = delegate { return true; };

        public event UIEventHandler<IUIButton, Vector2> ButtonDown = delegate { };
        public event UIEventHandler<IUIButton, Vector2> ButtonUp = delegate { };
        
        private bool _isPressed;

        protected override void AfterAwake()
        {
            _isPressed = false;
            ClickCondition = ClickCondition.BUTTON_UP;
            base.AfterAwake();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIRectAreaButtonSettings buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            ClickCondition = buttonSettings.ClickCondition;
            UnscaledHeight = buttonSettings.AreaHeight;
            UnscaledWidth = buttonSettings.AreaWidth;
            AreaOffset = buttonSettings.AreaOffset;

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);

            base.ApplySettings(buttonSettings);
        }

        public void Click()
        {
            if (CanClick(this))
            {
                OnClick(this);
            }
        }

        public UIRectAreaButton SetWidth(float width)
        {
            UnscaledWidth = width;
            return this;
        }

        public UIRectAreaButton SetHeight(float height)
        {
            UnscaledHeight = height;
            return this;
        }

        public virtual void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;

            if (CanClick(this) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                OnClick(this);
            }

            ButtonDown(this, worldPos);
        }

        public virtual void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            if (CanClick(this) && _isPressed &&
                ClickCondition == ClickCondition.BUTTON_UP && AreaChecker.InAreaShape(this, worldPos))
            {
                OnClick(this);
            }

            ButtonUp(this, worldPos);
            _isPressed = false;
        }
    }
}