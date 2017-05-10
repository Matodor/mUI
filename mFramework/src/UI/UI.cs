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
        public static UIView BaseView { get; private set; }

        public static UICamera UICamera { get; private set; }

        private UI(UISettings settings)
        {
            UICamera = UICamera.Create(settings.CameraSettings);
            UICamera.GameObject.SetParent(mEngine.Instance.gameObject);

            Instance = this;
            BaseView = UIView.Create<BaseView>(new UIViewSettings
            {
                Height = UICamera.PureHeight,
                Width = UICamera.PureWidth,
                Name = "BaseView"
            }, null);
            
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
