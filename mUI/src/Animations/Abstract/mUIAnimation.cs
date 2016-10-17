using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.src.Other;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Animations
{
    public enum mUIAnimationDir
    {
        FORWARD,
        BACKWARD
    }

    public enum mUIAnimationState
    {
        PLAYS,
        STOPPED
    }

    public enum mUIAnimationPlayType
    {
        PLAY_ONCE,
        RESET_REPEAT,
        REPEAT
    }

    public static class mUIAnimationHelper
    {
        public static T Duration<T>(this T animation, float duration) where T : mUIAnimation
        {
            animation.Duration = duration;
            return animation;
        }

        public static T PlayType<T>(this T animation, mUIAnimationPlayType type) where T : mUIAnimation
        {
            animation.PlayType = type;
            return animation;
        }

        public static T EasingType<T>(this T animation, mUIEasingType type) where T : mUIAnimation
        {
            animation.EasingType = type;
            return animation;
        }

        public static T State<T>(this T animation, mUIAnimationState state) where T : mUIAnimation
        {
            animation.State = state;
            return animation;
        }
    }

    public abstract class mUIAnimation
    {
        public float Duration { get; set; }
        public float CurrentTime { get { return _animationTime; } }
        public float CurrentEasingTime { get { return _animationEasingTime; } }
        public mUIAnimationPlayType PlayType { get; set; }
        public mUIAnimationState State { get; set; }
        public mUIEasingType EasingType { get; set; }

        public event Action<mUIAnimation> OnEndAnimationEvent;
        protected abstract void OnAnimation();
        protected abstract void OnEndAnimation();
        protected readonly UIGameObject _uiGameObject;

        private float _animationTime;
        private float _animationEasingTime;
        private mUIAnimationDir _animationDir;
        
        protected mUIAnimation(UIGameObject uiGameObject)
        {
            _animationTime = 0;
            _animationEasingTime = 0;
            _uiGameObject = uiGameObject;
            _uiGameObject.Animations.Add(this);
            _animationDir = mUIAnimationDir.FORWARD;
            Duration = 1;
            State = mUIAnimationState.PLAYS;
            PlayType = mUIAnimationPlayType.PLAY_ONCE;
            EasingType = mUIEasingType.linear;

            mUI.Log("Added animation");
        }

        public void Tick()
        {
            if (State == mUIAnimationState.STOPPED)
                return;
             
            _animationTime += (_animationDir == mUIAnimationDir.FORWARD ? 1 : -1) * (Time.deltaTime / Duration);
            _animationTime = mUI.Сlamp(_animationTime, 0, 1);
            _animationEasingTime = mUIEasingFunctions.GetValue(EasingType, 1, _animationTime, 1);

            OnAnimation();

            if (_animationTime >= 1 && _animationDir == mUIAnimationDir.FORWARD || 
                _animationTime <= 0 && _animationDir == mUIAnimationDir.BACKWARD)
            {
                if (PlayType == mUIAnimationPlayType.PLAY_ONCE)
                {
                    Remove();
                    return;
                }

                if (PlayType == mUIAnimationPlayType.RESET_REPEAT)
                {
                    _animationTime = 0;
                }
                else if (PlayType == mUIAnimationPlayType.REPEAT)
                {
                    _animationDir = _animationDir == mUIAnimationDir.FORWARD ? mUIAnimationDir.BACKWARD : mUIAnimationDir.FORWARD;
                }

                OnEndAnimation();
                OnEndAnimationEvent?.Invoke(this);
            }
        }

        public void Remove()
        {
            mUI.Log("Remove animation: " + ToString());
            _uiGameObject.Animations.Remove(this);
        }
    }
}
