using UnityEngine;

namespace mFramework.UI
{
    public class UIUtils
    {
        public static void RotateRect(ref UIRect rect, float rotation)
        {
            var cos = Mathf.Cos(Mathf.Deg2Rad * rotation);
            var sin = Mathf.Sin(Mathf.Deg2Rad * rotation);

            rect.UpperLeft = new Vector2(
                rect.MiddleCenter.x + rect.UpperLeft.x * cos - rect.UpperLeft.y * sin, 
                rect.MiddleCenter.y + rect.UpperLeft.x * sin + rect.UpperLeft.y * cos);

            rect.UpperCenter = new Vector2(
                rect.MiddleCenter.x + rect.UpperCenter.x * cos - rect.UpperCenter.y * sin,
                rect.MiddleCenter.y + rect.UpperCenter.x * sin + rect.UpperCenter.y * cos);

            rect.UpperRight = new Vector2(
                rect.MiddleCenter.x + rect.UpperRight.x * cos - rect.UpperRight.y * sin,
                rect.MiddleCenter.y + rect.UpperRight.x * sin + rect.UpperRight.y * cos);

            rect.MiddleLeft = new Vector2(
                rect.MiddleCenter.x + rect.MiddleLeft.x * cos - rect.MiddleLeft.y * sin,
                rect.MiddleCenter.y + rect.MiddleLeft.x * sin + rect.MiddleLeft.y * cos);

            rect.MiddleRight = new Vector2(
                rect.MiddleCenter.x + rect.MiddleRight.x * cos - rect.MiddleRight.y * sin,
                rect.MiddleCenter.y + rect.MiddleRight.x * sin + rect.MiddleRight.y * cos);

            rect.LowerLeft = new Vector2(
                rect.MiddleCenter.x + rect.LowerLeft.x * cos - rect.LowerLeft.y * sin,
                rect.MiddleCenter.y + rect.LowerLeft.x * sin + rect.LowerLeft.y * cos);

            rect.LowerCenter = new Vector2(
                rect.MiddleCenter.x + rect.LowerCenter.x * cos - rect.LowerCenter.y * sin,
                rect.MiddleCenter.y + rect.LowerCenter.x * sin + rect.LowerCenter.y * cos);

            rect.LowerRight = new Vector2(
                rect.MiddleCenter.x + rect.LowerRight.x * cos - rect.LowerRight.y * sin,
                rect.MiddleCenter.y + rect.LowerRight.x * sin + rect.LowerRight.y * cos);
        }
    }
}