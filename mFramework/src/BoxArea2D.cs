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
            base.InArea(worldPoint);

            var cos = Mathf.Cos(Mathf.Deg2Rad * Rotation);
            var sin = Mathf.Sin(Mathf.Deg2Rad * Rotation);

            var widthDiv2 = Width / 2;
            var heightDiv2 = Height / 2;

            // left top
            var x = (-widthDiv2 + Offset.x) * Scale.x;
            var y = (heightDiv2 + Offset.y) * Scale.y;
            var leftTop = mMath.GetRotatedPoint(Center.x, Center.y, ref x, ref y, ref sin, ref cos);

            // left bottom
            x = (-widthDiv2 + Offset.x) * Scale.x;
            y = (-heightDiv2 + Offset.y) * Scale.y;
            var leftBottom = mMath.GetRotatedPoint(Center.x, Center.y, ref x, ref y, ref sin, ref cos);

            // right bottom
            x = (widthDiv2 + Offset.x) * Scale.x;
            y = (-heightDiv2 + Offset.y) * Scale.y;
            var rightBottom = mMath.GetRotatedPoint(Center.x, Center.y, ref x, ref y, ref sin, ref cos);

            // right top
            x = (widthDiv2 + Offset.x) * Scale.x;
            y = (heightDiv2 + Offset.y) * Scale.y;
            var rightTop = mMath.GetRotatedPoint(Center.x, Center.y, ref x, ref y, ref sin, ref cos);


            return
                mMath.TriangleContainsPoint(leftTop, leftBottom, rightBottom, worldPoint) ||
                mMath.TriangleContainsPoint(rightBottom, rightTop, leftTop, worldPoint);
        }
    }
}
