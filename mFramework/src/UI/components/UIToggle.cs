using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIToggleSettings : UIButtonSettings
    {
        public bool DefaultSelected = false;
    }

    public class UIToggle : UIComponent, IUISpriteRenderer, IUIColored, IUIClickable
    {
        public UIClickable UIClickable { get; private set; }
        public Renderer UIRenderer => _uiSpriteRenderer.Renderer;

        public SpriteRenderer Renderer => _uiSpriteRenderer.Renderer;
        public IUISpriteRenderer SpriteMask => _uiSpriteRenderer.SpriteMask;

        public StateableSprite StateableSprite { get; private set; }
        public ClickCondition ClickCondition { get; set; }

        public bool IsSelected => _isSelected;

        public event Func<UIToggle, Vector2, bool> CanToggleClick = delegate { return true; };
        public event Func<UIToggle, bool> CanSelect = delegate { return true; };
        public event Func<UIToggle, bool> CanDeselect = delegate { return true; };

        public event UIEventHandler<UIToggle> Selected = delegate { };
        public event UIEventHandler<UIToggle> Deselected = delegate { };
        public event UIEventHandler<UIToggle> Changed = delegate { };

        private bool _isSelected;
        private bool _isPressed;

        private UISpriteRenderer _uiSpriteRenderer;

        protected override void Init()
        {
            _isSelected = false;
            _isPressed = false;
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIToggleSettings toggleSettings))
                throw new ArgumentException("UIToggle: The given settings is not UIToggleSettings");

            ClickCondition = toggleSettings.ClickCondition;

            if (toggleSettings.ButtonAreaType == AreaType.RECTANGLE)
            {
                var area = new RectangleArea2D();
                area.Update += a =>
                {
                    area.Width = GetWidth();
                    area.Height = GetHeight();
                    area.Offset = _uiSpriteRenderer.Renderer.sprite.GetCenterOffset();
                    area.Offset = new Vector2(
                        area.Offset.x * GlobalScale().x,
                        area.Offset.y * GlobalScale().y
                    );
                };

                UIClickable = new UIClickable(this, area);
            }
            else if (toggleSettings.ButtonAreaType == AreaType.CIRCLE)
            {
                var area = new CircleArea2D();
                area.Update += a =>
                {
                    area.Radius = GetWidth() / 2;
                    area.Offset = _uiSpriteRenderer.Renderer.sprite.GetCenterOffset();
                    area.Offset = new Vector2(
                        area.Offset.x * GlobalScale().x,
                        area.Offset.y * GlobalScale().y
                    );
                };
                UIClickable = new UIClickable(this, area);
            }

            _uiSpriteRenderer = new UISpriteRenderer(this, new UISpriteSettings
            {
                Sprite = toggleSettings.ButtonSpriteStates.Default
            });

            StateableSprite = StateableSprite.Create(toggleSettings.ButtonSpriteStates);
            StateableSprite.StateChanged += (s, sprite) =>
            {
                if (sprite != null && sprite != _uiSpriteRenderer.Renderer.sprite)
                    _uiSpriteRenderer.Renderer.sprite = sprite;
            };

            if (toggleSettings.DefaultSelected)
                Select();

            base.ApplySettings(settings);
        }

        public UIToggle Toggle()
        {
            if (_isSelected)
                Deselect();
            else
                Select();

            return this;
        }

        public UIToggle Select()
        {
            if (!CanSelect(this))
                return this;

            _isSelected = true;
            StateableSprite.SetSelected();

            Selected.Invoke(this);
            Changed.Invoke(this);
            return this;
        }

        public UIToggle Deselect()
        {
            if (!CanDeselect(this))
                return this;

            _isSelected = false;
            StateableSprite.SetDefault();

            Deselected.Invoke(this);
            Changed.Invoke(this);
            return this;
        }

        public Color GetColor()
        {
            return _uiSpriteRenderer.GetColor();
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

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            StateableSprite.SetHighlighted();

            if (CanToggleClick(this, worldPos) && ClickCondition == ClickCondition.BUTTON_DOWN)
            {
                Toggle();
            }
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;
            
            if (_isSelected)
                StateableSprite.SetSelected();
            else
                StateableSprite.SetDefault();

            if (CanToggleClick(this, worldPos) && _isPressed &&
                ClickCondition == ClickCondition.BUTTON_UP && UIClickable.Area2D.InArea(worldPos))
            {
                Toggle();
            }

            _isPressed = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
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

        public IUISpriteRenderer SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true)
        {
            return _uiSpriteRenderer.SetMask(mask, useAlphaClip, insideMask);
        }

        public override float UnscaledHeight()
        {
            return _uiSpriteRenderer.UnscaledHeight();
        }

        public override float UnscaledWidth()
        {
            return _uiSpriteRenderer.UnscaledWidth();
        }

        public override float GetHeight()
        {
            return _uiSpriteRenderer.GetHeight();
        }

        public override float GetWidth()
        {
            return _uiSpriteRenderer.GetWidth();
        }

        public override UIRect GetRect()
        {
            return _uiSpriteRenderer.GetRect();
        }
    }
}
