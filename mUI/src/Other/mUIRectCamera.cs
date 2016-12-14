using mUIApp.Views;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Other
{
    public static class mUIRectCameraHelper
    {
        public static mUIRectCamera CreateRectCamera(this UIView view)
        {
            return new mUIRectCamera(view);
        }
    }

    public class mUIRectCamera
    {
        private readonly mUICamera _camera;
        private readonly UIView _attachView;

        public mUIRectCamera(UIView attachView)
        {
            _attachView = attachView;
            _attachView.Translate(0, 0, 1);
            _camera = new mUICamera(_attachView.GameObject);

            _attachView.OnTranslateEvent += (view, vector) =>
            {
                UpdateViewport();
            };

            _attachView.OnChangeHeight += (view, height) =>
            {
                _camera.SetOrthographicSize(height/2);
                UpdateViewport();
            };

            UpdateViewport();
        }

        private void UpdateViewport()
        {
            var viewportPos = mUI.UICamera.Camera.WorldToViewportPoint(_attachView.Transform.position);
            var sliderWidth = _attachView.Width;
            var sliderHeight = _attachView.Height;
            var lb = new Vector2(mUI.UICamera.Left, mUI.UICamera.Bottom);

            var sliderScreenWidthScale = _attachView.Width / (mUI.UICamera.Right - mUI.UICamera.Left);
            var sliderScreenHeightScale = _attachView.Height / (mUI.UICamera.Top - mUI.UICamera.Bottom);
            
            //mUI.Log("sliderScreenWidthScale: {0}", sliderScreenWidthScale);
            //mUI.Log("sliderScreenHeightScale: {0}", sliderScreenHeightScale);
     
            Vector2 minViewport = mUI.UICamera.Camera.WorldToViewportPoint(
                new Vector3(lb.x + sliderWidth / 2, lb.y + sliderHeight / 2, 0));
            
            _camera.Camera.rect = new Rect(
                viewportPos.x - minViewport.x,
                viewportPos.y - minViewport.y,
                sliderScreenWidthScale,
                sliderScreenHeightScale
            );
        }
    }
}
