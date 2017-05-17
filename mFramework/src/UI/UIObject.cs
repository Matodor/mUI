using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : ITicking
    {
        public event Action<UIObject> OnActiveChanged, OnVisibleChanged;
        public event Action<UIObject> OnSortingOrderChanged;

        public bool IsActive { get { return _isActive; } }
        public bool IsVisible { get { return _isVisible; } }

        protected readonly GameObject _gameObject;
        protected readonly Transform _transform;

        private readonly List<UIObject> _childObjects;
        private readonly UIObject _parentObject;
        private int _sortingOrder;
        private bool _isActive;
        private bool _isVisible;

        protected UIObject(UIObject parentObject)
        {
            _isActive = true;
            _sortingOrder = 0;
            _parentObject = parentObject;
            _gameObject = new GameObject("UIView");
            _transform = _gameObject.transform;
            _childObjects = new List<UIObject>();

            if (parentObject == null)
                _gameObject.SetParent(mUI.BaseView == null ? mUI.UICamera.GameObject : mUI.BaseView._gameObject);
            else
            {
                _gameObject.SetParent(parentObject._gameObject);
                _parentObject.AddChildObject(this);
            }
        }

        public virtual float GetWidth()
        {
            return 0;
        }

        public virtual float GetHeight()
        {
            return 0;
        }

        public UIObject Rotate(float x, float y, float z)
        {
            _transform.Rotate(x, y, z);
            return this;
        }

        public UIObject Rotate(float angle)
        {
            _transform.Rotate(0, 0, angle);
            return this;
        }

        public UIObject Translate(Vector2 translatePos)
        {
            _transform.Translate(translatePos, Space.World);
            return this;
        }

        public UIObject SortingOrder(int sortingOrder)
        {
            _sortingOrder = sortingOrder;
            SortingOrderChanged();

            return this;
        }
        
        public int SortingOrder()
        {
            return _parentObject?.SortingOrder() ?? 0 + _sortingOrder;
        }

        private void SortingOrderChanged()
        {
            OnSortingOrderChanged?.Invoke(this);
            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].SortingOrderChanged();
        }

        public Vector2 GlobalScale()
        {
            return _transform.lossyScale;
        }

        public Vector2 Position()
        {
            return _transform.position;
        }

        public float Rotation()
        {
            return _transform.eulerAngles.z;
        }

        public UIObject Active()
        {
            if (!_isVisible) return this;

            ActiveChanged(true);
            return this;
        }

        public UIObject Inactive()
        {
            if (!_isVisible) return this;

            ActiveChanged(false);
            return this;
        }

        public UIObject Show()
        {
            VisibleChanged(true);
            return this;
        }

        public UIObject Hide()
        {
            VisibleChanged(false);
            return this;
        }

        private void VisibleChanged(bool visible)
        {
            if (_isVisible == visible)
                return;

            _isVisible = visible;
            _isActive = visible;

            OnVisibleChanged?.Invoke(this);
            OnActiveChanged?.Invoke(this);

            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].VisibleChanged(visible);
        }

        private void ActiveChanged(bool active)
        {
            if (_isActive == active)
                return;
            
            _isActive = active;

            OnActiveChanged?.Invoke(this);
            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].ActiveChanged(active);
        }

        public virtual void Tick()
        {
            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].Tick();
        }

        public virtual void FixedTick()
        {
            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].FixedTick();
        }

        public virtual void LateTick()
        {
            for (int i = 0; i < _childObjects.Count; i++)
                _childObjects[i].LateTick();
        }

        public T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            return UIComponent.Create<T>(this, settings);
        }

        private T AddChildObject<T>(T @object) where T : UIObject
        {
            _childObjects.Add(@object);
            return @object;
        }
    }
}
