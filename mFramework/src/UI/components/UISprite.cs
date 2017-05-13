using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISpriteSettings
    {
        public Sprite Sprite { get; set; }
        public Color? Color { get; set; } = null;
    }

    public abstract partial class UIView
    {
        public UISprite UISprite(string sprite)
        {
            return this.UISprite(new UISpriteSettings
            {
                Sprite = SpritesRepository.Get(sprite),
            });
        }

        public UISprite UISprite(Sprite sprite)
        {
            return this.UISprite(new UISpriteSettings
            {
                Sprite = sprite,
            });
        }

        public UISprite UISprite(UISpriteSettings settings)
        {
            return AddComponent(UI.UISprite.Create(this, settings));
        }
    }
    
    public class UISprite : UIComponent
    {
        public SpriteRenderer Renderer { get; }

        private UISprite(UIView parentView) : base(parentView)
        {
            Renderer = _gameObject.AddComponent<SpriteRenderer>();
        }

        public static UISprite Create(UIView view, UISpriteSettings settings)
        {
            if (settings.Sprite == null)
                throw new NullReferenceException("UISPrite: The given sprite was null");
            if (view == null)
                throw new NullReferenceException("UISPrite: The given parentView was null");

            var uiSprite = new UISprite(view);
            uiSprite.Renderer.sprite = settings.Sprite;
            if (settings.Color.HasValue)
                uiSprite.SetColor(settings.Color.Value);

            return uiSprite;
        }

        public UISprite SetColor(Color color)
        {
            Renderer.color = color;
            return this;
        }
    }
}
