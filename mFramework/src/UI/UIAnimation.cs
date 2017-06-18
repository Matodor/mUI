using System;
using System.Globalization;
using System.Reflection;

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

    public class UIAnimationSettings
    {
        public float Duration { get; set; }
    }

    public abstract class UIAnimation
    {
        public UIAnimationState State { get; set; }
        public UIAnimationPlayType PlayType { get; set; }

        public event Action<UIAnimation> OnAnimateEvent;
        public event Action<UIAnimation> OnEndEvent;
        public event Action<UIAnimation> OnStartEvent;

        private readonly UIObject _parentObject;

        protected UIAnimation(UIObject parentObject)
        {
            _parentObject = parentObject;
            _parentObject.AddAnimation(this);
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

        protected virtual void ApplySettings(UIAnimationSettings settings)
        {

        }

        protected virtual void OnAnimate()
        {
            
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
            if (State == UIAnimationState.STOPPED)
                return;

        }

        public void Remove()
        {

        }
    }
}
