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
        public static UIButton CreateButton(this BaseView view, Sprite sprite, string objName = "sprite")
        {
            return new UIButton(view, sprite, null).SetName(objName);
        }

        public static UIButton CreateButton(this BaseView view, Sprite sprite, Sprite hoverSprite, string objName = "sprite")
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
        COUNT_STATES,
    }

    public class UIButton : UIClickableObj
    {
        public override float Width { get { return Renderer.sprite.bounds.size.x*Transform.lossyScale.x; } }
        public override float Height { get { return Renderer.sprite.bounds.size.y * Transform.lossyScale.y; } }

        protected readonly Sprite[] _stateSprites;
        private UIButtonState _uiButtonState;
        private UIClickCondition _uiClickCondition;
        private event Action<object> _onButtonClick;
        private object _onButtonClickArgs;
        private float _lastClickTime;

        public UIButton(BaseView view, Sprite sprite, Sprite hoverSprite) : base(view)
        {
            _onButtonClickArgs = null;
            _uiButtonState = UIButtonState.ACTIVE;
            _uiClickCondition = UIClickCondition.BUTTON_UP;
            _stateSprites = new Sprite[(int)UIButtonState.COUNT_STATES];
            _stateSprites[(int) UIButtonState.ACTIVE] = sprite;
            _stateSprites[(int) UIButtonState.HOVER] = hoverSprite;
            Renderer.sprite = _stateSprites[(int) UIButtonState.ACTIVE];

            OnUIMouseDownEvent += OnButtonDown;
            OnUIMouseUpEvent += OnButtonUp;

            this.SetBoxArea(Renderer.sprite?.bounds ?? new Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 0)));
        }

        public UIButton ClickCondition(UIClickCondition condition)
        {
            _uiClickCondition = condition;
            return this;
        }

        public UIButton Click(Action<object> onClick, object args = null)
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
            _onButtonClick?.Invoke(_onButtonClickArgs);
        }

        private void UpdateSprite()
        {
            Renderer.sprite = _stateSprites[(int)_uiButtonState];
        }
    }
}
