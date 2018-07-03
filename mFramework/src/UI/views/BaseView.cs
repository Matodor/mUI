using UnityEngine;

namespace mFramework.UI
{
    public sealed class BaseView : UIView
    {
        protected override void CreateInterface(params object[] @params)
        {
            Debug.Log("[ui] Create BaseView");
        }

        public new void Destroy()
        {
            
        }
    }
}