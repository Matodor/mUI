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

    public class UIButton : UIComponent, IUIClickable, IUIRenderer, IColored
    {
        public Renderer UIRenderer { get { return _uiSprite.UIRenderer; } }
        public UIClickable UIClickable { get { return _clickableHandler; } }
        public ClickCondition ClickCondition { get; set; }
        public StateableSprite StateableSprite { get { return _stateableSprite; } }

        public event Action<UIButton> OnClick;
        public event Func<UIButton, Vector2, bool> OnMouseDown, OnMouseUp;
        
        private UIClickable _clickableHandler;
        private StateableSprite _stateableSprite;
        private UISprite _uiSprite;
        private bool _isMouseDown;

        protected UIButton(UIObject parent) : base(parent)
        {
            _isMouseDown = false;
            ClickCondition = ClickCondition.BUTTON_UP;
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var buttonSettings = settings as UIButtonSettings;
            if (buttonSettings == null)
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
            SetupClickableHandler();

            _uiSprite = Component<UISprite>(new UISpriteSettings { Sprite = buttonSettings.ButtonSpriteStates.Default });
            _stateableSprite = StateableSprite.Create(buttonSettings.ButtonSpriteStates);
            _stateableSprite.OnStateChanged += sprite =>
            {
                if (sprite != null && _uiSprite.Renderer.sprite != sprite)
                    _uiSprite.SetSprite(sprite);
            };

            base.ApplySettings(buttonSettings);
        }

        private void SetupRectangleHandler()
        {
            if (_clickableHandler != null)
            {
                // remove
            }

            _clickableHandler = UIClickable.Create(this, this, AreaType.RECTANGLE);
            _clickableHandler.Area2D.Update += area2d =>
            {
                var rect2d = area2d as RectangleArea2D;
                if (rect2d != null)
                {
                    rect2d.Height = GetHeight();
                    rect2d.Width = GetWidth();
                    rect2d.Offset = _uiSprite.Renderer.sprite?.GetCenterOffset() ?? Vector2.zero;
                }
            };
        }

        private void SetupClickableHandler()
        {
            if (_clickableHandler == null)
                return;

            _clickableHandler.CanMouseUp += (h, e) => IsActive;
            _clickableHandler.CanMouseDown += (h, e) => IsActive;
            _clickableHandler.CanMouseDrag += (h, e) => IsActive;
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

            if ((OnMouseDown?.Invoke(this, worldPos) ?? true) && ClickCondition == ClickCondition.BUTTON_DOWN)
                OnClick?.Invoke(this);
        }

        public void MouseUp(Vector2 worldPos)
        {
            _stateableSprite.SetDefault();
            if ((OnMouseUp?.Invoke(this, worldPos) ?? true) && _isMouseDown && ClickCondition == ClickCondition.BUTTON_UP && _clickableHandler.Area2D.InArea(worldPos)) 
                OnClick?.Invoke(this);
            _isMouseDown = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
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
