using UnityEngine;

namespace mFramework.UI
{
    public struct UIRect
    {
        public readonly Vector2 Top;
        public readonly Vector2 Bottom;
        public readonly Vector2 Left;
        public readonly Vector2 Right;

        public readonly Vector2 TopLeft;
        public readonly Vector2 TopRight;
        public readonly Vector2 BottomLeft;
        public readonly Vector2 BottomRight;

        public readonly Vector2 Center;
        public readonly Vector2 Anchor;

        public UIRect(
            Vector2 topLeft, Vector2 top, Vector2 topRight,
            Vector2 left, Vector2 center, Vector2 right,
            Vector2 bottomLeft, Vector2 bottom, Vector2 bottomRight, Vector2 anchor)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Anchor = anchor;
            Center = center;
        }
    }
}
