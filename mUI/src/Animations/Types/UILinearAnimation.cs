using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Other;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UILinearAnimationHelper
    {
        public static UILinearAnimation LinearAnimation<T>(this T obj, Vector2 startPos, Vector2 endPos) where T : UIGameObject
        {
            return new UILinearAnimation(startPos, endPos, obj);
        }
    }

    public class UILinearAnimation : mUIAnimation
    {
        private readonly Vector2 _startPos, _endPos;

        public UILinearAnimation(Vector2 startPos, Vector2 endPos, UIGameObject uiGameObject) : base(uiGameObject)
        {
            _startPos = startPos;
            _endPos = endPos;
        }

        protected override void OnAnimation()
        {
            var newPos = mUIBezierHelper.Linear(CurrentEasingTime, _startPos, _endPos);
            _uiGameObject.Transform.position = new Vector3(
                newPos.x, newPos.y, _uiGameObject.Transform.position.z
            );
        }

        //protected override void OnStartAnimation()
        //{
        //    
        //}

        protected override void OnEndAnimation()
        {
            mUI.Log("Time: " + Time.time);
        }
    }
}
