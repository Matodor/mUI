using System;

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
        public EasingFunctions EasingType = EasingFunctions.Linear;
        public bool DestroyUIObjectOnEnd = false;
        public float Duration = 1f;
        public float AnimateEvery = 0f;
    }

    public delegate void UIAnimationEventHandler(UIAnimation sender);

    /*public static class NewAnimation<T> where T : UIAnimation
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
    }*/

    public abstract class UIAnimation : IGlobalUniqueIdentifier
    {
        /// <summary>
        /// Normilized animation time
        /// </summary>
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }

        /// <summary>
        /// Normilized animation easing time
        /// </summary>
        public float EasingTime { get; private set; }
        public float DeltaEasingTime { get; private set; }
        public float Duration { get; set; }
        public readonly float AttachTime;

        public UIObject UIObject { get; private set; }
        public ulong GUID { get; }

        public UIAnimationState State { get; set; }
        public UIAnimationPlayType PlayType { get; set; }
        public EasingFunctions EasingType { get; set; }
        public UIAnimationDirection Direction { get; set; }

        public float AnimateEvery { get; set; }
        public bool DestroyUIObjectOnEnd { get; set; }
        public ulong MaxRepeats { get; set; }
        public ulong Repeats { get; private set; }

        public event UIAnimationEventHandler AnimationRepeat;
        public event UIAnimationEventHandler AnimationEnded;
        public event UIAnimationEventHandler AnimationRemoved;

        private static ulong _guid;

        private float _nextAnimationFrame;

        protected UIAnimation()
        {
            Direction = UIAnimationDirection.FORWARD;
            Time = 0;
            EasingTime = 0;
            Repeats = 0;
            _nextAnimationFrame = UnityEngine.Time.time;

            State = UIAnimationState.PLAYING;
            PlayType = UIAnimationPlayType.PLAY_ONCE;
            EasingType = EasingFunctions.Linear;
            DestroyUIObjectOnEnd = false;

            GUID = ++_guid;
            AttachTime = UnityEngine.Time.time;
        }

        internal static T Create<T>(UIObject parent, UIAnimationSettings settings) 
            where T : UIAnimation, new()
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var animation = new T {UIObject = parent};
            animation.ApplySettings(settings);
            return animation;
        }

        public void SetTime(float time)
        {
            Time = mMath.Clamp(time, 0, 1);
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
            Repeats++;
            AnimationRepeat?.Invoke(this);
        }

        private void OnEndAnimation()
        {
            AnimationEnded?.Invoke(this);
        }

        private void Animate(bool forcibly = false)
        {
            // TODO: выключение анимации, когда объект скрыт
            if (forcibly || _nextAnimationFrame <= UnityEngine.Time.time)
            {
                OnAnimate();
                _nextAnimationFrame = UnityEngine.Time.time + AnimateEvery;
            }

            if (Time >= 1f && Direction == UIAnimationDirection.FORWARD ||
                Time <= 0f && Direction == UIAnimationDirection.BACKWARD)
            {
                if (PlayType == UIAnimationPlayType.END_RESET)
                {
                    OnRepeatAnimation();
                    Time = 0f;
                    EasingTime = 0f;
                }
                else if (PlayType == UIAnimationPlayType.END_FLIP)
                {
                    OnRepeatAnimation();
                    Direction = Direction == UIAnimationDirection.FORWARD
                        ? UIAnimationDirection.BACKWARD
                        : UIAnimationDirection.FORWARD;
                }

                if (PlayType == UIAnimationPlayType.PLAY_ONCE ||
                    MaxRepeats > 0 && Repeats >= MaxRepeats)
                {
                    State = UIAnimationState.STOPPED;
                    OnEndAnimation();
                    Remove();

                    if (DestroyUIObjectOnEnd)
                        UIObject.Destroy();
                }
            }
        }

        internal void Tick()
        {
            if (State == UIAnimationState.STOPPED)
                return;

            DeltaTime = (Direction == UIAnimationDirection.FORWARD ? 1f : -1f) *
                        (UnityEngine.Time.deltaTime / Duration);
            Time = mMath.Clamp(Time + DeltaTime, 0f, 1f);

            var easingTime = Easings.Interpolate(Time, EasingType);
            DeltaEasingTime = easingTime - EasingTime;
            EasingTime = easingTime;

            Animate();
        }

        internal void RemoveInternal()
        {
            State = UIAnimationState.STOPPED;
            AnimationRemoved?.Invoke(this);
            AnimationRepeat = null;
            AnimationEnded = null;
            AnimationRemoved = null;
        }

        public void Remove()
        {
            UIObject.Animations.Remove(this);
            RemoveInternal();
        }
    }
}
