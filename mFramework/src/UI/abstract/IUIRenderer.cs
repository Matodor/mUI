using UnityEngine;

namespace mFramework.UI
{
    public interface IUIRenderer : IUIObject
    {
        Renderer UIRenderer { get; }
    }
}