using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : MonoBehaviour, IGlobalUniqueIdentifier
    {
        public int ChildsCount => _childsObjects.Count;
        public UIObject this[int index] => _childsObjects[index];

        public ulong GUID { get; private set; }
        public UIObject Parent { get; private set; }

        public bool IsActive => enabled;
        public bool IsShowing => gameObject.activeInHierarchy;

        #region Events
        public event UIEventHandler<UIObject> ActiveChanged; 
        public event UIEventHandler<UIObject> VisibleChanged;
        public event UIEventHandler<UIObject> SortingOrderChanged;
        public event UIEventHandler<UIObject> BeforeDestroy;

        public event UIEventHandler<UIObject, RemovedСhildObjectEventArgs> СhildObjectRemoved;
        public event UIEventHandler<UIObject, AddedСhildObjectEventArgs> СhildObjectAdded;
        public event UIEventHandler<UIObject, AddedAnimationEventArgs> AnimationAdded;
        public event UIEventHandler<UIObject, TranslateEventArgs> Translated;
        public event UIEventHandler<UIObject, ScaledEventArgs> Scaled;
        public event UIEventHandler<UIObject, RotatedEventArgs> Rotated;
        #endregion

        private List<UIAnimation> _animations;
        private List<UIObject> _childsObjects;
        private int _localSortingOrder;
        private static ulong _guid;

        protected UIObject()
        {
            
        }

        protected virtual void Init()
        {
        }

        protected void InitCompleted()
        {
            Parent?.AddChild(this);
        }

        private void Awake()
        {
            GUID = ++_guid;
            Parent = null;

            _localSortingOrder = 0;
            _childsObjects = new List<UIObject>();
            _animations = new List<UIAnimation>();

            mUI.Instance.AddUIObject(this);
            Init();
        }

        public void ChangeParent(UIObject parent)
        {
            SetupParent(parent);
            Parent?.AddChild(this);
        }

        protected void SetupParent(UIObject parent)
        {
            Parent?.RemoveChild(this);
            Parent = parent;

            if (Parent == null)
            {
                gameObject.SetParentTransform(mUI.BaseView == null ? mUI.UICamera.GameObject : mUI.BaseView.gameObject);
            }
            else
            {
                gameObject.SetParentTransform(Parent);
            }
        }

        private void OnDestroy()
        {
            BeforeDestroy?.Invoke(this);

            Parent?.RemoveChild(this);
            mUI.Instance.RemoveUIObject(this);

            _animations.Clear();
            for (var i = 0; i < _childsObjects.Count; i++)
                _childsObjects[i].Destroy();
            _childsObjects.Clear();
        }

        private void OnEnable()
        {
            VisibleChanged?.Invoke(this);
            ActiveChanged?.Invoke(this);
        }

        private void OnDisable()
        {
            VisibleChanged?.Invoke(this);
            ActiveChanged?.Invoke(this);
        }

        public new void Destroy()
        {
            if (this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");
            UnityEngine.Object.Destroy(this);
        }

        public UIObject SetName(string name)
        {
            gameObject.name = name;
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
            var oldAngle = transform.eulerAngles;
            transform.eulerAngles = new Vector3(x, y, z);
            Rotated?.Invoke(this, new RotatedEventArgs(oldAngle, transform.eulerAngles));
            return this;
        }

        public UIObject Rotate(float angle)
        {
            Rotate(transform.eulerAngles.x, transform.eulerAngles.y, angle);
            return this;
        }

        public float Rotation()
        {
            return transform.eulerAngles.z;
        }

        public UIObject Translate(float x, float y)
        {
            Translate(x, y, 0);
            return this;
        }

        public UIObject Translate(float x, float y, float z)
        {
            transform.Translate(x, y, z, Space.World);
            Translated?.Invoke(this, new TranslateEventArgs(new Vector3(x, y, z), transform.position));
            return this;
        }

        public UIObject Translate(Vector2 translatePos)
        {
            Translate(translatePos.x, translatePos.y, 0);
            return this;
        }

        public UIObject SortingOrder(int sortingOrder)
        {
            _localSortingOrder = sortingOrder;
            OnSortingOrderChanged();
            return this;
        }

        public int LocalSortingOrder()
        {
            return _localSortingOrder;
        }

        public int SortingOrder()
        {
            return (Parent?.SortingOrder() ?? 0) + _localSortingOrder;
        }

        private void OnSortingOrderChanged()
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
            var oldScale = transform.localScale;
            transform.localScale = new Vector3(x, y, transform.localScale.z);
            Scaled?.Invoke(this, new ScaledEventArgs(oldScale, transform.localScale));
            return this;
        }

        public UIObject Scale(Vector2 scale)
        {
            Scale(scale.x, scale.y);
            return this;
        }

        public Vector2 LocalScale()
        {
            return transform.localScale;
        }

        public Vector2 GlobalScale()
        {
            return transform.lossyScale;
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
            transform.position = new Vector3(x, y, transform.position.z);
            Translated?.Invoke(this, new TranslateEventArgs(Vector3.zero, transform.position));
            return this;
        }

        public UIObject LocalPosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, transform.localPosition.z);
            Translated?.Invoke(this, new TranslateEventArgs(Vector3.zero, transform.position));
            return this;
        }

        public UIObject LocalPosition(Vector2 position)
        {
            return LocalPosition(position.x, position.y);
        }

        public Vector2 LocalPosition()
        {
            return transform.localPosition;
        }

        public UIObject Position(Vector2 position)
        {
            return Position(position.x, position.y);
        }

        public Vector2 Position()
        {
            return transform.position;
        }

        public UIObject Enabled()
        {
            enabled = true;
            return this;
        }

        public UIObject Disabled()
        {
            enabled = false;
            return this;
        }

        public UIObject Show()
        {
            gameObject.SetActive(true);
            return this;
        }

        public UIObject Hide()
        {
            gameObject.SetActive(false);
            return this;
        }

        public virtual void OnTick()
        {
        }

        internal virtual void Tick()
        {
            if (isActiveAndEnabled)
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
                _childsObjects[i].Tick();
            }
        }

        public virtual void OnFixedTick()
        {
        }

        internal virtual void FixedTick()
        {
            if (isActiveAndEnabled)
                return;

            OnFixedTick();

            for (var i = _childsObjects.Count - 1; i >= 0; i--)
            {
                 _childsObjects[i].FixedTick();
            }
        }

        public virtual void OnLateTick()
        {
        }

        internal virtual void LateTick()
        {
            if (isActiveAndEnabled)
                return;

            OnLateTick();

            for (var i = _childsObjects.Count - 1; i >= 0; i--)
            {
                _childsObjects[i].LateTick();
            }
        }

        public T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            return UIComponent.Create<T>(this, settings);
        }

        public T Animation<T>(UIAnimationSettings settings) where T : UIAnimation
        {
            var uiAnimation = UIAnimation.Create<T>(this, settings);

            _animations.Add(uiAnimation);
            AnimationAdded?.Invoke(this, new AddedAnimationEventArgs(uiAnimation));

            return uiAnimation;
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

        internal void RemoveChild(UIObject obj)
        {
            if (_childsObjects.Remove(obj))
            {
                СhildObjectRemoved?.Invoke(this, new RemovedСhildObjectEventArgs(obj));       
            }
        }

        private void AddChild(UIObject obj)
        {
            if (_childsObjects.Contains(obj))
                return;

            _childsObjects.Add(obj);
            СhildObjectAdded?.Invoke(this, new AddedСhildObjectEventArgs(obj));
        }
    }
}
