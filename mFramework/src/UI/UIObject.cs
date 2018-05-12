using System;
using UnityEngine;

namespace mFramework.UI
{
    internal struct NotRotatedRect
    {
        public readonly Vector2 Center;
        public readonly float LocalLeft;
        public readonly float LocalRight;
        public readonly float LocalTop;
        public readonly float LocalBottom;

        public NotRotatedRect(Vector2 center, float localLeft, float localRight, float localTop, float localBottom)
        {
            Center = center;
            LocalLeft = localLeft;
            LocalRight = localRight;
            LocalTop = localTop;
            LocalBottom = localBottom;
        }
    }

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

        public virtual float Height => UnscaledHeight * GlobalScale.y 
            + (Padding.Top + Padding.Bottom) * GlobalScale.y;

        public virtual float Width => UnscaledWidth * GlobalScale.x 
            + (Padding.Left + Padding.Right) * GlobalScale.x;

        public virtual float UnscaledHeight { get; protected set; }
        public virtual float UnscaledWidth { get; protected set; }

        public virtual UIAnchor Anchor
        {
            get => _anchor;
            set
            {
                if (value == _anchor)
                    return;

                Transform.position = Transform.position + AnchorLocalPos(_anchorPivot);
                _anchor = value;
                _anchorPivot = PivotByAnchor(value);
                Transform.position = Transform.position - AnchorLocalPos(_anchorPivot);
            }
        }

        public Vector3 LocalPosition
        {
            get => Transform.localPosition + AnchorLocalPos(_anchorPivot);
            set => Transform.localPosition = value - AnchorLocalPos(_anchorPivot);
        }

        public virtual Vector3 Position
        {
            get => Transform.position + AnchorLocalPos(_anchorPivot);
            set => Transform.position = value - AnchorLocalPos(_anchorPivot);
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

        private Vector2 _anchorPivot;
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
            _anchorPivot = PivotByAnchor(_anchor);
            _destroyed = false;
            _localSortingOrder = 0;

            SetName(base.gameObject.name);
            mUI.AddUIObject(this);
            AfterAwake();
        }

        internal Vector3 GetAnchorPos(UIAnchor anchor)
        {
            return Transform.position + AnchorLocalPos(PivotByAnchor(anchor));
        }

        internal void PositionByAnchor(Vector3 pos, UIAnchor anchor)
        {
            Transform.position = pos - AnchorLocalPos(PivotByAnchor(anchor));
        } 

        // x - from left to right, y from bottom to top
        private static Vector2 PivotByAnchor(UIAnchor anchor)
        {
            switch (anchor)
            {
                case UIAnchor.UpperLeft:
                    return new Vector2(0f, 1f);
                case UIAnchor.UpperCenter:
                    return new Vector2(0.5f, 1f);
                case UIAnchor.UpperRight:
                    return new Vector2(1f, 1f);

                case UIAnchor.MiddleLeft:
                    return new Vector2(0f, 0.5f);
                case UIAnchor.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);
                case UIAnchor.MiddleRight:
                    return new Vector2(1f, 0.5f);

                case UIAnchor.LowerLeft:
                    return new Vector2(0f, 0f);
                case UIAnchor.LowerCenter:
                    return new Vector2(0.5f, 0f);
                case UIAnchor.LowerRight:
                    return new Vector2(1f, 0f);

                default:
                    return new Vector2(0.5f, 0.5f);
            }
        }

        private Vector3 AnchorLocalPos(Vector2 pivot)
        {
            var rect = NotRotatedLocalRect();

            var anchorLocalPos = new Vector2(
                BezierHelper.Linear(pivot.x, rect.LocalLeft, rect.LocalRight),
                BezierHelper.Linear(pivot.y, rect.LocalBottom, rect.LocalTop)
            );

            return mMath.GetRotatedPoint(Vector2.zero, anchorLocalPos, Rotation);
        }

        private NotRotatedRect NotRotatedLocalRect()
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

            return new NotRotatedRect(centerPos, left, right, top, bottom);
        }

        private UIRect GetRect()
        {
            var rect = NotRotatedLocalRect();
            var anchorLocalPos = new Vector2(
                BezierHelper.Linear(_anchorPivot.x, rect.LocalLeft, rect.LocalRight),
                BezierHelper.Linear(_anchorPivot.y, rect.LocalBottom, rect.LocalTop)
            );

            var points = new[]
            {
                new Vector2(rect.LocalLeft, rect.LocalTop),     // top left
                new Vector2(0, rect.LocalTop),                  // top
                new Vector2(rect.LocalRight, rect.LocalTop),    // top right

                new Vector2(rect.LocalLeft, 0),                 // left
                new Vector2(rect.LocalRight, 0),                // right

                new Vector2(rect.LocalLeft, rect.LocalBottom),  // bottom left
                new Vector2(0, rect.LocalBottom),               // bottom
                new Vector2(rect.LocalRight, rect.LocalBottom), // bottom right
                anchorLocalPos                                  // anchor pos
            };

            mMath.GetRotatedPoints(Rotation, rect.Center, points);

            return new UIRect(
                points[0],
                points[1],
                points[2],

                points[3],
                rect.Center,
                points[4],

                points[5],
                points[6],
                points[7],
                points[8]
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

        public virtual T Component<T>(UIComponentSettings settings) where T : UIComponent
        {
            return UIComponent.Create<T>(this, settings);
        }

        public virtual T Animation<T>(UIAnimationSettings settings) where T : UIAnimation, new()
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
