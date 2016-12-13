﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Animations;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Views
{
    public abstract class BaseView : UIRect, UIGameObject
    {
        public virtual void Create(object data) { }
        public const int NextViewSortingOrder = 10;
        public int SortingOrder
        {
            get
            {
                return _sortingOrder;
            }
            set
            {
                UpdateSortingOrder(_sortingOrder, value);
                _sortingOrder = value;
            }
        }

        public BaseView ParentView
        {
            get
            {
                return _baseView;
            }
            set
            {
                _baseView = value;
                if (value != null)
                {
                    SortingOrder = _baseView.SortingOrder + NextViewSortingOrder;
                }
            }
        }

        public void Destroy()
        {
            ParentView.RemoveChildView(this);
            UnityEngine.Object.Destroy(GameObject);
        }

        public void OnRotate()
        {
            OnRotateEvent?.Invoke(this);
        }

        public void OnTranslate()
        {
            OnTranslateEvent?.Invoke(this);
        }

        public void OnScale()
        {
            OnScaleEvent?.Invoke(this);
        }

        public event Action<UIGameObject> OnRotateEvent;
        public event Action<UIGameObject> OnTranslateEvent;
        public event Action<UIGameObject> OnScaleEvent;

        public List<mUIAnimation> Animations { get; } = new List<mUIAnimation>();
        public GameObject GameObject { get { return _viewObject; } }
        public Transform Transform { get { return _viewTransform; } }

        public float PureWidth { get { return _viewWidth; } }
        public float PureHeight { get { return _viewHeight; } }

        public float Height { get { return PureHeight*Transform.lossyScale.y; } }
        public float Width { get { return PureWidth*Transform.lossyScale.x; } }

        public float LeftAnchor { get { return Transform.position.x - Width/2; } }
        public float RightAnchor { get { return Transform.position.x + Width/2; } }
        public float TopAnchor { get { return Transform.position.y + Height/2; } }
        public float BottomAnchor { get { return Transform.position.y - Height/2; } }

        public event Action<BaseView> OnChangedHeight;
        public event Action<BaseView> OnChangedWidth;
        public event Action<BaseView> OnTick;
        public event Action<BaseView, PartialView> OnAddChildView;
        public event Action<BaseView, UIObject> OnAddChildObject;

        protected List<BaseView> _childViews;
        protected List<UIObject> _childObjects;
        private GameObject _viewObject;
        private Transform _viewTransform;
        private BaseView _baseView;
        private int _sortingOrder;

        private float _viewWidth;
        private float _viewHeight;

        private void UpdateSortingOrder(int oldvalue, int newValue)
        {
            for (int i = 0; i < _childObjects.Count; i++)
            {
                var old = _childObjects[i].Renderer.sortingOrder - oldvalue;
                _childObjects[i].SortingOrder(newValue + old);
            }

            for (int i = 0; i < _childViews.Count; i++)
            {
                var old = _childViews[i].SortingOrder - oldvalue;
                _childViews[i].SortingOrder = newValue + old;
            }
        }

        public void RemoveChildObject(UIObject obj)
        {
            _childObjects.Remove(obj);
        }

        public void AddChildObject(UIObject obj)
        {
            if (obj.Renderer != null)
                obj.Renderer.sortingOrder = SortingOrder;
            _childObjects.Add(obj);
            OnAddChildObject?.Invoke(this, obj);
        }

        public void RemoveChildView(BaseView view)
        {
            _childViews.Remove(view);
        }

        public void AddChildView(PartialView view)
        {
            view.SortingOrder = SortingOrder + NextViewSortingOrder;
            _childViews.Add(view);
            OnAddChildView?.Invoke(this, view);
        }

        private void InitBase(Transform parent)
        {
            _viewObject = new GameObject("view");
            _viewObject.transform.parent = parent;
            _viewObject.transform.localPosition = Vector3.zero;
            _viewTransform = _viewObject.transform;
            _childViews = new List<BaseView>();
            _childObjects = new List<UIObject>();
            _baseView = null;
            _sortingOrder = 0;
        }
        
        public void SetWidth(float width)
        {
            if (ParentView != null)
            {
                if (width <= ParentView._viewWidth)
                    _viewWidth = width;
            }
            else 
                _viewWidth = width;
            OnChangedWidth?.Invoke(this);
        }

        public void SetHeight(float height)
        {
            if (ParentView != null)
            {
                if (height <= ParentView._viewHeight)
                    _viewHeight = height;
            }
            else
                _viewHeight = height;
            OnChangedHeight?.Invoke(this);
        }

        public void Tick()
        {
            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].Tick();
            for (int i = 0; i < _childViews.Count; i++)
                _childViews[i].Tick();
            OnTick?.Invoke(this);
        }
    }
}
