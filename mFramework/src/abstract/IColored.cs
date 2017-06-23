using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public interface IColored
    {
        UIObject SetColor(Color32 color);
        UIObject SetColor(UIColor color);
    }
}
