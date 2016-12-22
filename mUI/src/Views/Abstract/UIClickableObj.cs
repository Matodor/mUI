﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Input;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public abstract class UIClickable
    {
        public float Scale { get; set; }
        public abstract bool InArea(Transform transform, Vector2 clickPos, Bounds bounds);

        protected UIClickable()
        {
            Scale = 1;
        }
    }

    public class UIClickableCircle : UIClickable
    {
        public override bool InArea(Transform transform, Vector2 clickPos, Bounds bounds)
        {
            return false;
        }
    }

    public class UIClickableBox : UIClickable
    {
        public override bool InArea(Transform transform, Vector2 clickPos, Bounds bounds)
        {
            var offset = mUI.GetSpriteOffset(bounds);
            var cosinus = Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z);
            var sinus = Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z);
            var pos = transform.position;
            float scaledX = transform.lossyScale.x*Scale;
            float scaledY = transform.lossyScale.y*Scale;

            var leftTop = new Vector2(-bounds.extents.x + offset.x, bounds.extents.y + offset.y);
            leftTop.x *= scaledX;
            leftTop.y *= scaledY;
            leftTop = new Vector2(pos.x + leftTop.x * cosinus - leftTop.y * sinus, pos.y + leftTop.x * sinus + leftTop.y * cosinus);

            var rightTop = new Vector2(bounds.extents.x + offset.x, bounds.extents.y + offset.y);
            rightTop.x *= scaledX;
            rightTop.y *= scaledY;
            rightTop = new Vector2(pos.x + rightTop.x * cosinus - rightTop.y * sinus, pos.y + rightTop.x * sinus + rightTop.y * cosinus);
            
            var rightBottom = new Vector2(bounds.extents.x + offset.x, -bounds.extents.y + offset.y);
            rightBottom.x *= scaledX;
            rightBottom.y *= scaledY;
            rightBottom = new Vector2(pos.x + rightBottom.x * cosinus - rightBottom.y * sinus, pos.y + rightBottom.x * sinus + rightBottom.y * cosinus);

            var leftBottom = new Vector2(-bounds.extents.x + offset.x, -bounds.extents.y + offset.y);
            leftBottom.x *= scaledX;
            leftBottom.y *= scaledY;
            leftBottom = new Vector2(pos.x + leftBottom.x * cosinus - leftBottom.y * sinus, pos.y + leftBottom.x * sinus + leftBottom.y * cosinus);

            if (mUI.Debug)
            {
                Debug.DrawLine(leftTop, rightTop);
                Debug.DrawLine(rightTop, rightBottom);
                Debug.DrawLine(rightBottom, leftBottom);
                Debug.DrawLine(leftBottom, leftTop);
            }

            return 
                mUI.TriangleContainsPoint(leftTop, leftBottom, rightBottom, clickPos) || 
                mUI.TriangleContainsPoint(rightBottom, rightTop, leftTop, clickPos);
        }
    }

    public static class UIClickableObjHelper
    {
        public static T SetCircleArea<T>(this T clickableObj) where T : UIClickableObj
        {
            clickableObj.AreaChecker = new UIClickableCircle();
            return clickableObj;
        }

        public static T SetBoxArea<T>(this T clickableObj) where T : UIClickableObj
        {
            clickableObj.AreaChecker = new UIClickableBox();
            return clickableObj;
        }
    }

    public abstract class UIClickableObj : UIObject
    {
        public UIClickable AreaChecker { get; set; }
        public bool IgnoreSortingOrder { get; set; } = false;
        public Func<Vector2, bool> CanClick { get; set; }

        public event Action<UIObject, mUIMouseEvent> OnUIMouseDownEvent;
        public event Action<UIObject, mUIMouseEvent> OnUIMouseUpEvent;
        public event Action<UIObject, mUIMouseEvent> OnUIMouseDragEvent;

        protected UIClickableObj(UIObject obj) : base(obj)
        {
            this.SetBoxArea();

            mUI.UIInput.UIClickableObjList.Add(this);
            OnDestroy += DestroyClickable;
            CanClick = (v) => Active && InArea(v);
        }

        private void DestroyClickable(UIObject uiObject)
        {
            mUI.UIInput.UIClickableObjList.Remove(this);
        }
         
        ~UIClickableObj()
        {
            mUI.UIInput.UIClickableObjList.Remove(this);
        }

        public abstract bool InArea(Vector2 screenPos);

        public void OnUIMouseDown(mUIMouseEvent mouseEvent)
        {
            OnUIMouseDownEvent?.Invoke(this, mouseEvent);
        }

        public void OnUIMouseUp(mUIMouseEvent mouseEvent)
        {
            OnUIMouseUpEvent?.Invoke(this, mouseEvent);
        }

        public void OnUIMouseDrag(mUIMouseEvent mouseEvent)
        {
            OnUIMouseDragEvent?.Invoke(this, mouseEvent);
        }
    }
}
