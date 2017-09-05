using System;
using UnityEngine;

namespace mFramework
{
    public sealed class mEngine : MonoBehaviour
    {
        public static mEngine Instance
        {
            get
            {
                if (_instance == null)
                    mCore.Instance.Init();
                return _instance;
            }
        }

        private static mEngine _instance;

        public void Awake()
        {
            _instance = this;
        }

        private void OnApplicationQuit()
        {
            mCore.OnApplicationQuit();
        }

        private void Update()
        {
            EventsController.Instance.Update();
            mCore.Instance.Tick();
        }

        private void FixedUpdate()
        {
            mCore.Instance.FixedTick();
        }

        private void LateUpdate()
        {
            mCore.Instance.LateTick();
        }
    }
}
