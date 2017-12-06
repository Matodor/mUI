using UnityEngine;

namespace mFramework.UI
{
    public interface IUIColored : IUIObject
    {
        Color GetColor();
        IUIColored SetColor(Color32 color);
        IUIColored SetColor(UIColor color);
    }
}