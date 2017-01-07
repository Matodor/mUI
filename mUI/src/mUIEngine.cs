using System;
using mUIApp.Input;
using UnityEngine;

namespace mUIApp
{
    public sealed class mUIEngine : MonoBehaviour
    {
        public event Action OnApplicationQuitEvent;

        public IInputBase UIInput { get; set; } = new mUIInputDefault();

        public void Awake()
        {
        }

        private void OnGUI()
        {
            UIInput.OnGUI();
        }

        private void Update()
        {
            UIInput.Update();
            mUI.Tick();
        }

        private void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
            mUI.Log("[mUI] OnApplicationQuit");
        }

        public void FixedUpdate()
        {
            mUI.FixedTick();
        } 

        public void LateUpdate()
        {
            mUI.LateTick(); 
        }
    }
}
