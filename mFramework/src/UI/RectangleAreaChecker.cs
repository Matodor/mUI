using UnityEngine;

namespace mFramework.UI
{
    public class RectangleAreaChecker : IAreaChecker
    {
        public static RectangleAreaChecker Default { get; } = new RectangleAreaChecker();

        public bool InAreaShape(IUIObject obj, Vector2 worldPos)
        {
            return InUIRect(obj.Rect, worldPos);
        }

        public static bool InUIRect(UIRect rect, Vector2 worldPos)
        {
            return
                mMath.TriangleContainsPoint(rect.UpperLeft, rect.LowerLeft, rect.LowerRight, worldPos) ||
                mMath.TriangleContainsPoint(rect.LowerRight, rect.UpperRight, rect.UpperLeft, worldPos);
        }
    }
}