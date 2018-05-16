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
                mMath.TriangleContainsPoint(rect.TopLeft, rect.BottomLeft, rect.BottomRight, worldPos) ||
                mMath.TriangleContainsPoint(rect.BottomRight, rect.TopRight, rect.TopLeft, worldPos);
        }
    }
}