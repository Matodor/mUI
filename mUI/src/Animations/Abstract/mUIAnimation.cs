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

        public static T OffsetStart<T>(this T animation, float offset) where T : mUIAnimation
        {
            animation.SetOffsetStart(offset);
            return animation;
        }

        public static T AnimationPos<T>(this T animation, float pos) where T : mUIAnimation
        {
            animation.SetAnimationPos(pos);
            return animation;
        }

        public static T DestroyOnEnd<T>(this T animation) where T : mUIAnimation
        {
            animation.DestroyOnEnd = true;
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
        public UIObject UIObject { get { return _uiObject; } }
        public bool DestroyOnEnd { get; set; }

        public event Action<mUIAnimation> OnEndAnimationEvent;
        public event Action<mUIAnimation> OnAnimationEvent;

        protected abstract void OnAnimation();
        protected abstract void OnEndAnimation();

        private readonly UIObject _uiObject;
        private float _animationStart;
        private float _animationTime;
        private float _animationEasingTime;
        private mUIAnimationDir _animationDir;

        ~mUIAnimation()
        {
            //mUI.Log("Destroy animation: {0}", ToString());
        }

        protected mUIAnimation(UIObject obj)
        {
            DestroyOnEnd = false;
            Duration = 1;
            State = mUIAnimationState.PLAYS;
            PlayType = mUIAnimationPlayType.PLAY_ONCE;
            EasingType = mUIEasingType.linear;

            _animationTime = 0;
            _animationEasingTime = 0;
            _uiObject = obj;
            _uiObject.AddAnimation(this);
            _uiObject.OnTick += Tick;
            _animationDir = mUIAnimationDir.FORWARD;
        }

        public void SetOffsetStart(float time)
        {
            _animationStart = Time.time + time;
        }

        public void SetAnimationPos(float time)
        {
            _animationTime = mUI.Сlamp(time, 0, 1);
        }

        private void Tick(UIObject obj)
        {
            if (State == mUIAnimationState.STOPPED || Time.time < _animationStart)
                return;
             
            _animationTime += (_animationDir == mUIAnimationDir.FORWARD ? 1 : -1) * (Time.deltaTime / Duration);
            _animationTime = mUI.Сlamp(_animationTime, 0, 1);
            _animationEasingTime = mUIEasingFunctions.GetValue(EasingType, 1, _animationTime, 1);

            OnAnimation();
            OnAnimationEvent?.Invoke(this);

            if (_animationTime >= 1 && _animationDir == mUIAnimationDir.FORWARD || 
                _animationTime <= 0 && _animationDir == mUIAnimationDir.BACKWARD)
            {
                OnEndAnimation();
                OnEndAnimationEvent?.Invoke(this);

                if (PlayType == mUIAnimationPlayType.RESET_REPEAT)
                {
                    _animationTime = 0;
                }
                else if (PlayType == mUIAnimationPlayType.REPEAT)
                {
                    _animationDir = _animationDir == mUIAnimationDir.FORWARD ? mUIAnimationDir.BACKWARD : mUIAnimationDir.FORWARD;
                }
                else if (PlayType == mUIAnimationPlayType.PLAY_ONCE)
                {
                    State = mUIAnimationState.STOPPED;
                    Remove();

                    if (DestroyOnEnd)
                    {
                        _uiObject.Destroy();
                    }
                }
            }
        }

        public void Remove()
        {
            //mUI.Log("Remove animation: " + ToString());
            State = mUIAnimationState.STOPPED;
            _uiObject.RemoveAnimation(this);
            _uiObject.OnTick -= Tick;
        }
    }
}
