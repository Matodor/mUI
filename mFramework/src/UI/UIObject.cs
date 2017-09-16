using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : IGlobalUniqueIdentifier
    {
        internal GameObject GameObject => _gameObject;
        internal bool MarkedForDestroy;

        public int ChildsCount => _childsObjects.Count;
        public UIObject this[int index] => _childsObjects[index];

        public ulong GUID { get; }
        public UIObject Parent { get; }

        public bool IsActive { get; private set; }
        public bool IsVisible { get; private set; }

        #region Events
        public event UIEventHandler<UIObject> ActiveChanged; 
        public event UIEventHandler<UIObject> VisibleChanged;
        public event UIEventHandler<UIObject> SortingOrderChanged;
        public event UIEventHandler<UIObject> BeforeDestroy;

        public event UIEventHandler<UIObject, AddedСhildObjectEventArgs> AddedСhildObject;
        public event UIEventHandler<UIObject, AddedAnimationEventArgs> AnimationAdded;
        public event UIEventHandler<UIObject, TranslateEventArgs> Translated;
        public event UIEventHandler<UIObject, ScaledEventArgs> Scaled;
        public event UIEventHandler<UIObject, RotatedEventArgs> Rotated;
        #endregion

        protected readonly GameObject _gameObject;
        protected readonly Transform _transform;

        private readonly List<UIAnimation> _animations;
        private readonly List<UIObject> _childsObjects;
        private int _sortingOrder;

        private bool _tmpActive;
        private bool _tmpVisible;

        private static ulong _guid;

        protected UIObject(UIObject parentObject)
        {
            GUID = ++_guid;
            IsActive = true;
            IsVisible = true;
            Parent = parentObject;

            _tmpVisible = true;
            _tmpActive = true;

            _sortingOrder = 0;
            _gameObject = new GameObject("UIObject");
            _transform = _gameObject.transform;
            _childsObjects = new List<UIObject>();
            _animations = new List<UIAnimation>();
            
            if (parentObject == null)
                _gameObject.SetParent(mUI.BaseView == null ? mUI.UICamera.GameObject : mUI.BaseView._gameObject);
            else
                _gameObject.SetParent(parentObject._gameObject);
            
            mUI.Instance.AddUIObject(this);
        }

        ~UIObject()
        {
            mUI.Instance.RemoveUIObject(this);
        }

        public void Destroy()
        {
            if (this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");

            MarkedForDestroy = true;
        }

        internal void DestroyImpl()
        {
            BeforeDestroy?.Invoke(this);

            _animations.Clear();
            for (var i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].DestroyImpl();
            _childsObjects.Clear();

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
            var scaledHeightDiv2 = GetHeight() / 2f;
            var scaledWidthDiv2 = GetWidth() / 2f;

            return new UIRect
            {
                Position = pos,
                Bottom = pos.y - scaledHeightDiv2,
                Top = pos.y + scaledHeightDiv2,
                Left = pos.x - scaledWidthDiv2,
                Right = pos.x + scaledWidthDiv2,
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
            var oldAngle = _transform.eulerAngles;
            _transform.eulerAngles = new Vector3(x, y, z);
            Rotated?.Invoke(this, new RotatedEventArgs(oldAngle, _transform.eulerAngles));
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
            Translate(x, y, 0);
            return this;
        }

        public UIObject Translate(float x, float y, float z)
        {
            _transform.Translate(x, y, z, Space.World);
            Translated?.Invoke(this, new TranslateEventArgs(new Vector3(x, y, z), _transform.position));
            return this;
        }

        public UIObject Translate(Vector2 translatePos)
        {
            Translate(translatePos.x, translatePos.y, 0);
            return this;
        }

        public UIObject SortingOrder(int sortingOrder)
        {
            _sortingOrder = sortingOrder;
            OnSortingOrderChanged();
            return this;
        }

        public int LocalSortingOrder()
        {
            return _sortingOrder;
        }

        public int SortingOrder()
        {
            return (Parent?.SortingOrder() ?? 0) + _sortingOrder;
        }

        internal void OnSortingOrderChanged()
        {
            SortingOrderChanged?.Invoke(this);
            for (var i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].OnSortingOrderChanged();
        }

        public UIObject Scale(float v)
        {
            Scale(v, v);
            return this;
        }

        public UIObject Scale(float x, float y)
        {
            var oldScale = _transform.localScale;
            _transform.localScale = new Vector3(x, y, _transform.localScale.z);
            Scaled?.Invoke(this, new ScaledEventArgs(oldScale, _transform.localScale));
            return this;
        }

        public UIObject Scale(Vector2 scale)
        {
            Scale(scale.x, scale.y);
            return this;
        }

        public Vector2 LocalScale()
        {
            return _transform.localScale;
        }

        public Vector2 GlobalScale()
        {
            return _transform.lossyScale;
        }

        public UIObject PositionX(float x)
        {
            Position(x, Position().y);
            return this;
        }

        public UIObject PositionY(float y)
        {
            Position(Position().x, y);
            return this;
        }

        public UIObject Position(float x, float y)
        {
            _transform.position = new Vector3(x, y, _transform.position.z);
            Translated?.Invoke(this, new TranslateEventArgs(Vector3.zero, _transform.position));
            return this;
        }

        public UIObject LocalPosition(float x, float y)
        {
            _transform.localPosition = new Vector3(x, y, _transform.localPosition.z);
            Translated?.Invoke(this, new TranslateEventArgs(Vector3.zero, _transform.position));
            return this;
        }

        public UIObject LocalPosition(Vector2 position)
        {
            return LocalPosition(position.x, position.y);
        }

        public Vector2 LocalPosition()
        {
            return _transform.localPosition;
        }

        public UIObject Position(Vector2 position)
        {
            return Position(position.x, position.y);
        }

        public Vector2 Position()
        {
            return _transform.position;
        }

        public UIObject Active()
        {
            if (!IsVisible) return this;

            _tmpActive = true;
            OnActiveChanged(true);
            return this;
        }

        public UIObject Inactive()
        {
            if (!IsVisible) return this;

            _tmpActive = false;
            OnActiveChanged(false);
            return this;
        }

        public UIObject Show()
        {
            _tmpVisible = true;
            OnVisibleChanged(true);
            return this;
        }

        public UIObject Hide()
        {
            _tmpVisible = false;
            OnVisibleChanged(false);
            return this;
        }

        private void OnVisibleChanged(bool visible)
        {
            if (IsVisible == visible)
                return;

            if (visible && !_tmpVisible)
                visible = false;

            IsVisible = visible;
            IsActive = visible;

            VisibleChanged?.Invoke(this);
            ActiveChanged?.Invoke(this);

            for (var i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].OnVisibleChanged(visible);
            _gameObject.SetActive(visible);
        }

        private void OnActiveChanged(bool active)
        {
            if (IsActive == active)
                return;

            if (active && !_tmpActive)
                active = false;

            IsActive = active;

            ActiveChanged?.Invoke(this);
            for (var i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].OnActiveChanged(active);
        }

        public virtual void OnTick()
        {
        }

        internal virtual void Tick()
        {
            if (!IsActive)
                return;

            OnTick();

            for (var i = _animations.Count - 1; i >= 0; i--)
            {
                if (_animations[i].MarkedForDestroy)
                    _animations.RemoveAt(i);
                else 
                    _animations[i].Tick();
            }

            for (var i = _childsObjects.Count - 1; i >= 0; i--)
            {
                if (_childsObjects[i].MarkedForDestroy)
                {
                    _childsObjects[i].DestroyImpl();
                    _childsObjects.RemoveAt(i);
                }
                else
                    _childsObjects[i].Tick();
            }
        }

        public virtual void OnFixedTick()
        {
        }

        internal virtual void FixedTick()
        {
            if (!IsActive)
                return;

            OnFixedTick();

            for (var i = _childsObjects.Count - 1; i >= 0; i--)
            {
                if (_childsObjects[i].MarkedForDestroy)
                {
                    _childsObjects[i].DestroyImpl();
                    _childsObjects.RemoveAt(i);
                }
                else
                    _childsObjects[i].FixedTick();
            }
        }

        public virtual void OnLateTick()
        {
        }

        internal virtual void LateTick()
        {
            if (!IsActive)
                return;

            OnLateTick();

            for (var i = _childsObjects.Count - 1; i >= 0; i--)
            {
                if (_childsObjects[i].MarkedForDestroy)
                {
                    _childsObjects[i].DestroyImpl();
                    _childsObjects.RemoveAt(i);
                }
                else
                    _childsObjects[i].LateTick();
            }
        }

        public T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            var child = UIComponent.Create<T>(this, settings);
            return child;
        }

        public T Animation<T>(UIAnimationSettings settings) where T : UIAnimation
        {
            var animation = UIAnimation.Create<T>(this, settings);

            _animations.Add(animation);
            AnimationAdded?.Invoke(this, new AddedAnimationEventArgs(animation));

            return animation;
        }

        public void RemoveAnimations<T>() where T : UIAnimation
        {
            for (var i = _animations.Count - 1; i >= 0; i--)
            {
                if (_animations[i].GetType() == typeof(T))
                    _animations[i].Remove();
            }
        }

        public void RemoveAnimations()
        {
            _animations.Clear();
        }

        internal void AddChildObject(UIObject obj)
        {
            _childsObjects.Add(obj);
            AddedСhildObject?.Invoke(this, new AddedСhildObjectEventArgs(obj));
        }
    }
}
