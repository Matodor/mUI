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

    public class UIRectAreaButton : UIComponent, IUIClickable
    {
        public Vector2 AreaOffset = Vector2.zero;
        public float AreaWidth = 0f;
        public float AreaHeight = 0;

        public delegate bool CanClick(UIRectAreaButton sender, Vector2 worldPos);
        
        public UIClickable UIClickable { get; protected set; }
        public ClickCondition ClickCondition { get; set; }

        public event UIEventHandler<UIRectAreaButton> Click = delegate { };
        public event CanClick CanButtonClick = delegate { return true; };

        public event UIEventHandler<UIRectAreaButton, Vector2> ButtonDown = delegate { };
        public event UIEventHandler<UIRectAreaButton, Vector2> ButtonUp = delegate { };

        private bool _isMouseDown;

        protected override void Init()
        {
            _isMouseDown = false;
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
                area.Width = GetWidth();
                area.Height = GetHeight();
                area.Offset = new Vector2(
                    AreaOffset.x * GlobalScale().x,
                    AreaOffset.y * GlobalScale().y
                );
            };
            UIClickable = new UIClickable(this, area);

            base.ApplySettings(buttonSettings);
        }

        public override float GetWidth()
        {
            return AreaWidth * GlobalScale().x;
        }
        
        public override float GetHeight()
        {
            return AreaHeight * GlobalScale().y;
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isMouseDown = true;

            if (CanButtonClick.Invoke(this, worldPos) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                Click.Invoke(this);
            }

            ButtonDown.Invoke(this, worldPos);
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (CanButtonClick.Invoke(this, worldPos) && _isMouseDown &&
                ClickCondition == ClickCondition.BUTTON_UP && UIClickable.Area2D.InArea(worldPos))
            {
                Click.Invoke(this);
            }

            ButtonUp.Invoke(this, worldPos);
            _isMouseDown = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }
    }
}