﻿namespace mFramework.UI
{
    public sealed class BaseView : UIView
    {
        protected override void CreateInterface(params object[] @params)
        {
            mCore.Log("[ui] Create base view");
        }

        public override void Tick()
        {
            base.Tick();
        }
    }
}