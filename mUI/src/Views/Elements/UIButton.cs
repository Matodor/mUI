using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Input;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static class UIButtonHelper
    {
        public static UIButton CreateButton(this BaseView view, Sprite sprite, string objName = "Button")
        {
            return new UIButton(view, sprite, null).SetName(objName);
        }

        public static UIButton CreateButton(this BaseView view, Sprite sprite, Sprite hoverSprite, string objName = "Button")
        {
            return new UIButton(view, sprite, hoverSprite).SetName(objName);
        }
    }

    public enum UIClickCondition
    {
        BUTTON_DOWN,
        BUTTON_UP,
        BUTTON_PRESSED,
    }

    public enum UIButtonState
    {
        DISABLED = 0,
        ACTIVE,
        HOVER,
    }

    public class UIButton : UIClickableObj
    {
        public override float Width { get { return ((SpriteRenderer)Renderer).sprite.bounds.size.x*Transform.lossyScale.x; } }
        public override float Height { get { return ((SpriteRenderer)Renderer).sprite.bounds.size.y * Transform.lossyScale.y; } }

        private readonly Sprite[] _stateSprites;
        private UIButtonState _uiButtonState;
        private UIClickCondition _uiClickCondition;
        private event Action<UIObject, object> _onButtonClick;
        private object _onButtonClickArgs;
        private float _lastClickTime;

        public UIButton(BaseView view, Sprite sprite, Sprite hoverSprite = null) : base(view)
        {
            _onButtonClickArgs = null;
            _uiButtonState = UIButtonState.ACTIVE;
            _uiClickCondition = UIClickCondition.BUTTON_UP;
            _stateSprites = new Sprite[3];
            _stateSprites[(int) UIButtonState.ACTIVE] = sprite;
            _stateSprites[(int) UIButtonState.HOVER] = hoverSprite;

            UpdateSprite();
            OnUIMouseDownEvent += OnButtonDown;
            OnUIMouseUpEvent += OnButtonUp;

            this.SetBoxArea();
        }
        
        public virtual UIButton ClickCondition(UIClickCondition condition)
        {
            _uiClickCondition = condition;
            return this;
        }

        public virtual UIButton Click(Action<UIObject, object> onClick, object args = null)
        {
            _onButtonClick += onClick;
            _onButtonClickArgs = args;
            return this;
        }

        private void OnButtonDown(mUIMouseEvent mouseEvent)
        {
            _uiButtonState = UIButtonState.HOVER;
            UpdateSprite();

            if (_uiClickCondition == UIClickCondition.BUTTON_DOWN || _uiClickCondition == UIClickCondition.BUTTON_PRESSED)
                Click();
        }

        private void OnButtonUp(mUIMouseEvent mouseEvent)
        {
            if (_uiButtonState != UIButtonState.HOVER)
                return;

            _uiButtonState = UIButtonState.ACTIVE;
            UpdateSprite();

            if (InArea(mouseEvent.MouseScreenPos))
            {
                if (_uiClickCondition == UIClickCondition.BUTTON_UP || _uiClickCondition == UIClickCondition.BUTTON_PRESSED)
                    Click();
            }
        }

        private void Click()
        {
            _lastClickTime = Time.time;
            _onButtonClick?.Invoke(this, _onButtonClickArgs);
        }

        public UIButton UpdateSprite(Sprite sprite, UIButtonState state)
        {
            if (sprite != null)
            {
                _stateSprites[(int) state] = sprite;
                UpdateSprite();
            }

            return this;
        }

        private void UpdateSprite()
        {
            if (_stateSprites[(int)_uiButtonState] != null)
                ((SpriteRenderer)Renderer).sprite = _stateSprites[(int)_uiButtonState];
        }
    }
}
