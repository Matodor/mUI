using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISpriteSettings : UIComponentSettings
    {
        public Sprite Sprite { get; set; }
        public Color? Color { get; set; } = null;
    }
    
    public class UISprite : UIComponent, IUIRenderer, IColored
    {
        public Renderer UIRenderer => Renderer;
        public SpriteRenderer Renderer { get; }
        public SpriteMask SpriteMask { get; private set; }

        protected UISprite(UIObject parent) : base(parent)
        {
            SpriteMask = null;
            Renderer = _gameObject.AddComponent<SpriteRenderer>();
            SortingOrderChanged += s => Renderer.sortingOrder = SortingOrder();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var spriteSettings = settings as UISpriteSettings;
            if (spriteSettings == null)
                throw new ArgumentException("UISPrite: The given settings is not UISpriteSettings");
          
            Renderer.sprite = spriteSettings.Sprite;
            if (spriteSettings.Color.HasValue)
                SetColor(spriteSettings.Color.Value);

            base.ApplySettings(spriteSettings);
        }

        public UISprite Flip(bool flipX, bool flipY)
        {
            Renderer.flipX = flipX;
            Renderer.flipY = flipY;
            return this;
        }

        public UISprite RemoveMask()
        {
            if (SpriteMask != null)
                UnityEngine.Object.Destroy(SpriteMask.gameObject);
            return this;
        }

        public UISprite SetMask(Sprite mask)
        {
            if (SpriteMask == null)
            {
                SpriteMask = new GameObject("SpriteMask")
                    .SetParent(_gameObject)
                    .AddComponent<SpriteMask>();
            }

            SpriteMask.sprite = mask;
            return this;
        }

        public override float GetHeight()
        {
            return (Renderer.sprite?.WorldSize().y ?? 0) * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return (Renderer.sprite?.WorldSize().x ?? 0) * GlobalScale().x;
        }

        public UISprite SetSprite(Sprite sprite)
        {
            Renderer.sprite = sprite;
            return this;
        }

        public UIObject SetColor(Color32 color)
        {
            Renderer.color = color;
            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            Renderer.color = color.Color32;
            return this;
        }

        public override UIRect GetRect()
        {
            var pos = Position();
            var scale = GlobalScale();
            var scaledHeightDiv2 = GetHeight() / 2f;
            var scaledWidthDiv2 = GetWidth() / 2f;
            var centerOffset = Renderer.sprite?.GetCenterOffset() ?? new Vector2(0f, 0f);

            return new UIRect()
            {
                Position = pos,
                Bottom = pos.y - scaledHeightDiv2 + centerOffset.y * scale.y,
                Top = pos.y + scaledHeightDiv2 + centerOffset.y * scale.y,
                Left = pos.x - scaledWidthDiv2 + centerOffset.x * scale.x,
                Right = pos.x + scaledWidthDiv2 + centerOffset.x * scale.x,
            };
        }
    }
}
