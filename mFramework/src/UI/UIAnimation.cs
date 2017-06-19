using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace mFramework.UI
{
    public enum UIAnimationPlayType
    {
        PLAY_ONCE = 0,
        END_RESET = 1,
        END_FLIP = 2
    }

    public enum UIAnimationState
    {
        STOPPED = 0,
        PLAYING = 1
    }

    public enum UIAnimationDirection
    {
        FORWARD,
        BACKWARD
    }

    public class UIAnimationSettings
    {
        public UIAnimationPlayType PlayType { get; set; } = UIAnimationPlayType.PLAY_ONCE;
        public EasingType EasingType { get; set; } = EasingType.linear;
        public bool DestroyUIObjectOnEnd { get; set; } = false;
        public float Duration { get; set; } = 1f;
    }

    public abstract class UIAnimation : IGlobalUniqueIdentifier
    {
        public float CurrentTime { get { return _animationTime; } }
        public float CurrentEasingTime { get { return _animationEasingTime; } }

        public ulong GUID { get; }
        public UIAnimationState State { get; set; }
        public UIAnimationPlayType PlayType { get; set; }
        public EasingType EasingType { get; set; }

        public bool DestroyUIObjectOnEnd { get; set; }
        public float Duration { get; set; }

        public event Action<UIAnimation> OnAnimateEvent;
        public event Action<UIAnimation> OnEndEvent;
        public event Action<UIAnimation> OnStartEvent;

        protected readonly UIObject _animatedObject;

        private static ulong _guid;

        private UIAnimationDirection _animationDirection;
        private float _animationStart;
        private float _animationTime;
        private float _animationEasingTime;

        protected UIAnimation(UIObject animatedObject)
        {
            _animatedObject = animatedObject;
            _animationDirection = UIAnimationDirection.FORWARD;
            _animationTime = 0;
            _animationEasingTime = 0;

            State = UIAnimationState.PLAYING;
            PlayType = UIAnimationPlayType.PLAY_ONCE;
            EasingType = EasingType.linear;
            DestroyUIObjectOnEnd = false;

            GUID = ++_guid;
        }

        internal static T Create<T>(UIObject parent, UIAnimationSettings settings) where T : UIAnimation
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var animation = (T)
                Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new object[] {parent}, CultureInfo.InvariantCulture);

            animation.ApplySettings(settings);
            return animation;
        }

        public void SetStartOffset(float time)
        {
            _animationStart = Time.time + time;
        }

        public void SetAnimationPos(float time)
        {
            _animationTime = mMath.Сlamp(time, 0, 1);
        }

        protected virtual void ApplySettings(UIAnimationSettings settings)
        {
            DestroyUIObjectOnEnd = settings.DestroyUIObjectOnEnd;
            Duration = settings.Duration;
            EasingType = settings.EasingType;
            PlayType = settings.PlayType;
        }

        protected virtual void OnAnimate()
        {
            OnAnimateEvent?.Invoke(this);
        }

        protected virtual void OnStartAnimation()
        {
            OnStartEvent?.Invoke(this);
        }

        protected virtual void OnEndAnimation()
        {
            OnEndEvent?.Invoke(this);
        }

        internal void Tick()
        {
            if (State == UIAnimationState.STOPPED || Time.time < _animationStart)
                return;

            _animationTime += (_animationDirection == UIAnimationDirection.FORWARD ? 1 : -1) * (Time.deltaTime / Duration);
            _animationTime = mMath.Сlamp(_animationTime, 0, 1);
            _animationEasingTime = EasingFunctions.GetValue(EasingType, 1, _animationTime, 1);

            OnAnimate();

            if (_animationTime >= 1 && _animationDirection == UIAnimationDirection.FORWARD ||
                _animationTime <= 0 && _animationDirection == UIAnimationDirection.BACKWARD)
            {
                OnEndAnimation();

                if (PlayType == UIAnimationPlayType.END_RESET)
                {
                    _animationTime = 0;
                }
                else if (PlayType == UIAnimationPlayType.END_FLIP)
                {
                    _animationDirection = _animationDirection == UIAnimationDirection.FORWARD 
                        ? UIAnimationDirection.BACKWARD 
                        : UIAnimationDirection.FORWARD;
                }
                else if (PlayType == UIAnimationPlayType.PLAY_ONCE)
                {
                    State = UIAnimationState.STOPPED;
                    Remove();

                    if (DestroyUIObjectOnEnd)
                    {
                        _animatedObject.Destroy();
                    }
                }
            }
        }

        public bool Remove()
        {
            return _animatedObject.RemoveAnimation(this);
        }
    }
}
