using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIJoystickSettings : UIComponentSettings
    {
        public Sprite BackgroundSprite;
        public Sprite PointSprite;
        public float PaddingRadius;
        public float Friction = 0f;
    }

    public class UIJoystick : UIComponent, IUIClickable
    {
        public event UIEventHandler<UIJoystick> PointMoved = delegate { };

        public UIClickable UIClickable { get; private set; }
        public UISprite Background { get; private set; }
        public UISprite PointSprite { get; private set; }

        public Vector2 NormilizedPointPos { get; private set; }
        public float PaddingRadius;
        public float Friction;

        private bool _isPressed;

        protected override void Init()
        {
            NormilizedPointPos = Vector2.zero;
            _isPressed = false;
            base.Init();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIJoystickSettings joystickSettings))
                throw new ArgumentException("UIJoystick: The given settings is not UIJoystickSettings");

            Friction = joystickSettings.Friction;
            PaddingRadius = joystickSettings.PaddingRadius;

            Background = this.Sprite(new UISpriteSettings {Sprite = joystickSettings.BackgroundSprite});
            PointSprite = Background.Sprite(new UISpriteSettings {Sprite = joystickSettings.PointSprite});
            PointSprite.SortingOrder(1);

            var area = new CircleArea2D();
            area.Update += a =>
            {
                area.Radius = GetWidth() / 2;
            };
            UIClickable = new UIClickable(this, area);

            base.ApplySettings(settings);
        }

        public override float UnscaledHeight()
        {
            return Background.UnscaledHeight();
        }

        public override float UnscaledWidth()
        {
            return Background.UnscaledWidth();
        }

        public override float GetWidth()
        {
            return Background.GetWidth();
        }

        public override float GetHeight()
        {
            return Background.GetHeight();
        }

        private void UpdatePos(Vector2 worldPos)
        {
            var scale = Background.GlobalScale();

            var widthDiv2 = (GetWidth() - PaddingRadius * scale.x * 2f) / 2f;
            var heightDiv2 = (GetHeight() - PaddingRadius * scale.y * 2f) / 2f;
            var pos = Background.Pos();

            var left = pos.x - widthDiv2;
            var right = pos.x + widthDiv2;
            var bottom = pos.y - heightDiv2;
            var top = pos.y + heightDiv2;

            var diff = worldPos - pos;
            var angle = mMath.Angle(diff);
            var length = mMath.Clamp(diff.Length() * (1f - Friction), 0f, UnscaledWidth() / 2 - PaddingRadius);
            var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            var sin = Mathf.Sin(angle * Mathf.Deg2Rad);

            var newPos = new Vector2(
                cos * length * scale.x, 
                sin * length * scale.y
            );

            PointSprite.LocalPos(newPos);
            NormilizedPointPos = new Vector2(
                mMath.NormilizeValue(left, right, pos.x + newPos.x) - 0.5f,
                mMath.NormilizeValue(bottom, top, pos.y + newPos.y) - 0.5f
            );

            PointMoved(this);
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            UpdatePos(worldPos);
        }

        public void MouseUp(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            _isPressed = false;
            PointSprite.LocalPos(0f, 0f);
        }

        public void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            UpdatePos(worldPos);
        }
    }
}