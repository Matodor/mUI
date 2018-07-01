using UnityEngine;

namespace mFramework.UI
{
    public interface IUIColored : IUIObject
    {
        Color Color { get; set; }

        /// <summary>
        /// Normalized opacity
        /// </summary>
        float Opacity { get; set; }
    }
}