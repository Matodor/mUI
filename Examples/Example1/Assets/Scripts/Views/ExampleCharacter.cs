using System;
using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleCharacter : BaseExampleView
    {
        private UIRotateAnimation _headRotateAnimation;
        private UIScaleAnimation _headScaleAnimation;

        private UIAnimation _leftHandRotationAnimation;
        private UIAnimation _leftHandMoveAnimation;

        private UIAnimation _rightHandRotationAnimation;
        private UIAnimation _rightHandMoveAnimation;

        private UISprite _body;
        private UISprite _head;

        private UISprite _leftHand;
        private UISprite _rightHand;

        private UISprite _leftLeg;
        private UISprite _rightLeg;

        private UIContainer _characterContainer;

        private DateTime _lastClick;

        protected override void CreateInterface(object[] @params)
        {
            var mouseEventListener = MouseEventListener.Create();
            mouseEventListener.MouseDown += MouseEventListenerOnMouseDown;
            BeforeDestroy += _ => mouseEventListener.Detach();

            CreateCharacter();
            AnimateHead();
            AnimateHands();
            AnimateBody();

            base.CreateInterface(@params);
        }

        protected override void OnFixedTick()
        {
            if ((DateTime.Now - _lastClick) > TimeSpan.FromSeconds(1f))
            {
                _headRotateAnimation.Duration = mMath.Clamp(_headRotateAnimation.Duration * 1.01f, 0f, 3f);
                _headScaleAnimation.Duration = mMath.Clamp(_headScaleAnimation.Duration * 1.01f, 0f, 5f);

                _leftHandMoveAnimation.Duration = mMath.Clamp(_leftHandMoveAnimation.Duration * 1.01f, 0f, 5f);
                _leftHandRotationAnimation.Duration = mMath.Clamp(_leftHandRotationAnimation.Duration * 1.01f, 0f, 6f);

                _rightHandMoveAnimation.Duration = mMath.Clamp(_rightHandMoveAnimation.Duration * 1.01f, 0f, 5f);
                _rightHandRotationAnimation.Duration = mMath.Clamp(_rightHandRotationAnimation.Duration * 1.01f, 0f, 6f);
            }

            base.OnFixedTick();
        }

        private void MouseEventListenerOnMouseDown(object s, MouseEvent e)
        {
            _headRotateAnimation.Duration = mMath.Clamp(_headRotateAnimation.Duration * 0.9f, 0.1f, 3f);
            _headScaleAnimation.Duration = mMath.Clamp(_headScaleAnimation.Duration * 0.9f, 0.1f, 5f);

            _leftHandMoveAnimation.Duration = mMath.Clamp(_leftHandMoveAnimation.Duration * 0.9f, 0.1f, 5f);
            _leftHandRotationAnimation.Duration = mMath.Clamp(_leftHandRotationAnimation.Duration * 0.9f, 0.1f, 6f);

            _rightHandMoveAnimation.Duration = mMath.Clamp(_rightHandMoveAnimation.Duration * 0.9f, 0.1f, 5f);
            _rightHandRotationAnimation.Duration = mMath.Clamp(_rightHandRotationAnimation.Duration * 0.9f, 0.1f, 6f);

            _lastClick = DateTime.Now;
        }

        private void AnimateBody()
        {
            _body.LinearAnimation(new UILinearAnimationSettings
            {
                StartPos = _body.TranslatedY(+0.1f),
                EndPos = _body.TranslatedY(-0.05f),
                EasingType = EasingType.easeInOutBack,
                Duration = 5f,
                PlayType = UIAnimationPlayType.END_FLIP
            });
        }

        private void AnimateHands()
        {
            _rightHandRotationAnimation = _rightHand.RotateAnimation(new UIRotateAnimationSettings
            {
                FromAngle = 0f,
                ToAngle = 45f,
                Duration = 6f,
                EasingType = EasingType.easeInOutBack,
                PlayType = UIAnimationPlayType.END_FLIP
            });

            _rightHandMoveAnimation = _rightHand.BezierCubicAnimation(new UIBezierCubicAnimationSettings
            {
                Duration = 5f,
                FirstPoint = _rightHand.Pos(),
                SecondPoint = _rightHand.Translated(0.2f, 0.3f),
                ThirdPoint = _rightHand.Translated(0.2f, -0.3f),
                FourthPoint = _rightHand.Pos(),
                PlayType = UIAnimationPlayType.END_RESET
            });

            _leftHandRotationAnimation = _leftHand.RotateAnimation(new UIRotateAnimationSettings
            {
                FromAngle = 0f,
                ToAngle = -45f,
                Duration = 6f,
                EasingType = EasingType.easeInOutBack,
                PlayType = UIAnimationPlayType.END_FLIP
            });

            _leftHandMoveAnimation = _leftHand.BezierCubicAnimation(new UIBezierCubicAnimationSettings
            {
                Duration = 5f,
                FirstPoint = _leftHand.Pos(),
                SecondPoint = _leftHand.Translated(-0.2f, 0.3f),
                ThirdPoint = _leftHand.Translated(-0.2f, -0.3f),
                FourthPoint = _leftHand.Pos(),
                PlayType = UIAnimationPlayType.END_RESET
            });
        }

        private void AnimateHead()
        {
            _headRotateAnimation = _head.RotateAnimation(new UIRotateAnimationSettings
            {
                FromAngle = 0,
                ToAngle = 10f,
                EasingType = EasingType.easeInOutBack,
                Duration = 3f,
                PlayType = UIAnimationPlayType.END_FLIP,
            });

            _headScaleAnimation = _head.ScaleAnimation(new UIScaleAnimationSettings
            {
                StartScale = _head.Scale(),
                EndScale = _head.Scale() * 1.1f,
                EasingType = EasingType.easeInQuint,
                Duration = 5f,
                PlayType = UIAnimationPlayType.END_FLIP,
            });
        }

        private void CreateCharacter()
        {
            _characterContainer = this.Container(new UIContainerSettings());

            _body = _characterContainer.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("mp_bar_buttons")});
            _body.SetColor(Color.black);

            _head = _characterContainer.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("notif_cancel")});
            _head.SortingOrder(1);
            _head.Scale(3f);
            _head.Translate(0f, 2.22f);

            _leftHand = _characterContainer.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("test_hand")});
            _leftHand.SortingOrder(2);
            _leftHand.Scale(0.2f, 0.32f);
            _leftHand.Translate(-0.87f, 0.36f);
            _leftHand.Flip(true, false);
            _leftHand.name = "Left Hand";
            _leftHand.SetColor(Color.black);

            _rightHand = _characterContainer.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("test_hand")});
            _rightHand.SortingOrder(2);
            _rightHand.Scale(0.2f, 0.32f);
            _rightHand.Translate(0.87f, 0.36f);
            _rightHand.name = "Right Hand";
            _rightHand.SetColor(Color.black);

            _leftLeg = _characterContainer.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("test_hand")});
            _leftLeg.SortingOrder(2);
            _leftLeg.Scale(0.2f, 0.32f);
            _leftLeg.Translate(-0.64f, -2.14f);
            _leftLeg.Rotate(-113.808f);
            _leftLeg.SetColor(Color.black);

            _rightLeg = _characterContainer.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("test_hand")});
            _rightLeg.SortingOrder(2);
            _rightLeg.Scale(0.2f, 0.32f);
            _rightLeg.Translate(0.64f, -2.14f);
            _rightLeg.Rotate(113.808f);
            _rightLeg.SetColor(Color.black);
            _rightLeg.Flip(true, false);

            _characterContainer.Scale(0.5f);
        }
    }
}