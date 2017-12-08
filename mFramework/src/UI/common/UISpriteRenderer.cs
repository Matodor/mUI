using UnityEngine;
using UnityEngine.Rendering;

namespace mFramework.UI
{
    public sealed class UISpriteSettings : UIComponentSettings
    {
        public Sprite Sprite { get; set; }
        public Color? Color { get; set; } = null;
    }

    public sealed class UISpriteRenderer
    {
        public SpriteRenderer Renderer { get; }
        public UISprite SpriteMask { get; private set; }

        private readonly UIObject _object;
        
        public UISpriteRenderer(UIObject obj, UISpriteSettings settings)
        {
            Renderer = obj.gameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = settings.Sprite;
            Renderer.sharedMaterial = UIStencilMaterials.GetOrCreate(obj.ParentView.StencilId ?? 0).SpritesMaterial;

            if (settings.Color.HasValue)
                SetColor(settings.Color.Value);

            _object = obj;

            obj.SortingOrderChanged += sender =>
            {
                Renderer.sortingOrder = sender.SortingOrder();
            };
        }

        public void Flip(bool flipX, bool flipY)
        {
            Renderer.flipX = flipX;
            Renderer.flipY = flipY;
        }

        public void SetSprite(Sprite sprite)
        {
            Renderer.sprite = sprite;
        }

        public void RemoveMask()
        {
            if (SpriteMask != null)
            {
                SpriteMask.Destroy();
                SpriteMask = null;
            }
        }

        public UISprite SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true)
        {
            if (SpriteMask == null)
            {
                SpriteMask = _object.Sprite(new UISpriteSettings
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

        public float GetHeight()
        {
            return Renderer.sprite.bounds.size.y * _object.GlobalScale().y;
        }

        public float GetWidth()
        {
            return Renderer.sprite.bounds.size.x * _object.GlobalScale().x;
        }

        public UIRect GetRect()
        {
            var pos = _object.Pos();
            var scale = _object.GlobalScale();
            var scaledHeightDiv2 = GetHeight() / 2f;
            var scaledWidthDiv2 = GetWidth() / 2f;
            var centerOffset = Renderer.sprite.GetCenterOffset();

            return new UIRect()
            {
                Position = pos,
                Bottom = pos.y - scaledHeightDiv2 + centerOffset.y * scale.y,
                Top = pos.y + scaledHeightDiv2 + centerOffset.y * scale.y,
                Left = pos.x - scaledWidthDiv2 + centerOffset.x * scale.x,
                Right = pos.x + scaledWidthDiv2 + centerOffset.x * scale.x,
            };
        }

        public Color GetColor()
        {
            return Renderer.color;
        }

        public void SetColor(Color32 color)
        {
            Renderer.color = color;
        }

        public void SetColor(UIColor color)
        {
            Renderer.color = color.Color32;
        }
    }
}