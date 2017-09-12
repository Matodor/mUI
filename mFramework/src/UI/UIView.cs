using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIViewSettings
    {
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public static class NewView<T> where T : UIView
    {
        public static readonly Func<T> Instance;

        static NewView()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[0], null);
            var e = Expression.New(constructor);
            Instance = Expression.Lambda<Func<T>>(e).Compile();
        }
    }

    public abstract class UIView : UIObject, IView
    {
        public UIView ParentView => (UIView) Parent;

        private float _height;
        private float _width;

        private static UIObject _nextParent;
        
        protected UIView() : base(_nextParent)
        {
        }
        
        internal static T Create<T>(UIViewSettings settings, UIObject parent, params object[] @params) where T : UIView, new()
        {
            _nextParent = parent;
            var view = new T();
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

        protected virtual void BeforeCreate()
        {
            
        }

        private void Init(params object[] @params)
        {
            BeforeCreate();
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
