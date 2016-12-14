using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace mUIApp.Views
{
    public static class UIViewHelper
    {
        public static T CreateView<T>(this UIObject view, params object[] param) where T : UIView
        {
            var newView = (T) Activator.CreateInstance(typeof (T), view, param);
            
            newView.SetHeight(view.PureHeight);
            newView.SetWidth(view.PureWidth);
            newView.Create();

            return newView;
        }
    }

    public abstract class UIView : UIObject
    {
        public override float PureWidth { get { return _viewWidth;} }
        public override float PureHeight { get { return _viewHeight; } }

        public virtual void Create() { }

        public event Action<UIView, float> OnChangeHeight;
        public event Action<UIView, float> OnChangeWidth;

        private float _viewHeight;
        private float _viewWidth;

        public static T Create<T>(params object[] param) where T : UIView, new()
        {
            var newView = (T)Activator.CreateInstance(typeof(T), null, param);
            newView.Create();
            return newView;
        }

        protected UIView(UIObject parent) : base(parent)
        {
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
