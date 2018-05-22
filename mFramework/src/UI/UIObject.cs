using System;
using UnityEngine;

namespace mFramework.UI
{
    public struct NotRotatedRect
    {
        /// <summary>
        /// Center offset with transform position
        /// </summary>
        public readonly Vector2 CenterOffset;

        public readonly float LocalLeft;
        public readonly float LocalRight;
        public readonly float LocalTop;
        public readonly float LocalBottom;

        public NotRotatedRect(Vector2 centerOffset, float localLeft, 
            float localRight, float localTop, float localBottom)
        {
            CenterOffset = centerOffset;
            LocalLeft = localLeft;
            LocalRight = localRight;
            LocalTop = localTop;
            LocalBottom = localBottom;
        }
    }

    public abstract class UIObject : MonoBehaviour, IUIObject
    {
        public Transform Transform => base.transform;
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

        public virtual Vector2 CenterOffset => new Vector2(
            UnscaledCenterOffset.x * GlobalScale.x,
            UnscaledCenterOffset.y * GlobalScale.y
        );

        public virtual Vector2 UnscaledCenterOffset { get; protected set; } = Vector2.zero;
        public virtual Vector3 GlobalScale => Transform.lossyScale;

        public virtual float Height => UnscaledHeight * GlobalScale.y 
            + (Padding.Top + Padding.Bottom) * GlobalScale.y;

        public virtual float Width => UnscaledWidth * GlobalScale.x 
            + (Padding.Left + Padding.Right) * GlobalScale.x;

        public virtual float UnscaledHeight { get; protected set; }
        public virtual float UnscaledWidth { get; protected set; }

        public Vector2 AnchorPivot
        {
            get => _anchorPivot;
            set => _anchorPivot = value;
        }

        public virtual UIAnchor Anchor
        {
            get => _anchor;
            set
            {
                if (value == _anchor)
                    return;

                var pos = Position;
                _anchor = value;
                _anchorPivot = PivotByAnchor(value);
                Position = pos;
            }
        }

        public Vector3 LocalPosition
        {
            get => Transform.localPosition -
                   (Parent?.AnchorOffsetFromTransformPos(Parent.AnchorPivot, 0) ?? Vector3.zero) +
                   AnchorOffsetFromTransformPos(_anchorPivot, LocalRotation);
            set => Transform.localPosition = value +
                   (Parent?.AnchorOffsetFromTransformPos(Parent.AnchorPivot, 0) ?? Vector3.zero) -
                   AnchorOffsetFromTransformPos(_anchorPivot, LocalRotation);
        }

        public virtual Vector3 Position
        {
            get => Transform.position + AnchorOffsetFromTransformPos(_anchorPivot, Rotation);
            set => Transform.position = value - AnchorOffsetFromTransformPos(_anchorPivot, Rotation);
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

        public virtual UIRect Rect => GetRect(Transform.position);

        public float Rotation
        {
            get => Transform.eulerAngles.z;
            set => Transform.RotateAround(
                Position, Vector3.forward, 
                value - Transform.eulerAngles.z
            );
        }

        public float LocalRotation
        {
            get => Transform.localEulerAngles.z;
            set => Transform.RotateAround(
                Position, Vector3.forward,
                value - Transform.localEulerAngles.z
            );
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
            if (Parent != null)
            {
                Position = Parent.Position;
                Parent.AddChild(this);
            }
            SortingOrderChanged(this);
        }

        private void Awake()
        {
            //Transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
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
        
        // x - from left to right, y from bottom to top
        public static Vector2 PivotByAnchor(UIAnchor anchor)
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

        internal Vector3 GetLocalAnchorPos(UIAnchor anchor)
        {
            return Transform.localPosition + 
                AnchorOffsetFromTransformPos(PivotByAnchor(anchor), LocalRotation);
        }

        internal Vector3 GetAnchorPos(UIAnchor anchor)
        {
            return Transform.position +
                AnchorOffsetFromTransformPos(PivotByAnchor(anchor), Rotation);
        }

        internal void LocalPositionByAnchor(Vector3 pos, UIAnchor anchor)
        {
            Transform.localPosition = pos - 
                AnchorOffsetFromTransformPos(PivotByAnchor(anchor), LocalRotation);
        }

        internal void PositionByAnchor(Vector3 pos, UIAnchor anchor)
        {
            Transform.position = pos - 
                AnchorOffsetFromTransformPos(PivotByAnchor(anchor), Rotation);
        } 

        private static Vector2 NotRotatedAnchorOffset(NotRotatedRect rect, Vector2 anchorPivor)
        {
            return new Vector2(
                BezierHelper.Linear(anchorPivor.x, rect.LocalLeft, rect.LocalRight),
                BezierHelper.Linear(anchorPivor.y, rect.LocalBottom, rect.LocalTop)
            );
        }

        private NotRotatedRect NotRotatedLocalRect()
        {
            var centerOffset = CenterOffset;
            var scale = GlobalScale;

            var left =   -UnscaledWidth  / 2 * scale.x - Padding.Left   * scale.x;
            var right =  +UnscaledWidth  / 2 * scale.x + Padding.Right  * scale.x;
            var top =    +UnscaledHeight / 2 * scale.y + Padding.Top    * scale.y;
            var bottom = -UnscaledHeight / 2 * scale.y - Padding.Bottom * scale.y;

            var paddingOffsetX = (left + right) / 2;
            var paddingOffsetY = (top + bottom) / 2;

            centerOffset = new Vector2(
                centerOffset.x + paddingOffsetX,        
                centerOffset.y + paddingOffsetY
            );

            return new NotRotatedRect(
                centerOffset: centerOffset,
                localLeft: left - paddingOffsetX,
                localRight: right - paddingOffsetX,
                localTop: top - paddingOffsetY,
                localBottom: bottom - paddingOffsetY
            );
        }

        private Vector3 AnchorOffsetFromTransformPos(Vector2 anchorPivot, float rotation)
        {
            var rect = NotRotatedLocalRect();
            var notRotatedAnchorOffset = NotRotatedAnchorOffset(rect, anchorPivot);
            var center = mMath.GetRotatedPoint(Vector2.zero, rect.CenterOffset, rotation);
            return mMath.GetRotatedPoint(center, notRotatedAnchorOffset, rotation);
        }

        /*internal Vector3 AnchorDiffWit234hTransformPos(Vector2 anchorPivor, bool local)
        {
            var rect = NotRotatedLocalRect();
            var center = mMath.GetRotatedPoint(Vector2.zero, rect.CenterOffset, 
                local 
                    ? LocalRotation 
                    : Rotation
            );

            var notRotatedAnchorOffset = NotRotatedAnchorOffset(rect, anchorPivor);
            return mMath.GetRotatedPoint(center, notRotatedAnchorOffset, 
                local
                    ? LocalRotation
                    : Rotation
            );
        }*/

        internal UIRect GetRect(Vector2 transformPosition)
        {
            var rect = NotRotatedLocalRect();
            var center = mMath.GetRotatedPoint(transformPosition, rect.CenterOffset, Rotation);
            var notRotatedAnchorOffset = NotRotatedAnchorOffset(rect, _anchorPivot);

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

                notRotatedAnchorOffset                          // anchor pos
            };

            mMath.GetRotatedPoints(Rotation, center, points);

            return new UIRect(
                topLeft: points[0],
                top: points[1],
                topRight: points[2],

                left: points[3],
                center: center,
                right: points[4],

                bottomLeft: points[5],
                bottom: points[6],
                bottomRight: points[7],
                anchor: points[8]
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
