using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mFramework;

namespace Assets.Scripts.Views
{
    public class MainMenu : UIView
    {
        protected override void CreateInterface(params object[] @params)
        {
            mCore.Log(@params.ToString());
        }
    }
}
