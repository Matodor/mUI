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
    }

    public sealed class UICamera
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Camera Camera { get; }

        public Vector2 Position => Transform.position;

        public float PureHeight => Camera.orthographicSize * 2;
        public float PureWidth => Camera.orthographicSize * Camera.aspect * 2;
        public float Height => PureHeight * Transform.lossyScale.y;
        public float Width => PureWidth * Transform.lossyScale.x;

        public float Left => Transform.position.x - Width / 2;
        public float Right => Transform.position.x + Width / 2;
        public float Top => Transform.position.y + Height / 2;
        public float Bottom => Transform.position.y - Height / 2;

        private UICamera(UICameraSettings settings)
        {
            Camera = new GameObject("UICamera").AddComponent<Camera>();
            Camera.clearFlags = settings.CameraClearFlags;
            Camera.depth = settings.Depth;
            Camera.orthographic = settings.Orthographic;
            Camera.farClipPlane = settings.FarClipPlane;
            Camera.nearClipPlane = settings.NearClipPlane;
            Camera.orthographicSize = settings.OrthographicSize;

            Transform = Camera.transform; 
            GameObject = Camera.gameObject;
        }

        public static UICamera Create(UICameraSettings settings)
        {
            return new UICamera(settings);
        }

        public UICamera SetOrthographicSize(float size)
        {
            Camera.orthographicSize = size;
            return this;
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return Camera.WorldToScreenPoint(worldPos);
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPos)
        {
            if (mCore.IsEditor)
                screenPos.y = Screen.height - screenPos.y;
            return Camera.ScreenToWorldPoint(screenPos);
        }
    }
}
