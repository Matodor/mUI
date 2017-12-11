using mFramework;
using mFramework.UI;
using mFramework.UI.ScrollBar;

namespace Example
{
    public class ExampleScrollBarView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.VerticalScrollBar(new mFramework.UI.ScrollBar.UIScrollBarSettings
            {
                Min = 100,
                Max = 200,
                Step = 2,
                Default = 150
            });
            base.CreateInterface(@params);
        }
    }
}