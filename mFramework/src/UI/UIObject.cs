using System;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : IGlobalUniqueIdentifier
    {
        public ulong GUID { get; }

        public event UIEventHandler<UIObject> ActiveChanged; 
        public event UIEventHandler<UIObject> VisibleChanged;
        public event UIEventHandler<UIObject> SortingOrderChanged;
        public event UIEventHandler<UIObject, AddedСhildObjectEventArgs> AddedСhildObject;
        public event UIEventHandler<UIObject, RemovedСhildObjectEventArgs> RemovedСhildObject;
        public event UIEventHandler<UIObject, RemovedAnimationEventArgs> AnimationRemoved;
        public event UIEventHandler<UIObject, AddedAnimationEventArgs> AnimationAdded;
        public event UIEventHandler<UIObject> BeforeDestroy;

        public event UIEventHandler<UIObject, TranslateEventArgs> Translated;
        public event UIEventHandler<UIObject, ScaledEventArgs> Scaled;
        public event UIEventHandler<UIObject, RotatedEventArgs> Rotated;

        public UIObject Parent { get { return _parentObject; } }
        public bool IsActive { get { return _isActive; } }
        public bool IsVisible { get { return _isVisible; } }

        protected readonly GameObject _gameObject;
        protected readonly Transform _transform;

        private readonly UnidirectionalList<UIAnimation> _animations;
        private readonly UnidirectionalList<UIObject> _childsObjects;
        private readonly UIObject _parentObject;
        protected int _sortingOrder;
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
            _childsObjects = UnidirectionalList<UIObject>.Create();
            _animations = UnidirectionalList<UIAnimation>.Create();
            
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

        public void ForEachChildren(Action<UIObject> action)
        {
            _childsObjects.ForEach(action);
        }

        public void Destroy()
        {
            if (this is UIView && this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");

            BeforeDestroy?.Invoke(this);
            Hide();

            _parentObject.RemoveChildObject(this);
            _animations.ForEach(a => a.Remove());
            DestroyChilds();

            mUI.Instance.RemoveUIObject(this);
            UnityEngine.Object.Destroy(_gameObject);
        }

        public void DestroyChilds()
        {
            _childsObjects.ForEach(o => o.Destroy());
        }

        public UIObject SetName(string name)
        {
            _gameObject.name = name;
            return this;
        }

        public virtual UIRect GetRect()
        {
            var pos = Position();
            var scale = GlobalScale();
            var scaledHeightDiv2 = (GetHeight() / 2) * scale.y;
            var scaledWidthDiv2 = (GetWidth() / 2) * scale.x;

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
            return (_parentObject?.SortingOrder() ?? 0) + _sortingOrder;
        }

        internal void OnSortingOrderChanged()
        {
            SortingOrderChanged?.Invoke(this);
            _childsObjects.ForEach(o => o.OnSortingOrderChanged());
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

        public void PositionX(float x)
        {
            Position(x, Position().y);
        }

        public void PositionY(float y)
        {
            Position(Position().x, y);
        }

        public void Position(float x, float y)
        {
            _transform.position = new Vector3
            {
                x = x,
                y = y,
                z = _transform.position.z
            };
            Translated?.Invoke(this, new TranslateEventArgs(Vector3.zero, _transform.position));
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

            OnActiveChanged(true);
            return this;
        }

        public UIObject Inactive()
        {
            if (!_isVisible) return this;

            OnActiveChanged(false);
            return this;
        }

        public UIObject Show()
        {
            OnVisibleChanged(true);
            return this;
        }

        public UIObject Hide()
        {
            OnVisibleChanged(false);
            return this;
        }

        private void OnVisibleChanged(bool visible)
        {
            if (_isVisible == visible)
                return;

            _isVisible = visible;
            _isActive = visible;

            VisibleChanged?.Invoke(this);
            ActiveChanged?.Invoke(this);

            _childsObjects.ForEach(o => o.OnVisibleChanged(visible));

            var renderer = this as IUIRenderer;
            if (renderer != null)
                renderer.UIRenderer.enabled = visible;
        }

        private void OnActiveChanged(bool active)
        {
            if (_isActive == active)
                return;
            
            _isActive = active;

            ActiveChanged?.Invoke(this);
            _childsObjects.ForEach(o => o.OnActiveChanged(active));
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

        public void RemoveAnimations<T>() where T : UIAnimation
        {
            _animations.ForEach(a =>
            {
                if (a.GetType() == typeof(T))
                    a.Remove();
            });
        }

        public void RemoveAnimations()
        {
            _animations.ForEach(a => a.Remove());
        }

        internal bool RemoveAnimation(UIAnimation animation)
        {
            if (_animations.Remove(animation))
            {
                AnimationRemoved?.Invoke(this, new RemovedAnimationEventArgs(animation));
                return true;
            }
            return false;
        }

        private void AddAnimation(UIAnimation animation)
        {
            _animations.Add(animation);
            AnimationAdded?.Invoke(this, new AddedAnimationEventArgs(animation));
        }

        private bool RemoveChildObject(UIObject @object)
        {
            if (_childsObjects.Remove(@object))
            {
                RemovedСhildObject?.Invoke(this, new RemovedСhildObjectEventArgs(@object));
                return true;
            }
            return false;
        }

        internal void AddChildObject(UIObject @object)
        {
            _childsObjects.Add(@object);
            AddedСhildObject?.Invoke(this, new AddedСhildObjectEventArgs(@object));
        }
    }
}
