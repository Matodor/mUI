﻿using System;
using SimpleJSON;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : MonoBehaviour, IGlobalUniqueIdentifier
    {
        internal IView InternalParentView { get; private set; }
        public UnidirectionalList<UIAnimation> Animations { get; private set; }
        public UnidirectionalList<UIObject> Childs { get; private set; }

        public new GameObject gameObject { get; private set; }
        public new Transform transform { get; private set; }
        public new virtual string tag { get; set; }

        public ulong GUID { get; private set; }
        public UIObject Parent { get; private set; }

        public bool IsActive => enabled && gameObject.activeInHierarchy;
        public bool IsShowing => gameObject.activeInHierarchy;
        
        #region Events
        public event UIEventHandler<UIObject> ActiveChanged = delegate { };
        public event UIEventHandler<UIObject> VisibleChanged = delegate { };
        public event UIEventHandler<UIObject> SortingOrderChanged = delegate { };
        public event UIEventHandler<UIObject> BeforeDestroy = delegate { };

        public event UIEventHandler<UIObject, RemovedСhildObjectEventArgs> ChildObjectRemoved = delegate { };
        public event UIEventHandler<UIObject, AddedСhildObjectEventArgs> ChildObjectAdded = delegate { };
        public event UIEventHandler<UIObject, AddedAnimationEventArgs> AnimationAdded = delegate { };
        #endregion

        private int _localSortingOrder;
        private static ulong _guid;
        private bool _destroyed;

        protected UIObject()
        {
            
        }

        protected virtual void Init()
        {
        }

        internal void InitCompleted()
        {
            Parent?.AddChild(this);
            SortingOrderChanged.Invoke(this);
        }

        private void Awake()
        {
            gameObject = base.gameObject;
            transform = base.transform;

            GUID = ++_guid;
            Parent = null;
            Animations = UnidirectionalList<UIAnimation>.Create();
            Childs = UnidirectionalList<UIObject>.Create();

            _destroyed = false;
            _localSortingOrder = 0;

            mUI.AddUIObject(this);
            Init();
        }

        private void ChangeParent(UIObject parent)
        {
            SetupParent(parent);
            Parent?.AddChild(this);
        }

        protected void SetupParent(UIObject parent)
        {
            Parent?.RemoveChild(this);
            Parent = parent;

            if (parent is IView parentView)
                InternalParentView = parentView;
            else
                InternalParentView = parent?.InternalParentView;

            if (Parent == null)
            {
                gameObject.SetParentTransform(mUI.BaseView == null ? mUI.UICamera.GameObject : mUI.BaseView.gameObject);
            }
            else
            {
                gameObject.SetParentTransform(Parent);
            }
        }

        private void DestroyImpl()
        {
            if (_destroyed)
                return;
         
            _destroyed = true;
            BeforeDestroy.Invoke(this);

            Childs.ForEach(c => c.DestroyImpl());
            Childs.Clear();
            Animations.Clear();

            Parent?.RemoveChild(this);
            mUI.RemoveUIObject(this);

            UnityEngine.Object.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            DestroyImpl();
        }

        private void OnEnable()
        {
            VisibleChanged.Invoke(this);
            ActiveChanged.Invoke(this);
        }

        private void OnDisable()
        {
            VisibleChanged.Invoke(this);
            ActiveChanged.Invoke(this);
        }

        internal void DestroyWithoutChecks()
        {
            DestroyImpl();
        }

        public void Destroy()
        {
            if (this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");

            DestroyWithoutChecks();
        }

        public UIObject SetName(string newName)
        {
            gameObject.name = newName;
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

        public UIObject LocalRotate(float x, float y, float z)
        {
            transform.localEulerAngles = new Vector3(x, y, z);
            return this;
        }

        public UIObject Rotate(float x, float y, float z)
        {
            transform.eulerAngles = new Vector3(x, y, z);
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
            SortingOrderChanged.Invoke(this);
            Childs.ForEach(c => c.OnSortingOrderChanged());
        }

        public UIObject Scale(float v)
        {
            Scale(v, v);
            return this;
        }

        public UIObject Scale(float x, float y)
        {
            transform.localScale = new Vector3(x, y, transform.localScale.z);
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
            return this;
        }

        public UIObject LocalPosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, transform.localPosition.z);
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

        public UIObject Enable()
        {
            enabled = true;
            return this;
        }

        public UIObject Disable()
        {
            enabled = false;
            return this;
        }

        public UIObject Show()
        {
            if (_destroyed)
                return this;
            gameObject.SetActive(true);
            return this;
        }

        public UIObject Hide()
        {
            if (_destroyed)
                return this;
            gameObject.SetActive(false);
            return this;
        }

        protected virtual void OnTick()
        {
        }

        internal void Tick()
        {
            Animations.ForEach(a =>
            {
                if (a.MarkedForDestroy)
                    Animations.Remove(a.GUID);
                else
                    a.Tick();
            });

            if (!IsActive)
                return;

            OnTick();
            Childs.ForEach(c => c.Tick());
        }

        protected virtual void OnFixedTick()
        {
        }

        internal void FixedTick()
        {
            if (!IsActive)
                return;

            OnFixedTick();
            Childs.ForEach(c => c.FixedTick());
        }

        protected virtual void OnLateTick()
        {
        }

        internal void LateTick()
        {
            if (!IsActive)
                return;

            OnLateTick();
            Childs.ForEach(c => c.LateTick());
        }

        public T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            return UIComponent.Create<T>(this, settings);
        }

        public T Animation<T>(UIAnimationSettings settings) where T : UIAnimation
        {
            var uiAnimation = UIAnimation.Create<T>(this, settings);

            Animations.Add(uiAnimation);
            AnimationAdded.Invoke(this, new AddedAnimationEventArgs(uiAnimation));

            return uiAnimation;
        }

        public void RemoveAnimations()
        {
            Animations.Clear();
        }

        internal void RemoveChild(UIObject obj)
        {
            if (Childs.Remove(obj))
            {
                ChildObjectRemoved.Invoke(this, new RemovedСhildObjectEventArgs(obj));       
            }
        }

        private void AddChild(UIObject obj)
        {
            Childs.Add(obj);
            ChildObjectAdded.Invoke(this, new AddedСhildObjectEventArgs(obj));
        }
    }
}
