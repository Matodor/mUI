using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIButtonSettings : UIComponentSettings
    {
        public ClickCondition ClickCondition = ClickCondition.BUTTON_UP;
        public readonly SpriteStates ButtonSpriteStates = new SpriteStates();
        public AreaType ButtonAreaType = AreaType.RECTANGLE;
    }

    public enum ClickCondition
    {
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_PRESSED
    }

    public class UIButton : UIComponent, IUIButton, IUISpriteRenderer
    {
        public UIClickableOld UiClickableOld { get; private set; }
        public Renderer UIRenderer => _uiSpriteRenderer.Renderer;

        public SpriteRenderer Renderer => _uiSpriteRenderer.Renderer;
        public UISprite SpriteMask => _uiSpriteRenderer.SpriteMask;
        
        public StateableSprite StateableSprite { get; private set; }
        public ClickCondition ClickCondition { get; set; }

        public event UIEventHandler<IUIButton> Click = delegate { };
        public event Func<IUIButton, Vector2, bool> CanButtonClick = delegate { return true; };

        public event UIEventHandler<IUIButton, Vector2> ButtonDown = delegate { };
        public event UIEventHandler<IUIButton, Vector2> ButtonUp = delegate { };

        public override float UnscaledHeight => _uiSpriteRenderer.UnscaledHeight();
        public override float UnscaledWidth => _uiSpriteRenderer.UnscaledWidth();
        public override Vector2 CenterOffset => _uiSpriteRenderer.CenterOffset();

        private UISpriteRenderer _uiSpriteRenderer;
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

            ClickCondition = buttonSettings.ClickCondition;

            if (buttonSettings.ButtonAreaType == AreaType.RECTANGLE)
            {
                var area = new RectangleArea2D();
                area.Update += a =>
                {
                    area.Width = Width;
                    area.Height = Height;
                    area.Offset = _uiSpriteRenderer.Renderer.sprite.GetCenterOffset();
                    area.Offset = new Vector2(
                        area.Offset.x * GlobalScale.x,
                        area.Offset.y * GlobalScale.y
                    );
                };

                UiClickableOld = new UIClickableOld(this, area);
            }
            else if (buttonSettings.ButtonAreaType == AreaType.CIRCLE)
            {
                var area = new CircleArea2D();
                area.Update += a =>
                {
                    area.Radius = Width / 2;
                    area.Offset = _uiSpriteRenderer.Renderer.sprite.GetCenterOffset();
                    area.Offset = new Vector2(
                        area.Offset.x * GlobalScale.x,
                        area.Offset.y * GlobalScale.y
                    );
                };
                UiClickableOld = new UIClickableOld(this, area);
            }

            _uiSpriteRenderer = new UISpriteRenderer(this, new UISpriteSettings
            {
                Sprite = buttonSettings.ButtonSpriteStates.Default
            });

            StateableSprite = StateableSprite.Create(buttonSettings.ButtonSpriteStates);
            StateableSprite.StateChanged += (s, sprite) =>
            {
                if (sprite != null && sprite != _uiSpriteRenderer.Renderer.sprite)
                    _uiSpriteRenderer.Renderer.sprite = sprite;
            };

            base.ApplySettings(buttonSettings);
        }

        public void Flip(bool flipX, bool flipY)
        {
            _uiSpriteRenderer.Flip(flipX, flipY);
        }

        public void SetSprite(Sprite sprite)
        {
            _uiSpriteRenderer.SetSprite(sprite);
        }

        public void RemoveMask()
        {
            _uiSpriteRenderer.RemoveMask();
        }

        public UISprite SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true)
        {
            return _uiSpriteRenderer.SetMask(mask, useAlphaClip, insideMask);
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            StateableSprite.SetHighlighted();

            if (CanButtonClick(this, worldPos) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                Click(this);
            }

            ButtonDown(this, worldPos);
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            StateableSprite.SetDefault();

            if (CanButtonClick(this, worldPos) && _isPressed &&
                ClickCondition == ClickCondition.BUTTON_UP && UiClickableOld.Area2D.InArea(worldPos))
            {
                Click(this);
            }

            ButtonUp(this, worldPos);
            _isPressed = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }

        public Color GetColor()
        {
            return _uiSpriteRenderer.GetColor();
        }

        public float GetOpacity()
        {
            return _uiSpriteRenderer.GetOpacity();
        }

        public IUIColored SetColor(Color32 color)
        {
            _uiSpriteRenderer.SetColor(color);
            return this;
        }

        public IUIColored SetColor(UIColor color)
        {
            _uiSpriteRenderer.SetColor(color);
            return this;
        }

        public IUIColored SetOpacity(float opacity)
        {
            _uiSpriteRenderer.SetOpacity(opacity);
            return this;
        }
    }
}
