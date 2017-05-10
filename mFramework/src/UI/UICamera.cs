using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public sealed class UICameraSettings
    {
        public CameraClearFlags CameraClearFlags { get; set; } = CameraClearFlags.Depth;
        public float Depth { get; set; } = 0;
        public bool Orthographic { get; set; } = true;
        public float FarClipPlane { get; set; } = 0.01f;
        public float NearClipPlane { get; set; } = -0.01f;
        public float OrthographicSize { get; set; } = 5;
    }

    public sealed class UICamera : IGameObject
    {
        public GameObject GameObject { get { return _cameraGameObject; } }

        public Vector2 Position { get { return Transform.position; } }
        public Transform Transform { get; }
        public Camera Camera { get { return _camera; } }

        public float PureHeight { get { return _camera.orthographicSize * 2; } }
        public float PureWidth { get { return _camera.orthographicSize * _camera.aspect * 2; } }
        public float Height { get { return PureHeight * Transform.lossyScale.y; } }
        public float Width { get { return PureWidth * Transform.lossyScale.x; } }

        public float Left { get { return Transform.position.x - Width / 2; } }
        public float Right { get { return Transform.position.x + Width / 2; } }
        public float Top { get { return Transform.position.y + Height / 2; } }
        public float Bottom { get { return Transform.position.y - Height / 2; } }
        
        private readonly Camera _camera;
        private readonly GameObject _cameraGameObject;

        private UICamera(UICameraSettings settings)
        {
            _cameraGameObject = new GameObject("UICamera");
            _camera = _cameraGameObject.AddComponent<Camera>();
            _camera.clearFlags = settings.CameraClearFlags;
            _camera.depth = settings.Depth;
            _camera.orthographic = settings.Orthographic;
            _camera.farClipPlane = settings.FarClipPlane;
            _camera.nearClipPlane = settings.NearClipPlane;
            _camera.orthographicSize = settings.OrthographicSize;

            Transform = _camera.transform;
        }

        public static UICamera Create(UICameraSettings settings)
        {
            return new UICamera(settings);
        }

        public UICamera SetOrthographicSize(float size)
        {
            _camera.orthographicSize = size;
            return this;
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPos)
        {
            if (Application.isEditor)
                screenPos.y = Screen.height - screenPos.y;
            return _camera.ScreenToWorldPoint(screenPos);
        }
    }
}
