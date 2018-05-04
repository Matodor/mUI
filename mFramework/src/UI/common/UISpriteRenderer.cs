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
            Renderer = obj.GameObject.AddComponent<SpriteRenderer>();
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
                SpriteMask.Renderer.color = new Color32(255, 255, 255, 0);
                Renderer.material = layer.SpritesMaterial;
            }

            return SpriteMask;
        }

        public float UnscaledHeight()
        {
            return Renderer.sprite.bounds.size.y;
        }

        public float UnscaledWidth()
        {
            return Renderer.sprite.bounds.size.x;
        }

        public Vector3 CenterOffset()
        {
            return Renderer.sprite.bounds.center;
        }

        public Color GetColor()
        {
            return Renderer.color;
        }

        public float GetOpacity()
        {
            return Renderer.color.a * 255f;
        }

        public void SetOpacity(float opacity)
        {
            var c = GetColor();
            c.a = opacity / 255f;
            SetColor(c);
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