using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public sealed class UISettings
    {
        public UICameraSettings CameraSettings { get; } = new UICameraSettings();
    }

    public sealed class UI : ITicking
    {
        public static UI Instance { get; private set; }

        private readonly UICamera _uiCamera;

        private UI(UISettings settings)
        {
            Instance = this;

            _uiCamera = UICamera.Create(settings.CameraSettings);
            _uiCamera.SetParent(mEngine.Instance.gameObject);

            mCore.Log("[ui] created");
        }

        public static UI Create()
        {
            return Create(new UISettings());
        }

        public static UI Create(UISettings settings)
        {
            if (settings == null)
                throw new NullReferenceException("UISettings is null");
            if (Instance != null)
                throw new Exception("UI already created");
            return new UI(settings);
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void LateTick()
        {
        }
    }
}
