using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        
        private void Update()
        {
            mCore.Instance.Tick();
        }

        public void FixedUpdate()
        {
            mCore.Instance.FixedTick();
        }

        public void LateUpdate()
        {
            mCore.Instance.LateTick();
        }
    }
}
