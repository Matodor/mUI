using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public sealed class UIViewSettings
    {
        public string Name { get; set; } = "UIView";
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public abstract class UIView : ITicking
    {
        public event Action<UIView, bool> OnChangeActive;
        public event Action<UIView> OnTick, OnFixedTick, OnLateTick; 

        private float _height;
        private float _width;
        private readonly GameObject _gameObject;
        private readonly IList<UIView> _childsViews;

        protected UIView()
        {
            _gameObject = new GameObject("UIView");
            _childsViews = new List<UIView>();
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
                view._gameObject.SetParent(UI.BaseView == null ? UI.UICamera.GameObject : UI.BaseView._gameObject);
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

        public void Tick()
        {
            for (int i = 0; i < _childsViews.Count; i++)
                _childsViews[i].Tick();

            OnTick?.Invoke(this);
        }

        public void FixedTick()
        {
            for (int i = 0; i < _childsViews.Count; i++)
                _childsViews[i].FixedTick();

            OnFixedTick?.Invoke(this);
        }

        public void LateTick()
        {
            for (int i = 0; i < _childsViews.Count; i++)
                _childsViews[i].LateTick();

            OnLateTick?.Invoke(this);
        }
    }
}
