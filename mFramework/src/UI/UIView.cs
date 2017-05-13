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

    public abstract partial class UIView : ITicking
    {
        public event Action<UIView, bool> OnChangeActive;

        private float _height;
        private float _width;
        private readonly GameObject _gameObject;
        private readonly IList<UIView> _childsViews;
        private readonly List<UIComponent> _childsComponents;

        protected UIView()
        {
            _gameObject = new GameObject("UIView");
            _childsViews = new List<UIView>();
            _childsComponents = new List<UIComponent>();
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
            var view = Create<T>(settings, this, @params);
            _childsViews.Add(view);
            return view;
        }

        public static T Create<T>(UIViewSettings settings, UIView parent, params object[] @params) where T : UIView
        {
            var view = Activator.CreateInstance<T>();
            view.SetupSettings(settings);

            if (parent == null)
                view._gameObject.SetParent(mUI.BaseView == null ? mUI.UICamera.GameObject : mUI.BaseView._gameObject);
            else
                view._gameObject.SetParent(parent._gameObject);
            
            view.Init(@params);
            return view;
        }

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

        protected abstract void CreateInterface(params object[] @params);

        private T AddComponent<T>(T component) where T : UIComponent
        {
            return component;
        }

        public UIView Show()
        {
            _gameObject.SetActive(true);
            OnChangeActive?.Invoke(this, true);
            return this;
        }

        public UIView Hide()
        {
            _gameObject.SetActive(false);
            OnChangeActive?.Invoke(this, true);
            return this;
        }

        public virtual void Tick()
        {
            for (int i = 0; i < _childsViews.Count; i++)
                _childsViews[i].Tick();
        }

        public virtual void FixedTick()
        {
            for (int i = 0; i < _childsViews.Count; i++)
                _childsViews[i].FixedTick();
        }

        public virtual void LateTick()
        {
            for (int i = 0; i < _childsViews.Count; i++)
                _childsViews[i].LateTick();
        }
    }
}
