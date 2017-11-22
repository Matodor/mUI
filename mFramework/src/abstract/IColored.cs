using UnityEngine;

namespace mFramework.UI
{
    public interface IColored
    {
        Color GetColor();
        UIObject SetColor(Color32 color);
        UIObject SetColor(UIColor color);
    }
}
