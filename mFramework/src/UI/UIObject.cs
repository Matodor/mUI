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

    public class UIObjectProps
    {
        public virtual UIAnchor? Anchor { get; set; }
        public virtual UIPadding? Padding { get; set; }
        public virtual int SortingOrder { get; set; } = 0;
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
        public UnidirectionalList<UIObject> Childs { get; private set; }

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

        public virtual UIPadding Padding
        {
            get => _padding;
            set
            {
                var pos = Position;
                _padding = value;
                Position = pos;
            }
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
            get => GetLocalAnchorPos(_anchorPivot);
            set => LocalPositionByPivot(value, _anchorPivot);
        }

        public virtual Vector3 Position
        {
            get => GetAnchorPos(_anchorPivot);
            set => PositionByPivot(value, _anchorPivot);
        }

        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set => ScaleByAnchor(value, _anchor);
        }

        public virtual UIRect Rect => GetRect(Transform.position);

        public float Rotation
        {
            get => Transform.eulerAngles.z;
            set => RotateAround(Position, value, Space.World);
        }

        public float LocalRotation
        {
            get => Transform.localEulerAngles.z;
            set => RotateAround(Position, value, Space.Self);
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
        public event UIEventHandler<IUIObject, IUIObject> ChildAdded = delegate { };
        public event UIEventHandler<IUIObject, UIAnimation> AnimationAdded = delegate { };
        #endregion

        private Vector2 _anchorPivot;
        private UIAnchor _anchor;
        private UIPadding _padding;
        private int _localSortingOrder;
        private bool _destroyed;
        private static ulong _guid;

        protected virtual void AfterAwake()
        {
        }

        protected virtual void ApplyProps(UIObjectProps props)
        {
            _anchor = props.Anchor.GetValueOrDefault(UIAnchor.MiddleCenter);
            _anchorPivot = PivotByAnchor(_anchor);
            _padding = props.Padding.GetValueOrDefault(new UIPadding());
            _localSortingOrder = props.SortingOrder;
        }

        internal void RotateAround(Vector3 point, float rotation, Space relativeTo)
        {
            Transform.RotateAround(point, Vector3.forward, rotation - (relativeTo == Space.Self
                ? Transform.localEulerAngles.z
                : Transform.eulerAngles.z)
            );
        }

        internal void ScaleByAnchor(Vector2 scale, UIAnchor anchor)
        {
            var pos = Transform.localPosition + 
                AnchorOffsetFromTransformPos(PivotByAnchor(anchor), Rotation);

            Transform.localScale = scale;
            Transform.localPosition = pos - AnchorOffsetFromTransformPos(PivotByAnchor(anchor), Rotation);
        }

        internal void InitCompleted(bool setPosition)
        {
            if (Parent != null)
            {
                if (setPosition)
                {
                    Position = Parent.Position;
                    Rotation = Parent.Rotation;
                }

                Parent.Childs.Add(this);
                Parent.ChildAdded(Parent, this);
            }

            OnSortingOrderChanged();
        }

        internal void SetupParent(UIObject parent)
        {
            Parent?.Childs.Remove(this);
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

        private void Awake()
        {
            Transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            GUID = ++_guid;
            Parent = null;
            Animations = UnidirectionalList<UIAnimation>.Create();
            Childs = UnidirectionalList<UIObject>.Create();

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

        internal Vector3 GetLocalAnchorPos(Vector2 pivot)
        {
            return Transform.localPosition -
                   (Parent?.AnchorOffsetFromTransformPos(Parent._anchorPivot, 0) ?? Vector3.zero) +
                   AnchorOffsetFromTransformPos(pivot, LocalRotation);
        }

        internal Vector3 GetAnchorPos(Vector2 pivot)
        {
            return Transform.position +
                   AnchorOffsetFromTransformPos(pivot, Rotation);
        }

        internal Vector3 GetLocalAnchorPos(UIAnchor anchor)
        {
            return GetLocalAnchorPos(PivotByAnchor(anchor));
        }

        internal Vector3 GetAnchorPos(UIAnchor anchor)
        {
            return GetAnchorPos(PivotByAnchor(anchor));
        }

        internal void LocalPositionByPivot(Vector3 pos, Vector2 anchorPivot)
        {
            Transform.localPosition = pos +
                (Parent?.AnchorOffsetFromTransformPos(Parent._anchorPivot, 0) ?? Vector3.zero) -
                AnchorOffsetFromTransformPos(anchorPivot, LocalRotation);
        }

        internal void PositionByPivot(Vector3 pos, Vector2 anchorPivot)
        {
            Transform.position = pos - 
                AnchorOffsetFromTransformPos(anchorPivot, Rotation);
        } 

        private static Vector2 NotRotatedAnchorOffset(NotRotatedRect rect, Vector2 anchorPivot)
        {
            return new Vector2(
                BezierHelper.Linear(anchorPivot.x, rect.LocalLeft, rect.LocalRight),
                BezierHelper.Linear(anchorPivot.y, rect.LocalBottom, rect.LocalTop)
            );
        }

        internal NotRotatedRect NotRotatedLocalRect(Vector2 scale)
        {
            var centerOffset = CenterOffset;
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

        internal Vector3 AnchorOffsetFromTransformPos(Vector2 anchorPivot, float rotation)
        {
            var rect = NotRotatedLocalRect(GlobalScale);
            var notRotatedAnchorOffset = NotRotatedAnchorOffset(rect, anchorPivot);
            var center = mMath.GetRotatedPoint(Vector2.zero, rect.CenterOffset, rotation);
            return mMath.GetRotatedPoint(center, notRotatedAnchorOffset, rotation);
        }

        internal UIRect GetRect(Vector2 transformPosition)
        {
            var rect = NotRotatedLocalRect(GlobalScale);
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
        
        protected virtual void OnBeforeDestroy()
        {
            
        }

        internal void DestroyImpl(bool removeFromParent)
        {
            if (_destroyed)
                return;

            DestroyChilds();
            RemoveAnimations();

            _destroyed = true;
            OnBeforeDestroy();
            BeforeDestroy(this);

            if (removeFromParent)
                Parent?.Childs.Remove(this);
            mUI.RemoveUIObject(this);

            ActiveChanged = null;
            VisibleChanged = null;
            SortingOrderChanged = null;
            BeforeDestroy = null;
            ChildAdded = null;
            AnimationAdded = null;

            UnityEngine.Object.Destroy(base.gameObject);
        }

        private void OnDestroy()
        {
            DestroyImpl(true);
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

        public void DestroyChilds()
        {
            Childs.Clear(o => o.DestroyImpl(false));
        }

        public void Destroy()
        {
            DestroyImpl(true);
        }

        public IUIObject SetName(string newName)
        {
            if (mCore.IsEditor)
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

        public virtual T Component<T>(UIComponentProps props) where T : UIComponent
        {
            return UIComponent.Create<T>(this, props);
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
            Animations.Clear(a => a.RemoveInternal());
        }

        public void RemoveAnimations<T>() where T : UIAnimation
        {
            Animations.ForEach(a =>
            {
                if (a is T)
                    a.Remove();
            });
        }
    }
}
