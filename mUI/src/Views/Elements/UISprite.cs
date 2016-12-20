using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static partial class UIElementsHelper
    {
        public static UISprite CreateSprite(this UIObject obj, Sprite sprite, string objName = "Sprite")
        {
            return new UISprite(obj, sprite).SetName(objName);
        }
    }

    public class UISprite : UIObject
    {
        public override float PureWidth { get { return ((SpriteRenderer)Renderer).sprite.bounds.size.x; } }
        public override float PureHeight { get { return ((SpriteRenderer)Renderer).sprite.bounds.size.y; } }

        public UISprite(UIObject view, Sprite sprite) : base(view)
        {
            ((SpriteRenderer)Renderer).sprite = sprite;
        }

        public UISprite Sprite(Sprite sprite)
        {
            ((SpriteRenderer)Renderer).sprite = sprite;
            return this;
        }
    }
}
