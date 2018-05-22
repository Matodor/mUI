using UnityEngine;

namespace mFramework.UI
{
    public sealed class UICameraSettings
    {
        public CameraClearFlags CameraClearFlags { get; set; } = CameraClearFlags.Depth;
        public float Depth { get; set; } = 0;
        public bool Orthographic { get; set; } = true;
        public float FarClipPlane { get; set; } = 0.01f;
        public float NearClipPlane { get; set; } = -0.01f;
        public float OrthographicSize { get; set; } = 5;
        public Color BackgroundColor { get; set; } = Color.gray;
    }

    public sealed class UICamera : IDimensions
    {
        internal Transform Transform => _camera.transform;

        public float Height => UnscaledHeight * _camera.transform.lossyScale.y;
        public float Width => UnscaledWidth * _camera.transform.lossyScale.x;
        public float UnscaledWidth => _camera.orthographicSize * _camera.aspect * 2;
        public float UnscaledHeight => _camera.orthographicSize * 2;

        public Color BackgroundColor
        {
            get => _camera.backgroundColor;
            set => _camera.backgroundColor = value;
        }

        private readonly Camera _camera;

        private UICamera(UICameraSettings settings)
        {
            _camera = new GameObject("UICamera").AddComponent<Camera>();
            _camera.clearFlags = settings.CameraClearFlags;
            _camera.depth = settings.Depth;
            _camera.orthographic = settings.Orthographic;
            _camera.farClipPlane = settings.FarClipPlane;
            _camera.nearClipPlane = settings.NearClipPlane;
            _camera.orthographicSize = settings.OrthographicSize;
            _camera.backgroundColor = settings.BackgroundColor;
        }

        public static UICamera Create(UICameraSettings settings)
        {
            return new UICamera(settings);
        }

        public UICamera OrthographicSize(float orthographicSize)
        {
            _camera.orthographicSize = orthographicSize;
            return this;
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return _camera.WorldToScreenPoint(worldPos);
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPos)
        {
            if (mCore.IsEditor)
                screenPos.y = Screen.height - screenPos.y;
            return _camera.ScreenToWorldPoint(screenPos);
        }
    }
}
