using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mFramework.src.UI;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIButtonSettings : UIComponentSettings
    {
        public SpriteStates ButtonSpriteStates { get; set; }
        public AreaType ButtonAreaType { get; set; } = AreaType.RECTANGLE;
    }

    public class UIButton : UIComponent, IUIClickable
    {
        private UIClickable _clickableHandler;
        private StateableSprite _stateableSprite;
        private UISprite _uiSprite;

        private UIButton(UIObject parent) : base(parent)
        {
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var buttonSettings = settings as UIButtonSettings;
            if (buttonSettings == null)
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            switch (buttonSettings.ButtonAreaType)
            {
                case AreaType.RECTANGLE:
                    SetupRectangleHandler();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _uiSprite = Component<UISprite>(new UISpriteSettings { Sprite = buttonSettings.ButtonSpriteStates.Default });
            _stateableSprite = StateableSprite.Create(buttonSettings.ButtonSpriteStates);
            _stateableSprite.OnStateChanged += sprite =>
            {
                if (sprite != null)
                {
                    _uiSprite.SetSprite(sprite);
                }
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
        }

        public void MouseUp(Vector2 worldPos)
        {
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }
    }
}
