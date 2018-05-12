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
        public override float UnscaledHeight => UIRenderer.sprite.bounds.size.y;
        public override float UnscaledWidth => UIRenderer.sprite.bounds.size.x;
        public override Vector2 CenterOffset => UIRenderer.sprite.bounds.center;

        public Color Color
        {
            get => UIRenderer.color;
            set => UIRenderer.color = value;
        }

        public float Opacity
        {
            get => UIRenderer.color.a;
            set
            {
                var color = UIRenderer.color;
                color.a = value;
                UIRenderer.color = color;
            }
        }

        public UISprite SpriteMask { get; private set; }

        public Sprite Sprite
        {
            get => UIRenderer.sprite;
            set => UIRenderer.sprite = value;
        }

        public SpriteRenderer UIRenderer { get; private set; }
        Renderer IUIRenderer.UIRenderer => UIRenderer;

        protected override void AfterAwake()
        {
            UIRenderer = GameObject.AddComponent<SpriteRenderer>();
            base.AfterAwake();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISpriteSettings spriteSettings))
                throw new ArgumentException("UISprite: The given settings is not UISpriteSettings");

            UIRenderer.sprite = spriteSettings.Sprite;
            UIRenderer.sharedMaterial = UIStencilMaterials
                .GetOrCreate(ParentView.StencilId ?? 0)
                .SpritesMaterial;

            if (spriteSettings.Color.HasValue)
                Color = spriteSettings.Color.Value;

            SortingOrderChanged += OnSortingOrderChanged;
            base.ApplySettings(spriteSettings);
        }

        private void OnSortingOrderChanged(IUIObject sender)
        {
            UIRenderer.sortingOrder = SortingOrder;
        }

        public void Flip(bool flipX, bool flipY)
        {
            UIRenderer.flipX = flipX;
            UIRenderer.flipY = flipY;
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
