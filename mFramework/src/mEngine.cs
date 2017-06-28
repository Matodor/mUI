using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace mFramework
{
    public sealed class mEngine : MonoBehaviour
    {
        public static mEngine Instance { get; private set; }

        private mEngine()
        {
            Instance = this;
        }

        public void Awake()
        {
        }

        private void OnGUI()
        {
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
