using System;
using JetBrains.Annotations;
using mFramework.UI;
using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : MonoBehaviour, IUIObject
    {
        internal Transform Transform => base.transform;
        internal GameObject GameObject => base.gameObject;

        [Obsolete("mUI disallow use gameObject property")]
        public new GameObject gameObject => throw new Exception("Not supported");

        [Obsolete("mUI disallow use transform property")]
        public new Transform transform => throw new Exception("Not supported");

        [Obsolete("mUI disallow use enabled property, use Enable()/Disable()")]
        public new bool enabled => throw new Exception("Not supported");

        public IView ParentView { get; private set; }
        public UnidirectionalList<UIAnimation> Animations { get; private set; }
        public UnidirectionalList<IUIObject> Childs { get; private set; }

        public virtual UIPadding Padding { get; set; }

        public virtual Vector2 CenterOffset => Vector2.zero;
        public virtual Vector3 GlobalScale => Transform.lossyScale;

        public virtual float Height => UnscaledHeight * GlobalScale.y +
                               (Padding.Top + Padding.Bottom) * GlobalScale.y;

        public virtual float Width => UnscaledWidth * GlobalScale.x +
                              (Padding.Left + Padding.Right) * GlobalScale.x;

        public virtual float UnscaledHeight => 0f;
        public virtual float UnscaledWidth => 0f;

        public virtual UIAnchor Anchor
        {
            get => _anchor;
            set
            {
                if (value == _anchor)
                    return;

                Transform.position -= AnchorTranslate();
                _anchor = value;
                Transform.position += AnchorTranslate();
            }
        }

        public Vector3 LocalPosition
        {
            get => Transform.localPosition - AnchorTranslate();
            set => Transform.localPosition = value + AnchorTranslate();
        }

        public virtual Vector3 Position
        {
            get => Transform.position - AnchorTranslate();
            set => Transform.position = value + AnchorTranslate();
        }
        
        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set
            {
                var pos = Position;
                Transform.localScale = value;
                Position = pos;
            }
        }

        public virtual UIRect Rect => GetRect();

        public float Rotation
        {
            get => Transform.eulerAngles.z;
            set
            {
                Transform.RotateAround(Position, Vector3.forward, value - Transform.eulerAngles.z);
            }
        }

        public int LocalSortingOrder
        {
            get => _localSortingOrder;
            set { _localSortingOrder = value; OnSortingOrderChanged(); }
        }

        public int SortingOrder
        {
            get => (Parent?.SortingOrder ?? 0) + _localSortingOrder;
            set { _localSortingOrder = value; OnSortingOrderChanged(); }
        }
        
        public ulong GUID { get; private set; }
        public UIObject Parent { get; private set; }

        public virtual bool IsActive => base.enabled && base.gameObject.activeInHierarchy;
        public virtual bool IsShowing => base.gameObject.activeInHierarchy;
        
        #region Events
        public event UIEventHandler<IUIObject> ActiveChanged = delegate { };
        public event UIEventHandler<IUIObject> VisibleChanged = delegate { };
        public event UIEventHandler<IUIObject> SortingOrderChanged = delegate { };
        public event UIEventHandler<IUIObject> BeforeDestroy = delegate { };

        public event UIEventHandler<IUIObject, IUIObject> ChildObjectRemoved = delegate { };
        public event UIEventHandler<IUIObject, IUIObject> ChildObjectAdded = delegate { };
        public event UIEventHandler<IUIObject, UIAnimation> AnimationAdded = delegate { };
        #endregion

        private UIAnchor _anchor;
        private int _localSortingOrder;
        private bool _destroyed;
        private static ulong _guid;

        protected virtual void AfterAwake()
        {
            
        }

        internal void InitCompleted()
        {
            Parent?.AddChild(this);
            SortingOrderChanged(this);
        }

        private void Awake()
        {
            Transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            GUID = ++_guid;
            Parent = null;
            Animations = UnidirectionalList<UIAnimation>.Create();
            Childs = UnidirectionalList<IUIObject>.Create();

            _anchor = UIAnchor.MiddleCenter;
            _destroyed = false;
            _localSortingOrder = 0;

            SetName(base.gameObject.name);
            mUI.AddUIObject(this);
            AfterAwake();
        }

        private Vector3 AnchorTranslate()
        {
            var rect = Rect;
            Vector3 translate;

            switch (_anchor)
            {
                case UIAnchor.UpperLeft:
                    translate = new Vector3(
                        Transform.position.x - rect.TopLeft.x,
                        Transform.position.y - rect.TopLeft.y
                    );
                    break;
                case UIAnchor.UpperCenter:
                    translate = new Vector3(
                        Transform.position.x - rect.Top.x,
                        Transform.position.y - rect.Top.y
                    );
                    break;
                case UIAnchor.UpperRight:
                    translate = new Vector3(
                        Transform.position.x - rect.TopRight.x,
                        Transform.position.y - rect.TopRight.y
                    );
                    break;
                case UIAnchor.MiddleLeft:
                    translate = new Vector3(
                        Transform.position.x - rect.Left.x,
                        Transform.position.y - rect.Left.y
                    );
                    break;
                case UIAnchor.MiddleCenter:
                    translate = new Vector3(
                        Transform.position.x - rect.Center.x,
                        Transform.position.y - rect.Center.y
                    );
                    break;
                case UIAnchor.MiddleRight:
                    translate = new Vector3(
                        Transform.position.x - rect.Right.x,
                        Transform.position.y - rect.Right.y
                    );
                    break;
                case UIAnchor.LowerLeft:
                    translate = new Vector3(
                        Transform.position.x - rect.BottomLeft.x,
                        Transform.position.y - rect.BottomLeft.y
                    );
                    break;
                case UIAnchor.LowerCenter:
                    translate = new Vector3(
                        Transform.position.x - rect.Bottom.x,
                        Transform.position.y - rect.Bottom.y
                    );
                    break;
                case UIAnchor.LowerRight:
                    translate = new Vector3(
                        Transform.position.x - rect.BottomRight.x,
                        Transform.position.y - rect.BottomRight.y
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return translate;
        }

        private UIRect GetRect()
        {
            var centerPos = (Vector2) Transform.position + CenterOffset;

            var left = -UnscaledWidth / 2 * GlobalScale.x - Padding.Left * GlobalScale.x;
            var right = UnscaledWidth / 2 * GlobalScale.x + Padding.Right * GlobalScale.x;
            var top = UnscaledHeight / 2 * GlobalScale.y + Padding.Top * GlobalScale.y;
            var bottom = -UnscaledHeight / 2 * GlobalScale.y - Padding.Bottom * GlobalScale.y;

            centerPos = new Vector2(
                centerPos.x + (left + right) / 2f,
                centerPos.y + (top + bottom) / 2f
            );

            var points = new[]
            {
                new Vector2(left, top),     // top left
                new Vector2(0, top),        // top
                new Vector2(right, top),    // top right

                new Vector2(left, 0),       // left
                new Vector2(right, 0),      // right

                new Vector2(left, bottom),  // bottom left
                new Vector2(0, bottom),     // bottom
                new Vector2(right, bottom)  // bottom right
            };

            mMath.GetRotatedPoints(Rotation, centerPos, points);

            return new UIRect(
                points[0],
                points[1],
                points[2],

                points[3],
                centerPos,
                points[4],

                points[5],
                points[6],
                points[7]
            );
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
                Transform.ParentTransform(mUI.BaseView == null
                    ? mUI.UICamera.Transform
                    : mUI.BaseView.Transform);
            }
            else
            {
                Transform.ParentTransform(Parent.Transform);
            }
        }

        /*internal void UpdatePositionWithAnchor()
        {
            var middleCenter = Transform.position - _pivot;

            switch (_anchor)
            {
                case UIAnchor.UpperLeft:
                    _pivot = new Vector3(+Width / 2, -Height / 2);
                    break;
                case UIAnchor.UpperCenter:
                    _pivot = new Vector3(0f, -Height / 2);
                    break;
                case UIAnchor.UpperRight:
                    _pivot = new Vector3(-Width / 2, -Height / 2);
                    break;
                case UIAnchor.MiddleLeft:
                    _pivot = new Vector3(+Width / 2, 0f);
                    break;
                case UIAnchor.MiddleCenter:
                    _pivot = new Vector3(0f, 0f);
                    break;
                case UIAnchor.MiddleRight:
                    _pivot = new Vector3(-Width / 2, 0f);
                    break;
                case UIAnchor.LowerLeft:
                    _pivot = new Vector3(+Width / 2, +Height / 2);
                    break;
                case UIAnchor.LowerCenter:
                    _pivot = new Vector3(0f, +Height / 2);
                    break;
                case UIAnchor.LowerRight:
                    _pivot = new Vector3(-Width / 2, +Height / 2);
                    break;
            }

            Transform.position = middleCenter + _pivot;
        }*/

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

            UnityEngine.Object.Destroy(base.gameObject);
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
            Childs.Clear();
        }

        public void Destroy()
        {
            if (this == mUI.BaseView)
                throw new Exception("Can't destroy BaseView");

            DestroyWithoutChecks();
        }

        public IUIObject SetName(string newName)
        {
            base.gameObject.name = $"{newName} ({GUID})";
            return this;
        }
        
        private void OnSortingOrderChanged()
        {
            SortingOrderChanged(this);
            
            // ReSharper disable once PossibleNullReferenceException
            Childs.ForEach(c => ((UIObject) c).OnSortingOrderChanged());
        }

        public IUIObject Enable()
        {
            base.enabled = true;
            return this;
        }

        public IUIObject Disable()
        {
            base.enabled = false;
            return this;
        }

        public IUIObject Show()
        {
            if (_destroyed)
                return this;
            
            base.gameObject.SetActive(true);
            return this;
        }

        public IUIObject Hide()
        {
            if (_destroyed)
                return this;

            base.gameObject.SetActive(false);
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
