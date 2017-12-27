using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class UIHorizontalSlider : UIBaseSlider
    {
        private float _lastInnerSpace;

        protected override void OnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            var sliderRect = GetRect();
            switch (ElementsDirection)
            {
                case LayoutElemsDirection.FORWARD:
                {
                    if (Childs.Count <= 1)
                        child.Pos(sliderRect.Left + child.GetWidth() / 2 + Padding, sliderRect.Position.y);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        child.Pos(new Vector2(
                            last.GetRect().Right + child.GetWidth() / 2 + ElementsOffset,
                            sliderRect.Position.y
                        ));
                    }
                }
                    break;

                case LayoutElemsDirection.BACKWARD:
                {
                    if (Childs.Count <= 1)
                        child.Pos(sliderRect.Right - child.GetWidth() / 2 - Padding, sliderRect.Position.y);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        child.Pos(new Vector2(
                            last.GetRect().Left - child.GetWidth() / 2 - ElementsOffset,
                            sliderRect.Position.y
                        ));
                    }
                }
                    break;
            }

            CheckChildOutsideSlider(child, ref sliderRect);
        }

        private static void CheckChildOutsideSlider(IUIObject child, ref UIRect sliderRect)
        {
            var r = child.GetRect();
            if (r.Right < sliderRect.Left || r.Left > sliderRect.Right)
            {
                if (child.gameObject.activeSelf)
                    child.Hide();
            }
            else if (!child.gameObject.activeSelf)
            {
                child.Show();
            }
        }

        protected override void SliderDrag(Vector2 drag)
        {
            HorizontalMove(drag.x);
        }

        protected override bool CheckInnerSpace()
        {
            if (Childs.Count == 0)
                return false;

            var sliderRect = GetRect();
            UIRect leftItemRect, rightItemRect;

            if (ElementsDirection == LayoutElemsDirection.FORWARD)
            {
                leftItemRect = Childs.FirstItem.Value.GetRect();
                rightItemRect = Childs.LastItem.Value.GetRect();
            }
            else
            {
                leftItemRect = Childs.LastItem.Value.GetRect();
                rightItemRect = Childs.FirstItem.Value.GetRect();
            }

            var leftFreeSpace = sliderRect.Left - leftItemRect.Left + Padding;
            var rightFreeSpace = rightItemRect.Right - sliderRect.Right + Padding;
            var pureWidth = rightItemRect.Right - leftItemRect.Left;

            if (pureWidth < GetWidth())
            {
                if (ElementsDirection == LayoutElemsDirection.FORWARD)
                {
                    HorizontalMove(leftFreeSpace * 0.1f * Time.deltaTime * 50);
                }
                else
                {
                    HorizontalMove(-rightFreeSpace * 0.1f * Time.deltaTime * 50);
                }

                return true;
            }

            if (leftFreeSpace < 0 && leftFreeSpace < rightFreeSpace)
            {
                HorizontalMove(leftFreeSpace * 0.1f * Time.deltaTime * 50);
                return true;
            }
            if (rightFreeSpace < 0)
            {
                HorizontalMove(-rightFreeSpace * 0.1f * Time.deltaTime * 50);
                return true;
            }

            return false;
        }
        private void HorizontalMove(float diff)
        {
            if (Childs.Count == 0 || Mathf.Abs(diff) < SLIDER_MIN_DIFF_TO_MOVE)
            {
                return;
            }

            var sliderRect = GetRect();
            UIRect leftItemRect, rightItemRect;

            if (ElementsDirection == LayoutElemsDirection.FORWARD)
            {
                leftItemRect = Childs.FirstItem.Value.GetRect();
                rightItemRect = Childs.LastItem.Value.GetRect();
            }
            else
            {
                leftItemRect = Childs.LastItem.Value.GetRect();
                rightItemRect = Childs.FirstItem.Value.GetRect();
            }

            float freeSpace;
            float newInnerSpace;
            var pureWidth = rightItemRect.Right - leftItemRect.Left;

            if (pureWidth < GetWidth())
            {
                if (ElementsDirection == LayoutElemsDirection.FORWARD)
                {
                    freeSpace = sliderRect.Left - leftItemRect.Left + Padding;
                    newInnerSpace = freeSpace - diff;
                }
                else
                {
                    freeSpace = rightItemRect.Right - sliderRect.Right + Padding;
                    newInnerSpace = freeSpace + diff;
                }
            }
            else
            {
                // check left space
                if (diff > 0)
                {
                    freeSpace = sliderRect.Left - leftItemRect.Left + Padding;
                    newInnerSpace = freeSpace - diff;
                }
                // check right space
                else
                {
                    freeSpace = rightItemRect.Right - sliderRect.Right + Padding;
                    newInnerSpace = freeSpace + diff;
                }
            }

            if (freeSpace < 0)
            {
                var innerSpace = Mathf.Abs(newInnerSpace);
                var maxFreeSpace = GetWidth() / 3f;

                if (_lastInnerSpace < innerSpace &&
                    innerSpace >= maxFreeSpace)
                {
                    return;
                }

                var t = mMath.Clamp(innerSpace / maxFreeSpace, 0f, 1f);
                diff *= mMath.Clamp(1f - t, 0f, 1f);
                _lastInnerSpace = innerSpace;
            }

            if (Mathf.Abs(diff) > SLIDER_MIN_DIFF_TO_MOVE)
            {
                Childs.ForEach(child =>
                {
                    child.Translate(diff, 0);
                    CheckChildOutsideSlider(child, ref sliderRect);
                });
            }
        }
    }
}