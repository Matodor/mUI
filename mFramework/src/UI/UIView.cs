using System;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIViewSettings
    {
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public abstract class UIView : UIObject
    {
        private float _height;
        private float _width;

        private static UIObject _nextParent;
        
        protected UIView() : base(_nextParent)
        {
        }

        public T ChildView<T>(object[] @params = null) where T : UIView
        {
            return ChildView<T>(new UIViewSettings
            {
                Width = GetWidth(),
                Height = GetHeight()
            }, @params);
        }

        public T ChildView<T>(UIViewSettings settings, params object[] @params) where T : UIView
        {
            return Create<T>(settings, this, @params);
        }

        internal static T Create<T>(UIViewSettings settings, UIObject parent, params object[] @params) where T : UIView
        {
            _nextParent = parent;
            var view = Activator.CreateInstance<T>();
            _nextParent = null;

            view.SetName(typeof(T).Name);
            view.SetupSettings(settings);

            parent?.AddChildObject(view);

            view.Init(@params);
            return view;
        }

        protected abstract void CreateInterface(params object[] @params);

        protected virtual void SetupSettings(UIViewSettings settings)
        {
            _height = settings.Height;
            _width = settings.Width;
        }

        private void Init(params object[] @params)
        {
            CreateInterface(@params);
        }

        public float RelativeX(float x)
        {
            return Position().x - GetWidth() / 2 + GetWidth() * mMath.Clamp(x, 0, 1);
        }

        public float RelativeY(float y)
        {
            return Position().y - GetHeight() / 2 + GetHeight() * mMath.Clamp(y, 0, 1);
        }

        public Vector2 RelativePos(float x, float y)
        {
            return new Vector2
            {
                x = RelativeX(x),
                y = RelativeY(y)
            };
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }
    }
}
