using UnityEngine;
/*
namespace mFramework.UI
{
    public class UIVerticalScrollBar : UIBaseScrollBar
    {
        protected override void ApplySettings(UIComponentSettings settings)
        {
            base.ApplySettings(settings);

            if (BarSpriteIsHorizontal)
            {
                Bar.Rotate(90f);
            }
        }

        public override float UnscaledHeight()
        {
            return BarSpriteIsHorizontal
                ? Bar.UnscaledWidth()
                : Bar.UnscaledHeight();
        }

        public override float UnscaledWidth()
        {
            return BarSpriteIsHorizontal
                ? Bar.UnscaledHeight()
                : Bar.UnscaledWidth();
        }

        public override float GetHeight()
        {
            return BarSpriteIsHorizontal
                ? Bar.GetWidth()
                : Bar.GetHeight();
        }

        public override float GetWidth()
        {
            return BarSpriteIsHorizontal
                ? Bar.GetHeight()
                : Bar.GetWidth();
        }

        protected override void MovePoint(Vector2 newPos)
        {
            var heightDiv2 = (GetHeight() - Padding.y * GlobalScale().y * 2f) / 2f;
            var top = Pos().y + heightDiv2;
            var bottom = Pos().y - heightDiv2;
            var y = mMath.Clamp(newPos.y, bottom, top);
            NormilizedValue = mMath.NormilizeValue(bottom, top, y);
        }

        protected override void UpdateBar(float normilized)
        {
            var heightDiv2 = (GetHeight() - Padding.y * GlobalScale().y * 2f) / 2f;
            var y = BezierHelper.Linear(NormilizedValue, Pos().y - heightDiv2, Pos().y + heightDiv2);
            BarPoint.PosY(y);
        }
    }
}*/