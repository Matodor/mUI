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

    public abstract class UIAnimationSettings
    {
        public ulong MaxRepeats = 0;
        public UIAnimationPlayType PlayType = UIAnimationPlayType.PLAY_ONCE;
        public EasingType EasingType = EasingType.linear;
        public bool DestroyUIObjectOnEnd = false;
        public float Duration = 1f;
        public float AnimateEvery = 0f;
    }
    
    public abstract class UIAnimation : IGlobalUniqueIdentifier
    {
        internal bool MarkedForDestroy { get; private set; }

        public float CurrentTime => _animationTime;
        public float CurrentEasingTime => _animationEasingTime;

        public ulong GUID { get; }
        public UIAnimationState State { get; set; }
        public UIAnimationPlayType PlayType { get; set; }
        public EasingType EasingType { get; set; }

        public float AnimateEvery { get; set; }
        public bool DestroyUIObjectOnEnd { get; set; }
        public float Duration { get; set; }
        public ulong MaxRepeats { get; set; }
        public ulong RepeatsNumber => _repeats;
        public UIObject AnimatedObject => _animatedObject;

        public event EventHandler<AnimationEventArgs> AnimationRepeat;
        public event EventHandler<AnimationEventArgs> AnimationEnded;
        
        private readonly UIObject _animatedObject;

        private static ulong _guid;
        private ulong _repeats;

        private float _nextAnimationFrame;
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
            _repeats = 0;
            _nextAnimationFrame = Time.realtimeSinceStartup;

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
            _animationStart = Time.realtimeSinceStartup + time;
        }

        public void SetAnimationPos(float time)
        {
            _animationTime = mMath.Clamp(time, 0, 1);
        }

        public void UpdateAnimation()
        {
            Tick();
        }

        protected virtual void ApplySettings(UIAnimationSettings settings)
        {
            MaxRepeats = settings.MaxRepeats;
            DestroyUIObjectOnEnd = settings.DestroyUIObjectOnEnd;
            Duration = settings.Duration;
            EasingType = settings.EasingType;
            PlayType = settings.PlayType;
            AnimateEvery = settings.AnimateEvery;
        }

        protected abstract void OnAnimate();

        private void OnRepeatAnimation()
        {
            _repeats++;
            AnimationRepeat?.Invoke(this, new AnimationEventArgs(this));
        }

        private void OnEndAnimation()
        {
            AnimationEnded?.Invoke(this, new AnimationEventArgs(this));
        }

        internal void Tick()
        {
            if (State == UIAnimationState.STOPPED || 
                Time.realtimeSinceStartup < _animationStart)
                return;

            _animationTime += (_animationDirection == UIAnimationDirection.FORWARD ? 1f : -1f) * (Time.deltaTime / Duration);
            _animationEasingTime = EasingFunctions.GetValue(EasingType, 1f, _animationTime, 1f);

            if (_animationTime >= 1 && _animationDirection == UIAnimationDirection.FORWARD)
            {
                _animationEasingTime = 1f;
                _animationTime = 1f;
            }
            else if (_animationTime <= 0 && _animationDirection == UIAnimationDirection.BACKWARD)
            {
                _animationEasingTime = 0f;
                _animationTime = 0f;
            }

            if (_animatedObject.IsVisible && _nextAnimationFrame <= Time.realtimeSinceStartup)
            {
                OnAnimate();
                _nextAnimationFrame = Time.realtimeSinceStartup + AnimateEvery;
            }

            if (_animationTime >= 1 && _animationDirection == UIAnimationDirection.FORWARD ||
                _animationTime <= 0 && _animationDirection == UIAnimationDirection.BACKWARD)
            {
                if (PlayType == UIAnimationPlayType.END_RESET)
                {
                    OnRepeatAnimation();
                    _animationTime = 0;
                }
                else if (PlayType == UIAnimationPlayType.END_FLIP)
                {
                    OnRepeatAnimation();
                    _animationDirection = _animationDirection == UIAnimationDirection.FORWARD 
                        ? UIAnimationDirection.BACKWARD 
                        : UIAnimationDirection.FORWARD;
                }

                if (PlayType == UIAnimationPlayType.PLAY_ONCE ||
                    MaxRepeats > 0 && _repeats >= MaxRepeats)
                {
                    OnEndAnimation();
                    State = UIAnimationState.STOPPED;
                    Remove();

                    if (DestroyUIObjectOnEnd)
                        _animatedObject.Destroy();
                }
            }
        }

        public void Remove()
        {
            MarkedForDestroy = true;
        }
    }
}
