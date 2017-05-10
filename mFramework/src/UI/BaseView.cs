namespace mFramework
{
    public sealed class BaseView : UIView
    {
        protected override void CreateInterface(params object[] @params)
        {
            mCore.Log("[ui] Create base view");
        }
    }
}