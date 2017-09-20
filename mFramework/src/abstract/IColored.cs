using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public interface IColored
    {
        Color GetColor();
        UIObject SetColor(Color32 color);
        UIObject SetColor(UIColor color);
    }
}
