using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UIButtonProps : UISpriteProps
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
        public event UIMouseEvent MouseDown = delegate { };
        public event UIMouseEvent MouseUp = delegate { };

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

        public event UIButtonClickEvent Clicked = delegate { };
        public event UIButtonAllowClick CanClick
        {
            add => _canClickButtonEvents.Add(value);
            remove => _canClickButtonEvents.Remove(value);
        }

        public bool IgnoreByHandler { get; set; }
        public bool IsPressed { get; protected set; }

        public IAreaChecker AreaChecker { get; set; }
        public ClickCondition ClickCondition { get; set; }

        public StateableSprite StateableSprite { get; private set; }

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

            StateableSprite.StateChanged -= StateableSpriteOnStateChanged;

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
            if (!(props is UIButtonProps buttonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");
            
            ClickCondition = buttonSettings.ClickCondition;
            StateableSprite = StateableSprite.Create(buttonSettings.SpriteStates);
            StateableSprite.StateChanged += StateableSpriteOnStateChanged;
            base.ApplyProps(buttonSettings);
        }

        private void StateableSpriteOnStateChanged(StateableSprite sender, Sprite sprite)
        {
            if (sprite != null && sprite != Sprite)
                Sprite = sprite;
        }

        public bool Click()
        {
            if (_canClickButtonEvents.Count != 0 &&
                !_canClickButtonEvents.TrueForAll(e => e(this)))
            {
                return false;
            }

            Clicked(this);
            return true;
        }
        
        private void OnUIMouseDown()
        {
            StateableSprite.SetHighlighted();

            if (ClickCondition == ClickCondition.BUTTON_DOWN)
                Click();
        }

        private void OnUIMouseUp(Vector2 worldPos)
        {
            StateableSprite.SetDefault();

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
            MouseDown(this, worldPos);
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
            MouseUp(this, worldPos);
            IsPressed = false;
        }
    }
}
