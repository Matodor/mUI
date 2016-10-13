using System;
using UnityEngine;

namespace mUIApp
{
    public class mUICamera
    {
        private readonly Camera _camera;

        public mUICamera()
        {
            _camera = mUI.ViewsGameObject.AddComponent<Camera>();
            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.depth = 0;
            _camera.orthographic = true;
            _camera.farClipPlane = 1;
            _camera.nearClipPlane = -1;
            _camera.orthographicSize = 5;
        }

        public void SetOrthographicSize(float size)
        {
            _camera.orthographicSize = size;
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPos)
        {
            if (Application.isEditor)
                screenPos.y = Screen.height - screenPos.y;
            return _camera.ScreenToWorldPoint(screenPos);
        }
    }
}
