using UnityEngine;

namespace mFramework.UI
{
    public class UIPos
    {
        /// <summary>
        /// Always return world X coordinate 
        /// </summary>
        public virtual float X { get; }

        /// <summary>
        /// Always return world Y coordinate 
        /// </summary>
        public virtual float Y { get; }

        public UIPos(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }
    }

    public class UIRelativePos : UIPos
    {
        public override float X => BezierHelper.Linear(NormilizedX,
            -mUI.UICamera.Width / 2,
            +mUI.UICamera.Width / 2);

        public override float Y => BezierHelper.Linear(NormilizedX,
            -mUI.UICamera.Height / 2,
            +mUI.UICamera.Height / 2);

        public readonly float NormilizedX;
        public readonly float NormilizedY;

        /// <summary>
        /// Relative pos on screen
        /// </summary>
        /// <param name="normilizedX">[0..1] 0 - left, 1 - right</param>
        /// <param name="normilizedY">[0..1] 0 - bottom, 1 - top</param>
        public UIRelativePos(float normilizedX, float normilizedY)
        {
            NormilizedX = normilizedX;
            NormilizedY = normilizedY;
        }
    }
}