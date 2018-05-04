using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRectAreaButtonSettings : UIComponentSettings
    {
        public ClickCondition ClickCondition = ClickCondition.BUTTON_UP;
        public float AreaWidth = 0f;
        public float AreaHeight = 0f;
        public Vector2 AreaOffset = Vector2.zero;
    }

    public class UIRectAreaButton : UIComponent, IUIButton
    {
        public Vector2 AreaOffset = Vector2.zero;
        public float AreaWidth = 0f;
        public float AreaHeight = 0;
        
        public UIClickableOld UiClickableOld { get; protected set; }
        public ClickCondition ClickCondition { get; set; }

        public event UIEventHandler<IUIButton> Click = delegate { };
        public event Func<IUIButton, Vector2, bool> CanButtonClick = delegate { return true; };

        public event UIEventHandler<IUIButton, Vector2> ButtonDown = delegate { };
        public event UIEventHandler<IUIButton, Vector2> ButtonUp = delegate { };

        private bool _isPressed;

        protected override void AfterAwake()
        {
            _isPressed = false;
            ClickCondition = ClickCondition.BUTTON_UP;
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIRectAreaButtonSettings buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            ClickCondition = buttonSettings.ClickCondition;
            AreaHeight = buttonSettings.AreaHeight;
            AreaWidth = buttonSettings.AreaWidth;
            AreaOffset = buttonSettings.AreaOffset;

            var area = new RectangleArea2D();
            area.Update += a =>
            {
                area.Width = Width;
                area.Height = Height;
                area.Offset = new Vector2(
                    AreaOffset.x * GlobalScale.x,
                    AreaOffset.y * GlobalScale.y
                );
            };
            UiClickableOld = new UIClickableOld(this, area);

            base.ApplySettings(buttonSettings);
        }

        public override float UnscaledHeight => AreaHeight * GlobalScale.y;
        public override float UnscaledWidth => AreaWidth * GlobalScale.x;
        public override Vector2 CenterOffset => Vector2.zero;

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;

            if (CanButtonClick.Invoke(this, worldPos) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                Click.Invoke(this);
            }

            ButtonDown.Invoke(this, worldPos);
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            if (CanButtonClick.Invoke(this, worldPos) && _isPressed &&
                ClickCondition == ClickCondition.BUTTON_UP && UiClickableOld.Area2D.InArea(worldPos))
            {
                Click.Invoke(this);
            }

            ButtonUp.Invoke(this, worldPos);
            _isPressed = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }
    }
}