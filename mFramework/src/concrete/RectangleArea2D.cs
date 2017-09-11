using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public class RectangleArea2D : Area2D
    {
        public float Height { get; set; }
        public float Width { get; set; }

        public override bool InArea(Vector2 worldPoint)
        {
            UpdateData();
            
            var cos = Mathf.Cos(Mathf.Deg2Rad * Rotation);
            var sin = Mathf.Sin(Mathf.Deg2Rad * Rotation);

            var widthDiv2 = Width / 2;
            var heightDiv2 = Height / 2;

            // left top
            var x = (-widthDiv2 + Offset.x) * AdditionalScale;
            var y = (heightDiv2 + Offset.y) * AdditionalScale;
            var leftTop = mMath.GetRotatedPoint(Center.x, Center.y, x, y, sin, cos);

            // left bottom
            x = (-widthDiv2 + Offset.x) * AdditionalScale;
            y = (-heightDiv2 + Offset.y) * AdditionalScale;
            var leftBottom = mMath.GetRotatedPoint(Center.x, Center.y, x, y, sin, cos);

            // right bottom
            x = (widthDiv2 + Offset.x) * AdditionalScale;
            y = (-heightDiv2 + Offset.y) * AdditionalScale;
            var rightBottom = mMath.GetRotatedPoint(Center.x, Center.y, x, y, sin, cos);

            // right top
            x = (widthDiv2 + Offset.x) * AdditionalScale;
            y = (heightDiv2 + Offset.y) * AdditionalScale;
            var rightTop = mMath.GetRotatedPoint(Center.x, Center.y, x, y, sin, cos);

            if (mCore.IsEditor && mCore.IsDebug)
            {
                Debug.DrawLine(leftTop, rightTop);
                Debug.DrawLine(rightTop, rightBottom);
                Debug.DrawLine(rightBottom, leftBottom);
                Debug.DrawLine(leftBottom, leftTop);
            }

            return
                mMath.TriangleContainsPoint(leftTop, leftBottom, rightBottom, worldPoint) ||
                mMath.TriangleContainsPoint(rightBottom, rightTop, leftTop, worldPoint);
        }
    }
}
