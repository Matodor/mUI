using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIButtonSettings : UIComponentSettings
    {
        public ClickCondition ClickCondition { get; set; } = ClickCondition.BUTTON_UP;
        public SpriteStates ButtonSpriteStates { get; set; }
        public AreaType ButtonAreaType { get; set; } = AreaType.RECTANGLE;
    }

    public enum ClickCondition
    {
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_PRESSED
    }

    public delegate bool CanButtonClickDelegate(UIButton sender, Vector2 worldPos);

    public class UIButton : UIComponent, IUIClickable, IUIRenderer, IColored
    {
        public Renderer UIRenderer => _uiSprite.UIRenderer;
        public UIClickable UIClickable => _clickableHandler;
        public StateableSprite StateableSprite => _stateableSprite;
        public ClickCondition ClickCondition { get; set; }

        public event UIEventHandler<UIButton> Click;
        public event CanButtonClickDelegate CanButtonClick;

        public event UIEventHandler<UIButton, ButtonEventArgs> ButtonDown;
        public event UIEventHandler<UIButton, ButtonEventArgs> ButtonUp;

        protected UIClickable _clickableHandler;
        protected StateableSprite _stateableSprite;
        protected UISprite _uiSprite;
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

            if (!(settings is UIButtonSettings buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            ClickCondition = buttonSettings.ClickCondition;

            switch (buttonSettings.ButtonAreaType)
            {
                case AreaType.RECTANGLE:
                    SetupRectangleHandler();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _uiSprite = this.Sprite(new UISpriteSettings {Sprite = buttonSettings.ButtonSpriteStates.Default});
            _stateableSprite = StateableSprite.Create(buttonSettings.ButtonSpriteStates);
            _stateableSprite.StateChanged += (s, e) =>
            {
                if (e.StateSprite != null && _uiSprite.Renderer.sprite != e.StateSprite)
                    _uiSprite.SetSprite(e.StateSprite);
            };

            base.ApplySettings(buttonSettings);
        }

        private void SetupRectangleHandler()
        {
            if (_clickableHandler != null)
            {
                // remove
            }

            _clickableHandler = UIClickable.Create(this, AreaType.RECTANGLE);
            _clickableHandler.Area2D.Update += area2d =>
            {
                if (area2d is RectangleArea2D rect2d)
                {
                    rect2d.Height = GetHeight();
                    rect2d.Width = GetWidth();
                    rect2d.Offset = _uiSprite.Renderer.sprite?.GetCenterOffset() ?? Vector2.zero;
                    rect2d.Offset = new Vector2(
                        rect2d.Offset.x * GlobalScale().x, 
                        rect2d.Offset.y * GlobalScale().y
                    );
                }
            };
        }

        public override UIRect GetRect()
        {
            return _uiSprite.GetRect();
        }

        public override float GetHeight()
        {
            return _uiSprite.GetHeight();
        }

        public override float GetWidth()
        {
            return _uiSprite.GetWidth();
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isMouseDown = true;
            _stateableSprite.SetHighlighted();

            if ((CanButtonClick?.Invoke(this, worldPos) ?? true) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                Click?.Invoke(this);
            }
            ButtonDown?.Invoke(this, new ButtonEventArgs(worldPos));
        }

        public void MouseUp(Vector2 worldPos)
        {
            _stateableSprite.SetDefault();
            if ((CanButtonClick?.Invoke(this, worldPos) ?? true) && _isMouseDown &&
                ClickCondition == ClickCondition.BUTTON_UP && _clickableHandler.InArea(worldPos))
            {
                Click?.Invoke(this);
            }
            ButtonUp?.Invoke(this, new ButtonEventArgs(worldPos));
            _isMouseDown = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }

        public Color GetColor()
        {
            return _uiSprite.GetColor();
        }

        public UIObject SetColor(Color32 color)
        {
            _uiSprite.SetColor(color);
            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            _uiSprite.SetColor(color);
            return this;
        }
    }
}
