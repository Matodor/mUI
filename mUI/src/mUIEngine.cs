using mUIApp.Input;
using UnityEngine;

namespace mUIApp
{
    public sealed class mUIEngine : MonoBehaviour
    {
        public IInputBase UIInput { get; set; }

        public void Awake()
        {
            UIInput = new mUIInputDefault();
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
