namespace mFramework.UI
{
    public class UIContainer : UIComponent
    {
        private float _width;
        private float _height;

        protected override void Init()
        {
            base.Init();
        }

        public UIContainer SetWidth(float width)
        {
            _width = width;
            return this;
        }

        public UIContainer SetHeight(float height)
        {
            _height = height;
            return this;
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }
    }
}
