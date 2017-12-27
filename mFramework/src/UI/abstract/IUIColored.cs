using UnityEngine;

namespace mFramework.UI
{
    public interface IUIColored : IUIRenderer
    {
        Color GetColor();
        IUIColored SetColor(Color32 color);
        IUIColored SetColor(UIColor color);
    }
}