﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static class UISpriteHelper
    {
        public static UISprite CreateSprite(this BaseView view, Sprite sprite, string objName = "sprite")
        {
            return new UISprite(view, sprite).SetName(objName);
        }
    }

    public class UISprite : UIObject
    {
        public override float Width { get { return Renderer.sprite.bounds.size.x * Transform.lossyScale.x; } }
        public override float Height { get { return Renderer.sprite.bounds.size.y * Transform.lossyScale.y; } }

        public UISprite(BaseView view, Sprite sprite) : base(view)
        {
            Renderer.sprite = sprite;
        }
    }
}
