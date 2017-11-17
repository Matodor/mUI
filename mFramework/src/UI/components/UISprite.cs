using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace mFramework.UI
{
    public sealed class UISpriteSettings : UIComponentSettings
    {
        public Sprite Sprite { get; set; }
        public Color? Color { get; set; } = null;
    }
    
    public class UISprite : UIComponent, IUIRenderer, IColored, IMaskable
    {
        public Renderer UIRenderer => Renderer;
        public SpriteRenderer Renderer { get; private set; }
        public UISprite SpriteMask { get; private set; }
        
        protected override void Init()
        {
            SpriteMask = null;
            Renderer = gameObject.AddComponent<SpriteRenderer>();

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
            Renderer.sharedMaterial = UIStencilMaterials.GetOrCreate(InternalParentView.StencilId ?? 0).SpritesMaterial;

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

        public UISprite SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true)
        {
            if (SpriteMask == null)
            {
                SpriteMask = this.Sprite(new UISpriteSettings
                {
                    Sprite = mask,
                });

                var layer = UIStencilMaterials.Create(1, 
                    insideMask ? CompareFunction.Equal : CompareFunction.NotEqual, StencilOp.Replace, 1, 1);

                SpriteMask.Renderer.material = layer.CanvasMaterial;
                SpriteMask.Renderer.material.SetFloat("_UseUIAlphaClip", useAlphaClip ? 1f : 0f);
                SpriteMask.Renderer.material.SetFloat("_UseUIAlphaClip", useAlphaClip ? 1f : 0f);
                SpriteMask.Renderer.color = new Color32(255, 255, 255, 1);
                Renderer.material = layer.SpritesMaterial;
            }

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
