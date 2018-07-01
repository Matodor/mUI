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
        // TODO
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
        public UnidirectionalList<UIObject> Childs { get; private set; }

        public virtual Vector2 UnscaledCenterOffset { get; protected set; } = Vector2.zero;

        public virtual Vector2 CenterOffset => new Vector2(
            UnscaledCenterOffset.x * GlobalScale.x,
            UnscaledCenterOffset.y * GlobalScale.y
        );

        public virtual Vector2 LocalCenterOffset => new Vector2(
            UnscaledCenterOffset.x * Scale.x,
            UnscaledCenterOffset.y * Scale.y
        );

        public virtual Vector3 GlobalScale => Transform.lossyScale;

        public virtual float Height => 
            SizeY * GlobalScale.y + (Padding.Top + Padding.Bottom) * GlobalScale.y;
        public virtual float Width => 
            SizeX * GlobalScale.x + (Padding.Left + Padding.Right) * GlobalScale.x;

        public float LocalHeight => SizeY * Scale.y + (Padding.Top + Padding.Bottom) * Scale.y;
        public float LocalWidth => SizeX * Scale.x + (Padding.Left + Padding.Right) * Scale.x;

        public float UnscaledHeight => SizeY + (Padding.Top + Padding.Bottom);
        public float UnscaledWidth => SizeX + (Padding.Left + Padding.Right);

        public virtual float SizeY { get; protected set; }
        public virtual float SizeX { get; protected set; }

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
            get => LocalAnchorPosition(_anchorPivot);
            set => SetLocalPosition(value, _anchorPivot);
        }

        public virtual Vector3 Position
        {
            get => GlobalAnchorPosition(_anchorPivot);
            set => SetGlobalPosition(value, _anchorPivot);
        }

        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set => ScaleByAnchor(value, _anchor);
        }

        public virtual UIRect Rect => UIRect(UIRectType.GLOBAL);

        public float Rotation
        {
            get => Transform.eulerAngles.z;
            set => RotationAround(Position, value, Space.World);
        }

        public float LocalRotation
        {
            get => Transform.localEulerAngles.z;
            set => RotationAround(Position, value, Space.Self);
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

        internal void RotationAround(Vector3 point, float rotation, Space relativeTo)
        {
            TurnAround(point, relativeTo == Space.Self
                ? rotation - Transform.localEulerAngles.z
                : rotation - Transform.eulerAngles.z
            );
        }

        internal void TurnAround(Vector3 point, float turnValue)
        {
            Transform.RotateAround(point, Vector3.forward, turnValue);
        }

        internal void ScaleByAnchor(Vector2 scale, UIAnchor anchor)
        {
            var position = GlobalAnchorPosition(PivotByAnchor(anchor));
            Transform.localScale = scale;
            SetGlobalPosition(position, PivotByAnchor(anchor));
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
            //Transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            GUID = ++_guid;
            Parent = null;
            Animations = UnidirectionalList<UIAnimation>.Create();
            Childs = UnidirectionalList<UIObject>.Create();

            _destroyed = false;
            _localSortingOrder = 0;

            SetName(base.gameObject.name);
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

        public Vector2 BoundLocalCenter()
        {
            return mMath.GetRotatedPoint(Transform.localPosition, LocalCenterOffset, LocalRotation) -
                   (Parent?.LocalCenterOffset ?? Vector2.zero);
        }

        public Vector2 BoundGlobalCenter()
        {
            return mMath.GetRotatedPoint(Transform.position, CenterOffset, Rotation);
        }

        public Vector2 AnchorShiftFromBoundLocalCenter(Vector2 anchorPivot, float? rotation = null)
        {
            var w = SizeX;
            var h = SizeY;
            var lowerLeft = new Vector2(-w / 2f - _padding.Left, -h / 2f - _padding.Bottom);
            var upperRight = new Vector2(+w / 2f + _padding.Right, +h / 2f + _padding.Top);

            return mMath.GetRotatedPoint(Vector2.zero, new Vector2(
                BezierHelper.Linear(anchorPivot.x, lowerLeft.x, upperRight.x) * Scale.x,
                BezierHelper.Linear(anchorPivot.y, lowerLeft.y, upperRight.y) * Scale.y
            ), rotation ?? LocalRotation);
        }

        public Vector2 AnchorShiftFromBoundGlobalCenter(Vector2 anchorPivot, float? rotation = null)
        {
            var w = SizeX;
            var h = SizeY;
            var lowerLeft = new Vector2(-w / 2f - _padding.Left, -h / 2f - _padding.Bottom);
            var upperRight = new Vector2(+w / 2f + _padding.Right, +h / 2f + _padding.Top);

            return mMath.GetRotatedPoint(Vector2.zero, new Vector2(
                BezierHelper.Linear(anchorPivot.x, lowerLeft.x, upperRight.x) * GlobalScale.x,
                BezierHelper.Linear(anchorPivot.y, lowerLeft.y, upperRight.y) * GlobalScale.y
            ), rotation ?? Rotation);
        }

        public Vector2 GlobalAnchorPosition(Vector2 anchorPivot)
        {
            return BoundGlobalCenter() + AnchorShiftFromBoundGlobalCenter(anchorPivot);
        }

        public Vector2 LocalAnchorPosition(Vector2 anchorPivot)
        {
            return BoundLocalCenter() + AnchorShiftFromBoundLocalCenter(anchorPivot) -
                (Parent?.AnchorShiftFromBoundLocalCenter(Parent._anchorPivot, 0f) ?? Vector2.zero);
        }

        public void SetGlobalPosition(Vector2 position, Vector2 anchorPivot)
        {
            position = mMath.GetRotatedPoint(position, -CenterOffset, Rotation);
            Transform.position = position - AnchorShiftFromBoundGlobalCenter(anchorPivot);
        }

        public void SetLocalPosition(Vector2 position, Vector2 anchorPivot)
        {
            position = mMath.GetRotatedPoint(position, -LocalCenterOffset, LocalRotation) +
                (Parent?.LocalCenterOffset ?? Vector2.zero);

            Transform.localPosition = position - AnchorShiftFromBoundLocalCenter(anchorPivot) +
                (Parent?.AnchorShiftFromBoundLocalCenter(Parent._anchorPivot, 0f) ?? Vector2.zero);
        }
        
        public UIRect UIRect(UIRectType type, Vector2 shift, bool rotated)
        {
            float w;
            float h;
            Vector2 paddingShift;

            switch (type)
            {
                case UIRectType.UNSCALED:
                    w = UnscaledWidth;
                    h = UnscaledHeight;
                    paddingShift = new Vector2(
                        (-_padding.Left + _padding.Right) / 2f,
                        (-_padding.Bottom + _padding.Top) / 2f
                    );
                    break;
                case UIRectType.LOCAL:
                    w = LocalWidth;
                    h = LocalHeight;
                    paddingShift = new Vector2(
                        ((-_padding.Left + _padding.Right) * Scale.x) / 2f,
                        ((-_padding.Bottom + _padding.Top) * Scale.y) / 2f
                    );
                    break;
                default:
                    w = Width;
                    h = Height;
                    paddingShift = new Vector2(
                        ((-_padding.Left + _padding.Right) * GlobalScale.x) / 2f,
                        ((-_padding.Bottom + _padding.Top) * GlobalScale.y) / 2f
                    );
                    break;
            }

            var rect = new UIRect
            {
                Type = type,
                UpperLeft = new Vector2(-w / 2f, h / 2f),
                UpperCenter = new Vector2(0f, h / 2f),
                UpperRight = new Vector2(w / 2f, h / 2f),

                MiddleLeft = new Vector2(-w / 2f, 0f),
                MiddleCenter = new Vector2(0f, 0f),
                MiddleRight = new Vector2(w / 2f, 0f),

                LowerLeft = new Vector2(-w / 2f, -h / 2f),
                LowerCenter = new Vector2(0f, -h / 2f),
                LowerRight = new Vector2(w / 2f, -h / 2f),
            };

            switch (type)
            {
                case UIRectType.UNSCALED:
                    break;
                case UIRectType.LOCAL:
                    rect.MiddleCenter = mMath.GetRotatedPoint(BoundLocalCenter(), 
                        paddingShift, LocalRotation) + shift;
                    if (rotated)
                        UIUtils.RotateRect(ref rect, LocalRotation);
                    break;
                default:
                    rect.MiddleCenter = mMath.GetRotatedPoint(BoundGlobalCenter(), 
                        paddingShift, Rotation) + shift;
                    if (rotated)
                        UIUtils.RotateRect(ref rect, Rotation);
                    break;
            }

            return rect;
        }

        public UIRect UIRect(UIRectType type, bool rotated)
        {
            return UIRect(type, Vector2.zero, rotated);
        }

        public UIRect UIRect(UIRectType type)
        {
            return UIRect(type, Vector2.zero, true);
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
