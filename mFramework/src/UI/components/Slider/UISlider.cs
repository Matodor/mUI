using System;
using UnityEngine;

namespace mFramework.UI
{
    /*public class UISlider : UIComponent, IUIClickable, IView
    {


        public void Move(float diff)
        {
            if (Mathf.Abs(diff) < SLIDER_MIN_DIFF_TO_MOVE)
                return;
            
            if (Orientation == UIObjectOrientation.HORIZONTAL)
                HorizontalMove(diff);
            else
                VerticalMove(diff);
        }

        protected override void OnTick()
        {
            if (!_isPressed)
            {
                if (Childs.Count == 0)
                    return;

                var rect = GetRect();
                UIRect firstRect, secondRect;
                if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
                {
                    firstRect = Childs.FirstItem.Value.GetRect();
                    secondRect = Childs.LastItem.Value.GetRect();
                }
                else
                {
                    firstRect = Childs.LastItem.Value.GetRect();
                    secondRect = Childs.FirstItem.Value.GetRect();
                }
                float firstFreeSpace;
                float secondFreeSpace;

                if (Orientation == UIObjectOrientation.VERTICAL)
                {
                    firstFreeSpace = firstRect.Top - rect.Top;
                    secondFreeSpace = rect.Bottom - secondRect.Bottom;
                }
                else
                {
                    firstFreeSpace = rect.Left - firstRect.Left;
                    secondFreeSpace = secondRect.Right - rect.Right;
                }
                
                if (firstFreeSpace < 0)
                {
                    Move(-Mathf.Abs(firstFreeSpace) * 0.1f * Time.deltaTime * 50);
                }
                else if (secondFreeSpace < 0)
                {
                    Move(Mathf.Abs(secondFreeSpace) * 0.1f * Time.deltaTime * 50);
                }
                else if (Math.Abs(_lastMoveDiff) > SLIDER_MIN_DIFF_TO_MOVE)
                    Move(_lastMoveDiff * 0.99f * Time.deltaTime * 50);
            }
            base.OnTick();
        }

        private static void CheckHRect(IUIObject c, ref UIRect sliderRect)
        {
            var r = c.GetRect();
            if (r.Right < sliderRect.Left || r.Left > sliderRect.Right)
            {
                if (c.gameObject.activeSelf)
                    c.Hide();
            }
            else if (!c.gameObject.activeSelf)
            {
                c.Show();
            }
        }

        protected virtual void HorizontalMove(float diff)
        {
            if (Childs.Count == 0)
                return;

            UIRect leftRect, rightRect;
            if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
            {
                leftRect = Childs.FirstItem.Value.GetRect();
                rightRect = Childs.LastItem.Value.GetRect();
            }
            else
            {
                leftRect = Childs.LastItem.Value.GetRect();
                rightRect = Childs.FirstItem.Value.GetRect();
            }

            var rect = GetRect();
            var pureWidth = rightRect.Right - leftRect.Left;

            if (pureWidth <= GetWidth())
                return;

            float freeSpace;

            // move right
            if (diff > 0)
            {
                freeSpace = rect.Left - leftRect.Left;
                freeSpace -= diff;
            }
            // move left
            else
            {
                freeSpace = rightRect.Right - rect.Right;
                freeSpace += diff;
            }

            // if freeSpace > 0 outside slider
            // if freeSpace < 0 inside slider

            if (freeSpace < 0)
            {
                var absFreeSpace = Mathf.Abs(freeSpace);
                var t = mMath.Clamp(absFreeSpace / (GetWidth() / 3f), 0f, 1f);

                if (Mathf.Abs(_lastFreeSpace) < absFreeSpace)
                    diff *= mMath.Clamp(1f - t, 0f, 1f);
            }

            Childs.ForEach(c =>
            {
                c.Translate(diff, 0);
                CheckHRect(c, ref rect);
            });

            _lastMoveDiff = diff;
            _lastFreeSpace = freeSpace;
        }
    }*/
}
