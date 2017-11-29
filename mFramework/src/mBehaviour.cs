using UnityEngine;

namespace mFramework
{
    internal sealed class mBehaviour : MonoBehaviour
    {
        private mBehaviour()
        {
            
        }

        public void Awake()
        {
        }

        private void OnApplicationPause(bool pauseState)
        {
            mCore.OnApplicationPause(pauseState);
        }

        private void OnApplicationQuit()
        {
            mCore.OnApplicationQuit();
        }

        private void Update()
        {
            EventsController.Update();
            mCore.Tick();
        }

        private void FixedUpdate()
        {
            mCore.FixedTick();
        }

        private void LateUpdate()
        {
            mCore.LateTick();
        }
    }
}
