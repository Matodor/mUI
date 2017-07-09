using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    internal class UICameraBehaviour : MonoBehaviour
    {
        public Camera Camera;
        public event Action OnPostRenderEvent;

        private UICameraBehaviour()
        {
            
        }

        public void OnPostRender()
        {
            OnPostRenderEvent?.Invoke();
        }

        public static UICameraBehaviour Create()
        {
            var go = new GameObject("UICamera");
            var cameraBehaviour = go.AddComponent<UICameraBehaviour>();
            cameraBehaviour.Camera = go.AddComponent<Camera>();
            return cameraBehaviour;
        }
    }

    public sealed class UICamera
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Camera Camera { get; }

        public Vector2 Position { get { return Transform.position; } }

        public float PureHeight { get { return _cameraBehaviour.Camera.orthographicSize * 2; } }
        public float PureWidth { get { return _cameraBehaviour.Camera.orthographicSize * _cameraBehaviour.Camera.aspect * 2; } }
        public float Height { get { return PureHeight * Transform.lossyScale.y; } }
        public float Width { get { return PureWidth * Transform.lossyScale.x; } }

        public float Left { get { return Transform.position.x - Width / 2; } }
        public float Right { get { return Transform.position.x + Width / 2; } }
        public float Top { get { return Transform.position.y + Height / 2; } }
        public float Bottom { get { return Transform.position.y - Height / 2; } }

        private readonly UICameraBehaviour _cameraBehaviour;

        private UICamera(UICameraSettings settings)
        {
            _cameraBehaviour = UICameraBehaviour.Create();
            Camera = _cameraBehaviour.Camera;
            Camera.clearFlags = settings.CameraClearFlags;
            Camera.depth = settings.Depth;
            Camera.orthographic = settings.Orthographic;
            Camera.farClipPlane = settings.FarClipPlane;
            Camera.nearClipPlane = settings.NearClipPlane;
            Camera.orthographicSize = settings.OrthographicSize;

            Transform = _cameraBehaviour.transform; 
            GameObject = _cameraBehaviour.gameObject;
        }

        public static UICamera Create(UICameraSettings settings)
        {
            return new UICamera(settings);
        }

        public UICamera SetOrthographicSize(float size)
        {
            _cameraBehaviour.Camera.orthographicSize = size;
            return this;
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPos)
        {
            if (mCore.IsEditor)
                screenPos.y = Screen.height - screenPos.y;
            return _cameraBehaviour.Camera.ScreenToWorldPoint(screenPos);
        }
    }
}
