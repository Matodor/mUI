using UnityEngine;

namespace mFramework.UI
{
    public interface IAreaChecker
    {
        bool InAreaShape(IUIObject obj, Vector2 worldPos);
    }
}