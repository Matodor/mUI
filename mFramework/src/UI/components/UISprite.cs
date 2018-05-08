using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace mFramework.UI
{
    public class UISpriteSettings : UIComponentSettings
    {
        public virtual Sprite Sprite { get; set; }
        public virtual Color? Color { get; set; } = null;
    }

    public class UISprite : UIComponent, IUIRenderer<SpriteRenderer>, IUIRenderer, IUIColored
    {
        public override float UnscaledHeight => _spriteRenderer.sprite.bounds.size.y;
        public override float UnscaledWidth => _spriteRenderer.sprite.bounds.size.x;
        public override Vector2 CenterOffset => _spriteRenderer.sprite.bounds.center;

        public Color Color
        {
            get => _spriteRenderer.color;
            set => _spriteRenderer.color = value;
        }

        public float Opacity
        {
            get => _spriteRenderer.color.a;
            set
            {
                var color = _spriteRenderer.color;
                color.a = value;
                _spriteRenderer.color = color;
            }
        }

        public UISprite SpriteMask { get; private set; }

        public Sprite Sprite
        {
            get => _spriteRenderer.sprite;
            set => _spriteRenderer.sprite = value;
        }

        public SpriteRenderer UIRenderer => _spriteRenderer;
        Renderer IUIRenderer.UIRenderer => _spriteRenderer;

        private SpriteRenderer _spriteRenderer; 

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISpriteSettings spriteSettings))
                throw new ArgumentException("UISprite: The given settings is not UISpriteSettings");

            _spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = spriteSettings.Sprite;
            _spriteRenderer.sharedMaterial = UIStencilMaterials
                .GetOrCreate(ParentView.StencilId ?? 0)
                .SpritesMaterial;

            if (spriteSettings.Color.HasValue)
                Color = spriteSettings.Color.Value;

            SortingOrderChanged += OnSortingOrderChanged;
            base.ApplySettings(spriteSettings);
        }

        private void OnSortingOrderChanged(IUIObject sender)
        {
            _spriteRenderer.sortingOrder = SortingOrder;
        }

        public void Flip(bool flipX, bool flipY)
        {
            _spriteRenderer.flipX = flipX;
            _spriteRenderer.flipY = flipY;
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
                SpriteMask = this.Sprite(new UISpriteSettings
                {
                    Sprite = mask,
                });

                var layer = UIStencilMaterials.Create(1,
                    insideMask ? CompareFunction.Equal : CompareFunction.NotEqual, StencilOp.Replace, 1, 1);

                SpriteMask.UIRenderer.material = layer.CanvasMaterial;
                SpriteMask.UIRenderer.material.SetFloat("_UseUIAlphaClip", useAlphaClip ? 1f : 0f);
                SpriteMask.UIRenderer.material.SetFloat("_UseUIAlphaClip", useAlphaClip ? 1f : 0f);
                SpriteMask.UIRenderer.color = new Color32(255, 255, 255, 0);
                UIRenderer.material = layer.SpritesMaterial;
            }

            return SpriteMask;
        }
    }
}
