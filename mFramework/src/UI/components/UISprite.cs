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
        public Renderer UIRenderer { get { return Renderer; } }
        public SpriteRenderer Renderer { get; }

        private UISprite(UIObject parent) : base(parent)
        {
            Renderer = _gameObject.AddComponent<SpriteRenderer>();
            OnSortingOrderChanged += o =>
            {
                Renderer.sortingOrder = SortingOrder();
            };
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

        public override float GetHeight()
        {
            return Renderer.sprite?.WorldSize().y ?? base.GetHeight();
        }

        public override float GetWidth()
        {
            return Renderer.sprite?.WorldSize().x ?? base.GetWidth();
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

        public override UIRect GetRect()
        {
            var pos = Position();
            var scale = GlobalScale();
            var scaledHeightDiv2 = GetHeight() / 2 * scale.y;
            var scaledWidthDiv2 = GetWidth() / 2 * scale.x;
            var centerOffset = Renderer.sprite?.GetCenterOffset() ?? new Vector2(0, 0);

            return new UIRect()
            {
                Position = pos,
                Bottom = pos.y - scaledHeightDiv2 + centerOffset.y * scale.y,
                Top = pos.y + scaledHeightDiv2 + centerOffset.y * scale.y,
                Left = pos.x - scaledWidthDiv2 + centerOffset.x * scale.x,
                Right = pos.x + scaledWidthDiv2 + centerOffset.x * scale.x,
            };
        }

        public UIObject SetColor(UIColor color)
        {
            Renderer.color = color.Color32;
            return this;
        }
    }
}
