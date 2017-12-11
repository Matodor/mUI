using UnityEngine;

namespace mFramework.UI
{
    public interface IMeshRenderer : IUIRenderer
    {
        MeshFilter MeshFilter { get; }
        MeshRenderer MeshRenderer { get; }

        IMeshRenderer SetSharedMesh(Mesh mesh);
        IMeshRenderer SetMesh(Mesh mesh);

        IMeshRenderer SetWidth(float width);
        IMeshRenderer SetHeight(float height);
    }
}