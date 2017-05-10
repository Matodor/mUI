using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Components
{
    public static partial class UIComponentsHelper
    {
        public static UISprite CreateSprite(this UIObject obj, Sprite sprite, string objName = "Sprite")
        {
            return new UISprite(obj, sprite).SetName(objName);
        }
    }

    public class UISprite : UIClickableObj
    {
        public override float PureWidth { get { return ((SpriteRenderer)Renderer).sprite.bounds.size.x; } }
        public override float PureHeight { get { return ((SpriteRenderer)Renderer).sprite.bounds.size.y; } }

        public UISprite(UIObject view, Sprite sprite) : base(view)
        {
            ((SpriteRenderer)Renderer).sprite = sprite;
        }

        public Sprite CurrentSprite()
        {
            return ((SpriteRenderer) Renderer).sprite;
        }

        public UISprite Sprite(Sprite sprite)
        {
            ((SpriteRenderer)Renderer).sprite = sprite;
            return this;
        }

        public override bool InArea(Vector2 screenPos)
        {
            return AreaChecker.InArea(Transform, mUI.UICamera.ScreenToWorldPoint(screenPos),
                   ((SpriteRenderer)Renderer).sprite?.bounds ?? new Bounds(new Vector3(0, 0), new Vector3(1, 1)));
        }
    }
}
