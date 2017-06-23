using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : IGlobalUniqueIdentifier
    {
        public ulong GUID { get; }

        public event Action<UIObject> OnActiveChanged, OnVisibleChanged;
        public event Action<UIObject> OnSortingOrderChanged;
        public event Action<UIObject> OnAddedChildren;
        public event Action<UIObject> OnRemovedChildren;
        public event Action<UIObject> OnAddedAnimation;
        public event Action<UIObject> OnBeforeDestroy;
        public event Action<UIObject> OnTranslate;
        public event Action<UIObject> OnScale;
        public event Action<UIObject> OnRotate;

        public UIObject Parent { get { return _parentObject; } }
        public bool IsActive { get { return _isActive; } }
        public bool IsVisible { get { return _isVisible; } }

        protected readonly GameObject _gameObject;
        protected readonly Transform _transform;

        private readonly UnidirectionalList<UIAnimation> _animations;
        private readonly UnidirectionalList<UIObject> _childsObjects;
        private readonly UIObject _parentObject;
        private int _sortingOrder;
        private bool _isActive;
        private bool _isVisible;

        private static ulong _guid;

        protected UIObject(UIObject parentObject)
        {
            _isActive = true;
            _isVisible = true;
            _sortingOrder = 0;
            _parentObject = parentObject;
            _gameObject = new GameObject("UIObject");
            _transform = _gameObject.transform;
            _childsObjects = UnidirectionalList<UIObject>.Create();
            _animations = UnidirectionalList<UIAnimation>.Create();

            GUID = ++_guid;

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

        public void ForEachChildren(Action<UIObject> action)
        {
            _childsObjects.ForEach(action);
        }

        public void Destroy()
        {
            if (this is UIView && this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");

            OnBeforeDestroy?.Invoke(this);
            _parentObject.RemoveChildObject(this);

            _animations.ForEach(a => a.Remove());
            _childsObjects.ForEach(o => o.Destroy());

            mUI.Instance.RemoveUIObject(this);
            UnityEngine.Object.Destroy(_gameObject);
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
            _transform.eulerAngles = new Vector3(x, y, z);
            OnRotate?.Invoke(this);
            return this;
        }

        public UIObject Rotate(float angle)
        {
            Rotate(_transform.eulerAngles.x, _transform.eulerAngles.y, angle);
            return this;
        }

        public float Rotation()
        {
            return _transform.eulerAngles.z;
        }

        public UIObject Translate(float x, float y)
        {
            TranslateImpl(x, y);
            return this;
        }

        public UIObject Translate(Vector2 translatePos)
        {
            TranslateImpl(translatePos.x, translatePos.y);
            return this;
        }

        private void TranslateImpl(float x, float y)
        {
            _transform.Translate(x, y, 0, Space.World);
            OnTranslate?.Invoke(this);
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
            _childsObjects.ForEach(o => o.SortingOrderChanged());
        }

        public UIObject Scale(float x, float y)
        {
            _transform.localScale = new Vector3(x, y, _transform.localScale.z);
            OnScale?.Invoke(this);
            return this;
        }

        public UIObject Scale(Vector2 scale)
        {
            return Scale(scale.x, scale.y);
        }

        public Vector2 LocalScale()
        {
            return _transform.localScale;
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

            _childsObjects.ForEach(o => o.VisibleChanged(visible));
        }

        private void ActiveChanged(bool active)
        {
            if (_isActive == active)
                return;
            
            _isActive = active;

            OnActiveChanged?.Invoke(this);
            _childsObjects.ForEach(o => o.ActiveChanged(active));
        }

        internal virtual void Tick()
        {
            _animations.ForEach(a => a.Tick());
            _childsObjects.ForEach(o => o.Tick());
        }

        internal virtual void FixedTick()
        {
            _childsObjects.ForEach(o => o.FixedTick());
        }

        internal virtual void LateTick()
        {
            _childsObjects.ForEach(o => o.LateTick());
        }

        public T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            var child = UIComponent.Create<T>(this, settings);
            AddChildObject(child);
            return child;
        }

        public T Animation<T>() where T : UIAnimation
        {
            return Animation<T>(new UIAnimationSettings());
        }

        public T Animation<T>(UIAnimationSettings settings) where T : UIAnimation
        {
            var animation = UIAnimation.Create<T>(this, settings);
            AddAnimation(animation);
            return animation;
        }

        internal bool RemoveAnimation(UIAnimation animation)
        {
            return _animations.Remove(animation);
        }

        private void AddAnimation(UIAnimation animation)
        {
            _animations.Add(animation);
            OnAddedAnimation?.Invoke(this);
        }

        private bool RemoveChildObject(UIObject @object)
        {
            OnRemovedChildren?.Invoke(this);
            return _childsObjects.Remove(@object);
        }

        private void AddChildObject(UIObject @object)
        {
            _childsObjects.Add(@object);
            OnAddedChildren?.Invoke(@object);
        }
    }
}
