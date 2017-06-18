using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject
    {
        public ulong GUID { get; }

        public event Action<UIObject> OnActiveChanged, OnVisibleChanged;
        public event Action<UIObject> OnSortingOrderChanged;
        public event Action<UIObject> OnAddedChildren;
        public event Action<UIObject> OnTranslate;

        public UIObject Parent { get { return _parentObject; } }
        public bool IsActive { get { return _isActive; } }
        public bool IsVisible { get { return _isVisible; } }

        public ReadOnlyCollection<UIObject> ChildsObjects { get { return _childsObjects.AsReadOnly(); } }

        protected readonly GameObject _gameObject;
        protected readonly Transform _transform;

        private readonly List<UIAnimation> _animations;
        private readonly List<UIObject> _childsObjects;
        private readonly UIObject _parentObject;
        private int _sortingOrder;
        private bool _isActive;
        private bool _isVisible;

        private static ulong _guid;

        protected UIObject(UIObject parentObject)
        {
            GUID = ++_guid;
            _isActive = true;
            _isVisible = true;
            _sortingOrder = 0;
            _parentObject = parentObject;
            _gameObject = new GameObject("UIObject");
            _transform = _gameObject.transform;
            _childsObjects = new List<UIObject>();
            _animations = new List<UIAnimation>();

            if (parentObject == null)
                _gameObject.SetParent(mUI.BaseView == null ? mUI.UICamera.GameObject : mUI.BaseView._gameObject);
            else
            {
                _gameObject.SetParent(parentObject._gameObject);
                if (this is UIView)
                    parentObject.AddChildObject(this);
            }
            
            mUI.Instance.AddUIObject(this);
        }

        ~UIObject()
        {
            mUI.Instance.RemoveUIObject(this);
        }

        public UIObject SetName(string name)
        {
            _gameObject.name = name;
            return this;
        }

        public virtual UIRect GetRect()
        {
            var pos = Position();
            var heightDiv2 = GetHeight() / 2;
            var widthDiv2 = GetWidth() / 2;

            return new UIRect()
            {
                Position = pos,
                Bottom = pos.y - heightDiv2,
                Top = pos.y + heightDiv2,
                Left = pos.x - widthDiv2,
                Right = pos.x + widthDiv2,
            };
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

        public UIObject Translate(float x, float y)
        {
            TranslateImpl(x, y);
            return this;
        }

        public UIObject Translate(Vector2 translatePos)
        {
            TranslateImpl(translatePos.x, translatePos.y);
            OnTranslate?.Invoke(this);
            return this;
        }

        private void TranslateImpl(float x, float y)
        {
            _transform.Translate(x, y, 0, Space.World);
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
            for (int i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].SortingOrderChanged();
        }

        public Vector2 GlobalScale()
        {
            return _transform.lossyScale;
        }

        public void Position(float x, float y)
        {
            _transform.position = new Vector3
            {
                x = x,
                y = y,
                z = _transform.position.z
            };
        }

        public void Position(Vector2 position)
        {
            Position(position.x, position.y);
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

            for (int i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].VisibleChanged(visible);
        }

        private void ActiveChanged(bool active)
        {
            if (_isActive == active)
                return;
            
            _isActive = active;

            OnActiveChanged?.Invoke(this);
            for (int i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].ActiveChanged(active);
        }

        internal virtual void Tick()
        {
            for (int i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].Tick();
        }

        internal virtual void FixedTick()
        {
            for (int i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].FixedTick();
        }

        internal virtual void LateTick()
        {
            for (int i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].LateTick();
        }

        public T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            var child = UIComponent.Create<T>(this, settings);
            AddChildObject(child);
            return child;
        }

        public T Animation<T>() where T : UIAnimation
        {
            var animation = UIAnimation<T>.Create();

            return animation;
        }

        internal void AddAnimation(UIAnimation animation)
        {
            
        }

        private T AddChildObject<T>(T @object) where T : UIObject
        {
            _childsObjects.Add(@object);
            OnAddedChildren?.Invoke(@object);
            return @object;
        }
    }
}
