using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Input;
using mUIApp.Views;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.src.Views.Elements
{
    public static partial class UIElementsHelper
    {
        public static UIScrollBar CreateScrollBar(this UIObject obj, Sprite bar, Sprite point, string objName = "Scroll bar")
        {
            return new UIScrollBar(obj, bar, point).SetName(objName);
        }
    }

    public class UIScrollBar : UIClickableObj
    {
        public override float PureWidth { get { return 0; } }
        public override float PureHeight { get { return 0; } }

        private UISprite _bar, _point;

        public UIScrollBar(UIObject obj, Sprite barSprite, Sprite pointSprite) : base(obj)
        {
            OnUIMouseDownEvent += MouseDownEvent;
            OnUIMouseDragEvent += MouseDragEvent; 
            OnUIMouseUpEvent += MouseUpEvent;

            _bar = this.CreateSprite(barSprite);
            _point = _bar.CreateSprite(pointSprite);
        }

        private void MouseUpEvent(UIObject uiObject, mUIMouseEvent mUiMouseEvent)
        {
            
        }

        private void MouseDragEvent(UIObject uiObject, mUIMouseEvent mUiMouseEvent)
        {
            
        }

        private void MouseDownEvent(UIObject uiObject, mUIMouseEvent mUiMouseEvent)
        {
            
        }

        public override bool InArea(Vector2 screenPos)
        {
            return false;
        }
    }
}
