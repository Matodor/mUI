using System;
using System.Globalization;
using System.Linq.Expressions;
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
        public ulong MaxRepeats = 0;
        public UIAnimationPlayType PlayType = UIAnimationPlayType.PLAY_ONCE;
        public EasingType EasingType = EasingType.linear;
        public bool DestroyUIObjectOnEnd = false;
        public float Duration = 1f;
        public float AnimateEvery = 0f;
    }

    public delegate void UIAnimationEventHandler(UIAnimation sender);

    public static class NewAnimation<T> where T : UIAnimation
    {
        public static readonly Func<UIObject, T> Instance;

        static NewAnimation()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null,
                new Type[] { typeof(UIObject) }, null);
            var parameter = Expression.Parameter(typeof(UIObject), "animatedObject");
            var e = Expression.New(constructor, parameter);
            Instance = Expression.Lambda<Func<UIObject, T>>(e, parameter).Compile();
        }
    }

    public abstract class UIAnimation : IGlobalUniqueIdentifier
    {
        public float CurrentTime => _animationTime;
        public float CurrentEasingTime => _animationEasingTime;

        public ulong GUID { get; }
        public UIAnimationState State { get; set; }
        public UIAnimationPlayType PlayType { get; set; }
        public EasingType EasingType { get; set; }
        public UIAnimationDirection Direction { get; set; }

        public float AnimateEvery { get; set; }
        public bool DestroyUIObjectOnEnd { get; set; }
        public float Duration { get; set; }
        public ulong MaxRepeats { get; set; }
        public ulong RepeatsNumber => _repeats;
        public UIObject AnimatedObject => _animatedObject;

        public event UIAnimationEventHandler AnimationRepeat;
        public event UIAnimationEventHandler AnimationEnded;

        private readonly UIObject _animatedObject;

        private static ulong _guid;
        private ulong _repeats;

        private float _nextAnimationFrame;
        private float _animationStart;
        private float _animationTime;
        private float _animationEasingTime;
        
        protected UIAnimation(UIObject animatedObject)
        {
            Direction = UIAnimationDirection.FORWARD;
            _animatedObject = animatedObject;
            _animationTime = 0;
            _animationEasingTime = 0;
            _repeats = 0;
            _nextAnimationFrame = Time.time;

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

            var animation = NewAnimation<T>.Instance(parent);
            animation.ApplySettings(settings);
            
            //(T)
            //    Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null,
            //        new object[] { parent }, CultureInfo.InvariantCulture);

            return animation;
        }

        public void SetStartOffset(float time)
        {
            _animationStart = Time.time + time;
        }

        public void SetAnimationPos(float time)
        {
            _animationTime = mMath.Clamp(time, 0, 1);
        }

        public void UpdateAnimation()
        {
            Animate(true);
        }

        protected virtual void ApplySettings(UIAnimationSettings settings)
        {
            MaxRepeats = settings.MaxRepeats;
            DestroyUIObjectOnEnd = settings.DestroyUIObjectOnEnd;
            Duration = settings.Duration;
            EasingType = settings.EasingType;
            PlayType = settings.PlayType;
            AnimateEvery = settings.AnimateEvery;
            UpdateAnimation();
        }

        protected abstract void OnAnimate();

        private void OnRepeatAnimation()
        {
            _repeats++;
            AnimationRepeat?.Invoke(this);
        }

        private void OnEndAnimation()
        {
            AnimationEnded?.Invoke(this);
        }

        public void Animate(bool forcibly = false)
        {
            _animationEasingTime = EasingFunctions.GetValue(EasingType, 1f, _animationTime, 1f);

            if (forcibly || _nextAnimationFrame <= Time.time)
            {
                OnAnimate();
                _nextAnimationFrame = Time.time + AnimateEvery;
            }

            if (_animationTime >= 1f && Direction == UIAnimationDirection.FORWARD ||
                _animationTime <= 0f && Direction == UIAnimationDirection.BACKWARD)
            {
                if (PlayType == UIAnimationPlayType.END_RESET)
                {
                    OnRepeatAnimation();
                    _animationTime = 0;
                }
                else if (PlayType == UIAnimationPlayType.END_FLIP)
                {
                    OnRepeatAnimation();
                    Direction = Direction == UIAnimationDirection.FORWARD
                        ? UIAnimationDirection.BACKWARD
                        : UIAnimationDirection.FORWARD;
                }

                if (PlayType == UIAnimationPlayType.PLAY_ONCE ||
                    MaxRepeats > 0 && _repeats >= MaxRepeats)
                {
                    State = UIAnimationState.STOPPED;
                    OnEndAnimation();
                    Remove();

                    if (DestroyUIObjectOnEnd)
                        _animatedObject.Destroy();
                }
            }
        }

        internal void Tick()
        {
            if (State == UIAnimationState.STOPPED || Time.time < _animationStart)
                return;

            _animationTime += (Direction == UIAnimationDirection.FORWARD ? 1f : -1f) * (Time.deltaTime / Duration);
            _animationTime = mMath.Clamp(_animationTime, 0f, 1f);

            Animate();
        }

        public void Remove()
        {
            AnimationRepeat = null;
            AnimationEnded = null;
            AnimatedObject.RemoveAnimation(this);
        }
    }
}
