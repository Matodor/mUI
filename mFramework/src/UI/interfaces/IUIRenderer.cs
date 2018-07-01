using UnityEngine;

namespace mFramework.UI
{
    public interface IUIRenderer : IUIObject
    {
        Renderer UIRenderer { get; }
    }

    public interface IUIRenderer<out T> : IUIObject where T : Renderer
    {
        T UIRenderer { get; }
    }
}