using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using mUIApp.Input;
using UnityEngine;

namespace mUIApp.Components
{
    public static partial class UIElementsHelper
    {
        public static T CreateView<T>(this UIObject view, params object[] param) where T : UIView
        {
            var newView = (T) Activator.CreateInstance(typeof (T), view, param);
            newView.Create();
            return newView;
        }
    }

    public abstract class UIView : UIObject
    {
        public override float PureWidth { get { return _viewWidth;} }
        public override float PureHeight { get { return _viewHeight; } }
        
        public event Action<UIView, float> OnChangeHeight;
        public event Action<UIView, float> OnChangeWidth;

        private float _viewHeight;
        private float _viewWidth;  

        public virtual void Create() { }

        public static T Create<T>(params object[] param) where T : UIView
        { 
            var newView = (T)Activator.CreateInstance(typeof(T), new object[] { null, param });
            newView.Create();
            return newView;
        }

        protected UIView(UIObject parent) : base(parent)
        {
            if (Parent != null)
            {
                SetHeight(parent.PureHeight);
                SetWidth(parent.PureWidth);
            }
            else
            {
                SetHeight(mUI.UICamera.PureHeight);
                SetWidth(mUI.UICamera.PureWidth);
            }

            if (mUI.Debug)
            {
                mUI.UIInput.OnMouseUpEvent += DrawDebugView;
                mUI.UIInput.OnMouseDownEvent += DrawDebugView;
                mUI.UIInput.OnMouseDragEvent += DrawDebugView;
            }
        }

        ~UIView()
        {
            if (mUI.Debug)
            {
                mUI.UIInput.OnMouseUpEvent -= DrawDebugView;
                mUI.UIInput.OnMouseDownEvent -= DrawDebugView;
                mUI.UIInput.OnMouseDragEvent -= DrawDebugView;
            }
        }

        private void DrawDebugView(mUIMouseEvent mUiMouseEvent)
        {
            Debug.DrawLine(new Vector3(Left, Top), new Vector3(Right, Top));
            Debug.DrawLine(new Vector3(Right, Top), new Vector3(Right, Bottom));
            Debug.DrawLine(new Vector3(Right, Bottom), new Vector3(Left, Bottom));
            Debug.DrawLine(new Vector3(Left, Bottom), new Vector3(Left, Top));
        }

        public void SetHeight(float height)
        {
            _viewHeight = height;
            OnChangeHeight?.Invoke(this, height);
        }

        public void SetWidth(float width)
        {
            _viewWidth = width;
            OnChangeWidth?.Invoke(this, width);
        }
    }
}
