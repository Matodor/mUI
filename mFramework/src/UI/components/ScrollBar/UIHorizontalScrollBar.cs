using UnityEngine;

namespace mFramework.UI
{
    public class UIHorizontalScrollBar : UIBaseScrollBar
    {
        protected override void ApplySettings(UIComponentSettings settings)
        {
            base.ApplySettings(settings);

            if (!BarSpriteIsHorizontal)
            {
                Bar.Rotate(90f);
            }
        }

        public override float GetHeight()
        {
            return !BarSpriteIsHorizontal
                ? Bar.GetWidth() - Padding.y * GlobalScale().y
                : Bar.GetHeight() - Padding.y * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return !BarSpriteIsHorizontal
                ? Bar.GetHeight() - Padding.x * GlobalScale().x
                : Bar.GetWidth() - Padding.x * GlobalScale().x;
        }

        protected override void MovePoint(Vector2 newPos)
        {
            var left = Pos().x - GetWidth() / 2;
            var right = Pos().x + GetWidth() / 2;
            var x = mMath.Clamp(newPos.x, left, right);
            NormilizedValue = NormilizeValue(left, right, x);
        }

        protected override void UpdateBar(float normilized)
        {
            var left = Pos().x - GetWidth() / 2;
            var right = Pos().x + GetWidth() / 2;
            var x = BezierHelper.Linear(NormilizedValue, left, right);
            BarPoint.PosX(x);
        }
    }
}