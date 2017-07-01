using System;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIViewSettings
    {
        public string Name { get; set; } = "UIView";
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public abstract class UIView : UIObject
    {
        private float _height;
        private float _width;

        private static UIView _nextParentView;
        
        protected UIView() : base(_nextParentView)
        {
        }

        public T ChildView<T>(params object[] @params) where T : UIView
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

        internal static T Create<T>(UIViewSettings settings, UIView parent, params object[] @params) where T : UIView
        {
            _nextParentView = parent;
            var view = Activator.CreateInstance<T>();
            _nextParentView = null;

            view.SetName(typeof(T).Name);
            view.SetupSettings(settings);
            view.Init(@params);
            return view;
        }

        protected abstract void CreateInterface(params object[] @params);

        protected virtual void SetupSettings(UIViewSettings settings)
        {
            _height = settings.Height;
            _width = settings.Width;
            _gameObject.name = settings.Name;
        }

        private void Init(params object[] @params)
        {
            CreateInterface(@params);
        }

        public float RelativeX(float x)
        {
            return -GetWidth() / 2 + GetWidth() * mMath.Clamp(x, 0, 1);
        }

        public float RelativeY(float y)
        {
            return -GetHeight() / 2 + GetHeight() * mMath.Clamp(y, 0, 1);
        }

        public Vector2 RelativePosition(float x, float y)
        {
            return Position() + new Vector2
            {
                x = RelativeX(x),
                y = RelativeY(y)
            };
        }

        public override float GetHeight()
        {
            return _height;
        }

        public override float GetWidth()
        {
            return _width;
        }
    }
}
