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
    
    public sealed class UISprite : UIComponent
    {
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
            if (!(settings is UISpriteSettings))
                throw new NullReferenceException("UISPrite: The given settings is not UISpriteSettings");
            if (settings == null)
                throw new NullReferenceException("UISPrite: The given settings was null");

            UISpriteSettings uiSpriteSettings = (UISpriteSettings) settings;
            if (uiSpriteSettings.Sprite == null)
                throw new NullReferenceException("UISPrite: The given sprite was null");

            Renderer.sprite = uiSpriteSettings.Sprite;
            if (uiSpriteSettings.Color.HasValue)
                SetColor(uiSpriteSettings.Color.Value);
        }

        public UISprite SetColor(Color color)
        {
            Renderer.color = color;
            return this;
        }
    }
}
