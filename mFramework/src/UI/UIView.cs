using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mFramework.UI;
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
        private bool _created;

        private static UIView _nextParentView;
        
        protected UIView() : base(_nextParentView)
        {
            _created = false;
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

        public static T Create<T>(UIViewSettings settings, UIView parent, params object[] @params) where T : UIView
        {
            _nextParentView = parent;
            var view = Activator.CreateInstance<T>();
            _nextParentView = null;

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

        public void Init(params object[] @params)
        {
            if (_created)
                throw new Exception("This UIView already created");

            CreateInterface(@params);
            _created = true;
        }

        public Vector2 RelativePosition(float x, float y)
        {
            return Position() + new Vector2
            {
                x = -GetWidth() / 2 + GetWidth() * mMath.Сlamp(x, 0, 1),
                y = -GetHeight() / 2 + GetHeight() * mMath.Сlamp(y, 0, 1)
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

        public override void Tick()
        {
            base.Tick();
        }

        public override void FixedTick()
        {
            base.FixedTick();
        }

        public override void LateTick()
        {
            base.LateTick();
        }
    }
}
