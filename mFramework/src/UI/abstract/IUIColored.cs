using UnityEngine;

namespace mFramework.UI
{
    public interface IUIColored : IUIObject
    {
        Color GetColor();
        void SetColor(Color32 color);
        void SetColor(UIColor color);
    }
}