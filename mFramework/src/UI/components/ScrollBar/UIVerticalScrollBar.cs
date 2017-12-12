using UnityEngine;

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

        public override float GetHeight()
        {
            return BarSpriteIsHorizontal
                ? Bar.GetWidth() - Padding.y * GlobalScale().y
                : Bar.GetHeight() - Padding.y * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return BarSpriteIsHorizontal
                ? Bar.GetHeight() - Padding.x * GlobalScale().x
                : Bar.GetWidth() - Padding.x * GlobalScale().x;
        }

        protected override void MovePoint(Vector2 newPos)
        {
            var top = Pos().y + GetHeight() / 2;
            var bottom = Pos().y - GetHeight() / 2;
            var y = mMath.Clamp(newPos.y, bottom, top);
            NormilizedValue = NormilizeValue(bottom, top, y);

            mCore.Log($"MovePoint NormilizedValue = {NormilizedValue}");
        }

        protected override void UpdateBar(float normilized)
        {
            var top = Pos().y + GetHeight() / 2;
            var bottom = Pos().y - GetHeight() / 2;
            var y = BezierHelper.Linear(NormilizedValue, bottom, top);
            BarPoint.PosY(y);
        }
    }
}