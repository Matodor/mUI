using UnityEngine;

namespace mFramework.UI
{
    public interface IUIColored
    {
        Color GetColor();
        void SetColor(Color32 color);
        void SetColor(UIColor color);
    }
}