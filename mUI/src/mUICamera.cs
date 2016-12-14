using System;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp
{
    public class mUICamera
    {
        public Vector2 Position { get { return Transform.position; } }
        public Transform Transform { get; }
        public Camera Camera { get { return _camera; } }

        public float PureHeight { get { return _camera.orthographicSize*2; } }
        public float PureWidth { get { return _camera.orthographicSize*_camera.aspect*2; } }
        public float Height { get { return PureHeight*Transform.lossyScale.y; } }
        public float Width { get { return PureWidth*Transform.lossyScale.x; } }

        public float Left { get { return Transform.position.x - Width/2; } }
        public float Right { get { return Transform.position.x + Width/2; } }
        public float Top { get { return Transform.position.y + Height/2; } }
        public float Bottom { get { return Transform.position.y - Height/2; } }


        private readonly Camera _camera;

        public mUICamera(GameObject attachObj)
        {
            _camera = attachObj.AddComponent<Camera>();
            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.depth = 0;
            _camera.orthographic = true;
            _camera.farClipPlane = 0.01f;
            _camera.nearClipPlane = -0.01f;
            _camera.orthographicSize = 5;

            Transform = _camera.transform;
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
