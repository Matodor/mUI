﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIViewSettings
    {
        public float? Height = null;
        public float? Width = null;
        public int? SortingOrder = null;
        public Vector2? DefaultPos = null;
    }

    /*public static class NewView<T> where T : UIView
    {
        public static readonly Func<T> Instance;

        static NewView()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[0], null);
            var e = Expression.New(constructor);
            Instance = Expression.Lambda<Func<T>>(e).Compile();
        }
    }*/

    public abstract class UIView : UIObject, IView
    {
        public UIView ParentView => (UIView) Parent;

        private float _height;
        private float _width;

        public static UIView Create(Type viewType, UIViewSettings settings, UIObject parent, params object[] @params)
        {
            if (!typeof(UIView).IsAssignableFrom(viewType))
                throw new Exception($"The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            SetupView(view, settings, parent, @params);
            return view;
        }

        public static T Create<T>(UIViewSettings settings, UIObject parent, params object[] @params) where T : UIView
        {
            var view = new GameObject(typeof(T).Name).AddComponent<T>();
            SetupView(view, settings, parent, @params);
            return view;
        }

        private static void SetupView(UIView view, UIViewSettings settings, UIObject parent, object[] @params)
        {
            view.SetupParent(parent);
            view.SetupSettings(settings);
            view.InitCompleted();
            view.Create(@params);
        }

        protected abstract void CreateInterface(params object[] @params);

        protected virtual void SetupSettings(UIViewSettings settings)
        {
            if (settings.Height.HasValue)
                _height = settings.Height.Value;

            if (settings.Width.HasValue)
                _width = settings.Width.Value;

            if (settings.DefaultPos.HasValue)
                Position(settings.DefaultPos.Value);

            if (settings.SortingOrder.HasValue)
                SortingOrder(settings.SortingOrder.Value);
        }

        protected virtual void BeforeCreate()
        {
            
        }

        private void Create(params object[] @params)
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
