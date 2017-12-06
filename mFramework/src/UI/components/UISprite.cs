using System;
using UnityEngine;

namespace mFramework.UI
{ 
    public class UISprite : UIComponent, IUISpriteRenderer, IUIColored
    {
        public SpriteRenderer Renderer => _uiSpriteRenderer.Renderer;
        public UISprite SpriteMask => _uiSpriteRenderer.SpriteMask;

        private UISpriteRenderer _uiSpriteRenderer;

        protected override void Init()
        {
            base.Init();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISpriteSettings spriteSettings))
                throw new ArgumentException("UISPrite: The given settings is not UISpriteSettings");

            _uiSpriteRenderer = new UISpriteRenderer(this, spriteSettings);

            base.ApplySettings(spriteSettings);
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
    }
}
