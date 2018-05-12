using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIButtonSettings : UISpriteSettings
    {
        public virtual ClickCondition ClickCondition { get; set; } = ClickCondition.BUTTON_UP;
        public virtual SpriteStates SpriteStates { get; set; }

        public override Sprite Sprite
        {
            get => SpriteStates.Default;
            set
            {
                var spriteStates = SpriteStates;
                spriteStates.Default = value;
                SpriteStates = spriteStates;
            }
        }
    }

    public enum ClickCondition
    {
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_PRESSED
    }
    
    public class UIButton : UISprite, IUIButton
    {
        public StateableSprite StateableSprite { get; private set; }

        public IAreaChecker AreaChecker { get; set; }
        public ClickCondition ClickCondition { get; set; }

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

            if (!(settings is UIButtonSettings buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);
            ClickCondition = buttonSettings.ClickCondition;

            StateableSprite = StateableSprite.Create(buttonSettings.SpriteStates);
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

            ButtonDown(this, worldPos);
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

            ButtonUp(this, worldPos);
            _isPressed = false;
        }
    }
}
