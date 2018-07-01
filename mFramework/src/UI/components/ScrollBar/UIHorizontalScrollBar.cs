using UnityEngine;
/*
namespace mFramework.UI
{
    public class UIHorizontalScrollBar : UIBaseScrollBar
    {
        protected override void ApplySettings(UIComponentSettings settings)
        {
            base.ApplySettings(settings);

            if (!BarSpriteIsHorizontal)
            {
                Bar.Rotation = 90f;
            }
        }

        public override float UnscaledHeight()
        {
            return !BarSpriteIsHorizontal
                ? Bar.UnscaledWidth()
                : Bar.UnscaledHeight();
        }

        public override float UnscaledWidth()
        {
            return !BarSpriteIsHorizontal
                ? Bar.UnscaledHeight()
                : Bar.UnscaledWidth();
        }

        public override float GetHeight()
        {
            return !BarSpriteIsHorizontal
                ? Bar.GetWidth()
                : Bar.GetHeight();
        }

        public override float GetWidth()
        {
            return !BarSpriteIsHorizontal
                ? Bar.GetHeight()
                : Bar.GetWidth();
        }

        protected override void MovePoint(Vector2 newPos)
        {
            var widthDiv2 = (GetWidth() - Padding.x * GlobalScale().x * 2f) / 2f;
            var left = Pos().x - widthDiv2;
            var right = Pos().x + widthDiv2;
            var x = mMath.Clamp(newPos.x, left, right);
            NormilizedValue = mMath.NormilizeValue(left, right, x);
        }

        protected override void UpdateBar(float normilized)
        {
            var widthDiv2 = (GetWidth() - Padding.x * GlobalScale().x * 2f) / 2f;
            var x = BezierHelper.Linear(NormilizedValue, Pos().x - widthDiv2, Pos().x + widthDiv2);
            BarPoint.PosX(x);
        }
    }
}*/