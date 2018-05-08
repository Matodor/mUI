using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIButtonSettings : UISpriteSettings
    {
        public virtual ClickCondition ClickCondition { get; set; } = ClickCondition.BUTTON_UP;
        public virtual SpriteStates ButtonSpriteStates { get; set; }

        public override Sprite Sprite
        {
            get => ButtonSpriteStates.Default;
            set
            {
                var buttonSpriteStates = ButtonSpriteStates;
                buttonSpriteStates.Default = value;
                ButtonSpriteStates = buttonSpriteStates;
            }
        }
    }

    public enum ClickCondition
    {
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_PRESSED
    }
    
    public class UIButton : UISprite, IUIClickable
    {
        public IAreaChecker AreaChecker { get; set; }

        public StateableSprite StateableSprite { get; private set; }
        public ClickCondition ClickCondition { get; set; }

        public event UIEventHandler<UIButton> OnClick = delegate { };
        public event Func<UIButton, bool> CanClick = delegate { return true; };

        public event UIEventHandler<UIButton, Vector2> OnButtonDown = delegate { };
        public event UIEventHandler<UIButton, Vector2> OnButtonUp = delegate { };

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

            if (!(settings is UIButtonSettings buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);
            ClickCondition = buttonSettings.ClickCondition;

            StateableSprite = StateableSprite.Create(buttonSettings.ButtonSpriteStates);
            StateableSprite.StateChanged += (s, sprite) =>
            {
                if (sprite != null && sprite != Sprite)
                    Sprite = sprite;
            };

            base.ApplySettings(buttonSettings);
        }

        public void Click()
        {
            if (CanClick(this))
            {
                OnClick(this);
            }
        }

        public virtual void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            StateableSprite.SetHighlighted();

            if (CanClick(this) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                OnClick(this);
            }

            OnButtonDown(this, worldPos);
        }

        public virtual void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            StateableSprite.SetDefault();

            if (CanClick(this) && _isPressed &&
                ClickCondition == ClickCondition.BUTTON_UP && AreaChecker.InAreaShape(this, worldPos))
            {
                OnClick(this);
            }

            OnButtonUp(this, worldPos);
            _isPressed = false;
        }
    }
}
