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
    }
    
    public class UIButton : UISprite, IUIButton
    {
        public event UIMouseEvent MouseDown;
        public event UIMouseEvent MouseUp;

        public event UIMouseAllowEvent CanMouseDown = delegate { return true; };
        public event UIMouseAllowEvent CanMouseUp = delegate { return true; };

        public event UIButtonClickEvent OnClick = delegate { };
        public event UIButtonAllowClick CanClick = delegate { return true; };

        public bool IsPressed { get; protected set; }
        public IAreaChecker AreaChecker { get; set; }
        public ClickCondition ClickCondition { get; set; }

        public StateableSprite StateableSprite { get; private set; }
        
        protected override void AfterAwake()
        {
            MouseDown += OnUIMouseDown;
            MouseUp += OnUIMouseUp;
            CanMouseDown += OnUICanMouseDown;
            CanMouseUp += OnUICanMouseUp;

            IsPressed = false;
            ClickCondition = ClickCondition.BUTTON_UP;

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddClickable(this);

            base.AfterAwake();
        }

        private bool OnUICanMouseUp(IUIClickable sender, ref Vector2 worldPos)
        {
            return IsPressed;
        }

        private bool OnUICanMouseDown(IUIClickable sender, ref Vector2 worldPos)
        {
            return !IsPressed && AreaChecker.InAreaShape(this, worldPos);
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIButtonSettings buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");
            
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
        
        private void OnUIMouseDown(IUIClickable clickable, ref Vector2 worldPos)
        {
            StateableSprite.SetHighlighted();

            if (CanClick(this) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                OnClick(this);
            }
        }

        public void DoMouseDown(Vector2 worldPos)
        {
            if (!CanMouseDown(this, ref worldPos))
                return;

            IsPressed = true;
            // ReSharper disable once PossibleNullReferenceException
            MouseDown(this, ref worldPos);
        }

        private void OnUIMouseUp(IUIClickable clickable, ref Vector2 worldPos)
        {
            StateableSprite.SetDefault();

            if (CanClick(this) && ClickCondition == ClickCondition.BUTTON_UP &&
                AreaChecker.InAreaShape(this, worldPos))
            {
                OnClick(this);
            }
        }

        public void DoMouseUp(Vector2 worldPos)
        {
            if (!CanMouseUp(this, ref worldPos))
                return;

            // ReSharper disable once PossibleNullReferenceException
            MouseUp(this, ref worldPos);
            IsPressed = false;
        }
    }
}
