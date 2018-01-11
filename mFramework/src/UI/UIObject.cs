using System;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : MonoBehaviour, IUIObject
    {
        public IView ParentView { get; private set; }
        public UnidirectionalList<UIAnimation> Animations { get; private set; }
        public UnidirectionalList<IUIObject> Childs { get; private set; }
        public object Data { get; set; }

        public new GameObject gameObject { get; private set; }
        public new Transform transform { get; private set; }
        public new string tag { get; set; }

        public ulong GUID { get; private set; }
        public UIObject Parent { get; private set; }

        public bool IsActive => enabled && gameObject.activeInHierarchy;
        public bool IsShowing => gameObject.activeInHierarchy;
        
        #region Events
        public event UIEventHandler<IUIObject> ActiveChanged = delegate { };
        public event UIEventHandler<IUIObject> VisibleChanged = delegate { };
        public event UIEventHandler<IUIObject> SortingOrderChanged = delegate { };
        public event UIEventHandler<IUIObject> BeforeDestroy = delegate { };

        public event UIEventHandler<IUIObject, IUIObject> ChildObjectRemoved = delegate { };
        public event UIEventHandler<IUIObject, IUIObject> ChildObjectAdded = delegate { };
        public event UIEventHandler<IUIObject, UIAnimation> AnimationAdded = delegate { };
        #endregion

        private int _localSortingOrder;
        private bool _destroyed;
        private static ulong _guid;

        protected virtual void Init()
        {
            
        }

        internal void InitCompleted()
        {
            Parent?.AddChild(this);
            SortingOrderChanged(this);
        }

        internal void Awake()
        {
            gameObject = base.gameObject;
            transform = base.transform;

            GUID = ++_guid;
            Parent = null;
            Animations = UnidirectionalList<UIAnimation>.Create();
            Childs = UnidirectionalList<IUIObject>.Create();

            _destroyed = false;
            _localSortingOrder = 0;

            SetName(gameObject.name);
            mUI.AddUIObject(this);
            Init();
        }

        internal void SetupParent(UIObject parent)
        {
            Parent?.RemoveChild(this);
            Parent = parent;

            if (parent is IView parentView)
                ParentView = parentView;
            else
                ParentView = parent?.ParentView;

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
            BeforeDestroy(this);

            Childs.ForEach(c => c.Destroy());
            Childs.Clear();
            Animations.Clear();

            Parent?.RemoveChild(this);
            mUI.RemoveUIObject(this);

            ActiveChanged = null;
            VisibleChanged = null;
            SortingOrderChanged = null;
            BeforeDestroy = null;

            ChildObjectRemoved = null;
            ChildObjectAdded = null;
            AnimationAdded = null;

            UnityEngine.Object.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            DestroyImpl();
        }

        private void OnEnable()
        {
            if (_destroyed)
                return;
            VisibleChanged(this);
            ActiveChanged(this);
        }

        private void OnDisable()
        {
            if (_destroyed)
                return;
            VisibleChanged(this);
            ActiveChanged(this);
        }

        internal void DestroyWithoutChecks()
        {
            DestroyImpl();
        }

        public void DestroyChilds()
        {
            Childs.ForEach(c => c.Destroy());
        }

        public void Destroy()
        {
            if (this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");

            DestroyWithoutChecks();
        }

        public IUIObject SetName(string newName)
        {
            gameObject.name = $"{newName} ({GUID})";
            return this;
        }

        public virtual UIRect GetRect()
        {
            var pos = Pos();
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

        public abstract float GetWidth();
        public abstract float GetHeight();
        public abstract float UnscaledHeight();
        public abstract float UnscaledWidth();

        public IUIObject LocalRotate(float angle)
        {
            LocalRotate(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
            return this;
        }

        public IUIObject LocalRotate(float x, float y, float z)
        {
            transform.localEulerAngles = new Vector3(x, y, z);
            return this;
        }

        public float LocalRotation()
        {
            return transform.localEulerAngles.z;
        }
        
        public IUIObject Rotate(float x, float y, float z)
        {
            transform.eulerAngles = new Vector3(x, y, z);
            return this;
        }

        public IUIObject Rotate(float angle)
        {
            Rotate(transform.eulerAngles.x, transform.eulerAngles.y, angle);
            return this;
        }

        public float Rotation()
        {
            return transform.eulerAngles.z;
        }

        public IUIObject LocalTranslate(float x, float y)
        {
            LocalTranslate(x, y, 0);
            return this;
        }

        public IUIObject LocalTranslate(float x, float y, float z)
        {
            transform.Translate(x, y, z, Space.Self);
            return this;
        }

        public IUIObject LocalTranslate(Vector2 translatePos)
        {
            LocalTranslate(translatePos.x, translatePos.y, 0);
            return this;
        }

        public IUIObject Translate(float x, float y)
        {
            Translate(x, y, 0);
            return this;
        }

        public IUIObject Translate(float x, float y, float z)
        {
            transform.Translate(x, y, z, Space.World);
            return this;
        }

        public IUIObject Translate(Vector2 translatePos)
        {
            Translate(translatePos.x, translatePos.y, 0);
            return this;
        }

        public IUIObject SortingOrder(int sortingOrder)
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

        public void OnSortingOrderChanged()
        {
            SortingOrderChanged(this);
            Childs.ForEach(c => c.OnSortingOrderChanged());
        }

        public IUIObject Scale(float v)
        {
            Scale(v, v);
            return this;
        }

        public IUIObject Scale(float x, float y)
        {
            transform.localScale = new Vector3(x, y, transform.localScale.z);
            return this;
        }

        public IUIObject Scale(Vector2 scale)
        {
            Scale(scale.x, scale.y);
            return this;
        }

        public Vector2 Scale()
        {
            return transform.localScale;
        }

        public Vector2 GlobalScale()
        {
            return transform.lossyScale;
        }

        public Vector2 LocalTranslatedY(float y)
        {
            return LocalTranslated(transform.localPosition.x, y);
        }

        public Vector2 LocalTranslatedX(float x)
        {
            return LocalTranslated(x, transform.localPosition.y);
        }

        public Vector2 LocalTranslated(Vector2 vec)
        {
            return LocalTranslated(vec.x, vec.y);
        }

        public Vector2 LocalTranslated(float x, float y)
        {
            return new Vector2(
                transform.localPosition.x + x,
                transform.localPosition.y + y
            );
        }

        public IUIObject LocalPosX(float x)
        {
            LocalPos(x, transform.localPosition.y);
            return this;
        }

        public IUIObject LocalPosY(float y)
        {
            LocalPos(transform.localPosition.x, y);
            return this;
        }

        public IUIObject LocalPos(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, transform.localPosition.z);
            return this;
        }

        public IUIObject LocalPos(Vector2 position)
        {
            LocalPos(position.x, position.y);
            return this;
        }

        public Vector2 LocalPos()
        {
            return transform.localPosition;
        }

        public Vector2 TranslatedY(float y)
        {
            return new Vector2(
                transform.position.x,
                transform.position.y + y
            );
        }

        public Vector2 TranslatedX(float x)
        {
            return new Vector2(
                transform.position.x + x,
                transform.position.y
            );
        }

        public Vector2 Translated(Vector2 vec)
        {
            return Translated(vec.x, vec.y);
        }

        public Vector2 Translated(float x, float y)
        {
            return new Vector2(
                transform.position.x + x,
                transform.position.y + y
            );
        }

        public IUIObject PosX(float x)
        {
            Pos(x, transform.position.y);
            return this;
        }

        public IUIObject PosY(float y)
        {
            Pos(transform.position.x, y);
            return this;
        }

        public IUIObject Pos(float x, float y)
        {
            transform.position = new Vector3(x, y, transform.position.z);
            return this;
        }

        public IUIObject Pos(Vector2 position)
        {
            Pos(position.x, position.y);
            return this;
        }   

        public Vector2 Pos()
        {
            return transform.position;
        }

        public IUIObject Enable()
        {
            enabled = true;
            return this;
        }

        public IUIObject Disable()
        {
            enabled = false;
            return this;
        }

        public IUIObject Show()
        {
            if (_destroyed)
                return this;
            gameObject.SetActive(true);
            return this;
        }

        public IUIObject Hide()
        {
            if (_destroyed)
                return this;
            gameObject.SetActive(false);
            return this;
        }

        protected virtual void OnTick()
        {
        }

        public void Tick()
        {
            OnTick();
            Animations.ForEach(a => a.Tick());
            Childs.ForEach(c => c.Tick());
        }

        protected virtual void OnFixedTick()
        {
        }

        public void FixedTick()
        {
            OnFixedTick();
            Childs.ForEach(c => c.FixedTick());

            /*if (transform.hasChanged)
            {
                transform.hasChanged = false;
                mCore.Log($"{GetType().Name} transform.hasChanged = {transform.hasChanged}");
            }*/
        }

        protected virtual void OnLateTick()
        {
        }

        public void LateTick()
        {
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
            AnimationAdded(this, uiAnimation);

            return uiAnimation;
        }

        public void RemoveAnimations()
        {
            Animations.Clear();
        }

        public void RemoveAnimations<T>() where T : UIAnimation
        {
            Animations.ForEach(a =>
            {
                if (a is T)
                    a.Remove();
            });
        }

        internal void RemoveAnimation(UIAnimation anim)
        {
            Animations.Remove(anim);
        }

        internal void RemoveChild(IUIObject obj)
        {
            if (Childs.Remove(obj))
            {
                ChildObjectRemoved(this, obj);       
            }
        }

        private void AddChild(IUIObject obj)
        {
            Childs.Add(obj);
            ChildObjectAdded(this, obj);
        }
    }
}
