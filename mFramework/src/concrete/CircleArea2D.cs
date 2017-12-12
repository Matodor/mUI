﻿using UnityEngine;

namespace mFramework
{
    public class CircleArea2D : Area2D
    {
        public float Radius;

        public override bool InArea(Vector2 worldPos)
        {
            UpdateData();

            var circleCenter = new Vector2(
                Center.x + Offset.x * AdditionalScale,
                Center.y + Offset.y * AdditionalScale
            );

            var r = Radius * AdditionalScale;

            if (mCore.IsEditor && mCore.IsDebug)
            {
                mCore.DrawDebugCircle(circleCenter, r);
            }

            return mMath.CircleContainsPoint(circleCenter, r, worldPos);
        }
    }
}