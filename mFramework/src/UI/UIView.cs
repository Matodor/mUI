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
        private static UIView _nextParentView;
        
        protected UIView() : base(_nextParentView)
        {
        }

        public T ChildView<T>(params object[] @params) where T : UIView
        {
            return ChildView<T>(new UIViewSettings
            {
                Width = _width,
                Height = _height
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
            CreateInterface(@params);
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
