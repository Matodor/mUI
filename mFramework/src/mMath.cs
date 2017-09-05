using UnityEngine;

namespace mFramework
{
    public static class mMath
    {
        public static bool CheckBoxCircleCollide(Vector2 circlePos, float circleRadius, Vector2 boxPos, Vector2 boxSize)
        {
            Vector2 circleDistance = new Vector2(
                circleDistance.x = Mathf.Abs(circlePos.x - boxPos.x),
                circleDistance.y = Mathf.Abs(circlePos.y - boxPos.y)
            );

            if (circleDistance.x > (boxSize.x / 2 + circleRadius))
                return false;

            if (circleDistance.y > (boxSize.y / 2 + circleRadius))
                return false;

            if (circleDistance.x <= (boxSize.x / 2)) { return true; }
            if (circleDistance.y <= (boxSize.y / 2)) { return true; }

            var cornerDistance_sq = (circleDistance.x - boxSize.x / 2) * (circleDistance.x - boxSize.x / 2) +
                                    (circleDistance.y - boxSize.y / 2) * (circleDistance.y - boxSize.y / 2);

            return (cornerDistance_sq <= (circleRadius * circleRadius));
        }

        public static bool CheckCircleCollide(Vector2 c1Pos, float c1Radius, Vector2 c2Pos, float c2Radius)
        {
            return Mathf.Abs((c1Pos.x - c2Pos.x) * (c1Pos.x - c2Pos.x) + (c1Pos.y - c2Pos.y) * (c1Pos.y - c2Pos.y)) <
                   (c1Radius + c2Radius) * (c1Radius + c2Radius);
        }

        public static bool CheckBoxCollide(Vector2 box1Size, Vector2 box1Pos, Vector2 box2Size, Vector2 box2Pos)
        {
            return (Mathf.Abs(box1Pos.x - box2Pos.x) * 2 < (box1Size.x + box2Size.x)) &&
                   (Mathf.Abs(box1Pos.y - box2Pos.y) * 2 < (box1Size.y + box2Size.y));
        }

        // Точки в треугольнике против часовой стрелки
        // Points in the triangle are anti-clockwise
        public static bool TriangleContainsPoint(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            if ((P.x - A.x) * (A.y - B.y) - (P.y - A.y) * (A.x - B.x) >= 0)
                if ((P.x - B.x) * (B.y - C.y) - (P.y - B.y) * (B.x - C.x) >= 0)
                    if ((P.x - C.x) * (C.y - A.y) - (P.y - C.y) * (C.x - A.x) >= 0)
                        return true;
            return false;
        }

        public static int Clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static double Clamp(double val, double min, double max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static float Clamp(float val, float min, float max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static void GetRotatedPoints(float angle, Vector2 center, Vector2[] rotatedPoints)
        {
            var cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            var sin = Mathf.Sin(Mathf.Deg2Rad * angle);

            for (int i = 0; i < rotatedPoints.Length; i++)
            {
                rotatedPoints[i] = GetRotatedPoint(
                    center.x, center.y,
                    rotatedPoints[i].x, rotatedPoints[i].y,
                    sin, cos
                );
            }
        }

        public static Vector2 GetRotatedPoint(Vector2 center, Vector2 point, float angle)
        {
            var cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            var sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            return GetRotatedPoint(center.x, center.y, point.x, point.y, sin, cos);
        }

        public static Vector2 GetRotatedPoint(float centerX, float centerY, float x, float y, float sin, float cos)
        {
            return new Vector2(centerX + x * cos - y * sin, centerY + x * sin + y * cos);
        }
    }
}
