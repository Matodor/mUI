using mUIApp.Input;
using UnityEngine;

namespace mUIApp
{
    public sealed class mUIEngine : MonoBehaviour
    {
        public IInputBase UIInput { get; set; }

        public void Awake()
        {
            UIInput = new mGUIInputDefault();
        }

        private void OnGUI()
        {
            UIInput.OnGUI();
        }

        private void Update()
        {
            UIInput.Update();
        }

        public void FixedUpdate()
        {
            
        }

        public void LateUpdate()
        {
            
        }
    }
}
