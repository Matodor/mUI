using System;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISpriteSettings : UIComponentSettings
    {
        public Sprite Sprite { get; set; }
        public Color? Color { get; set; } = null;
    }
    
    public class UISprite : UIComponent, IUIRenderer, IColored, IMaskable
    {
        public static Material DefaultMaterial { get; private set; }

        public Renderer UIRenderer => Renderer;
        public SpriteRenderer Renderer { get; private set; }
        public SpriteMask SpriteMask { get; private set; }
        
        protected override void Init()
        {
            if (DefaultMaterial == null)
            {
                DefaultMaterial = new Material(Shader.Find("UI/Default")) {color = Color.white};
            }

            SpriteMask = null;
            Renderer = gameObject.AddComponent<SpriteRenderer>();
            Renderer.sharedMaterial = DefaultMaterial;

            SortingOrderChanged += s =>
            {
                Renderer.sortingOrder = SortingOrder();
            };
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISpriteSettings spriteSettings))
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

        public void RemoveMask()
        {
            if (SpriteMask != null)
            {
                UnityEngine.Object.Destroy(SpriteMask.gameObject);
                SpriteMask = null;
            }
        }

        public SpriteMask SetMask(Sprite mask)
        {
            if (SpriteMask == null)
            {
                SpriteMask = new GameObject("SpriteMask")
                    .SetParentTransform(gameObject)
                    .AddComponent<SpriteMask>();
            }

            SpriteMask.sprite = mask;
            return SpriteMask;
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

        public Color GetColor()
        {
            return Renderer.color;
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
