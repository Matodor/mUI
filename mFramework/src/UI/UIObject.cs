using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : ITicking
    {
        public event Action<UIObject, bool> OnActiveChanged;
        public event Action<UIObject> OnSortingOrderChanged;

        protected readonly GameObject _gameObject;
        protected readonly Transform _transform;

        private readonly List<UIObject> _childObjects;
        private readonly UIObject _parentObject;
        private int _sortingOrder;

        protected UIObject(UIObject parentObject)
        {
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

        public UIObject Show()
        {
            _gameObject.SetActive(true);
            OnActiveChanged?.Invoke(this, true);
            return this;
        }

        public UIObject Hide()
        {
            _gameObject.SetActive(false);
            OnActiveChanged?.Invoke(this, true);
            return this;
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
