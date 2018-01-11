using UnityEngine;

namespace mFramework.UI
{
    public interface IUIColored : IUIRenderer
    {
        Color GetColor();
        float GetOpacity();

        IUIColored SetColor(Color32 color);
        IUIColored SetColor(UIColor color);
        IUIColored SetOpacity(float opacity);
    }
}