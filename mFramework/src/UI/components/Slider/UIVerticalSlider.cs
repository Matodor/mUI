using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class UIVerticalSlider : UIBaseSlider
    {
        private float _lastInnerSpace;

        protected override void OnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            var rect = GetRect();
            switch (ElementsDirection)
            {
                case LayoutElemsDirection.FORWARD:
                {
                    if (Childs.Count <= 1)
                        child.Position(rect.Position.x, rect.Top - child.GetHeight() / 2);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        child.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.GetRect().Bottom - child.GetHeight() / 2 - ElementsOffset,
                        });
                    }
                } break;

                case LayoutElemsDirection.BACKWARD:
                {
                    if (Childs.Count <= 1)
                        child.Position(rect.Position.x, rect.Bottom + child.GetHeight() / 2);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        child.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.GetRect().Top + child.GetHeight() / 2 + ElementsOffset,
                        });
                    }
                } break;
            }

            CheckChildOutsideSlider(child, ref rect);
        }

        private static void CheckChildOutsideSlider(IUIObject child, ref UIRect sliderRect)
        {
            var childRect = child.GetRect();
            if (childRect.Bottom > sliderRect.Top || childRect.Top < sliderRect.Bottom)
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
            VerticalMove(drag.y);
        }

        protected override bool CheckInnerSpace()
        {
            if (Childs.Count == 0)
                return false;

            var sliderRect = GetRect();
            UIRect topItemRect, bottomItemRect;

            if (ElementsDirection == LayoutElemsDirection.FORWARD)
            {
                topItemRect = Childs.FirstItem.Value.GetRect();
                bottomItemRect = Childs.LastItem.Value.GetRect();
            }
            else
            {
                topItemRect = Childs.LastItem.Value.GetRect();
                bottomItemRect = Childs.FirstItem.Value.GetRect();
            }

            var bottomFreeSpace = sliderRect.Bottom - bottomItemRect.Bottom;
            var topFreeSpace = topItemRect.Top - sliderRect.Top;
            var pureHeight = topItemRect.Top - bottomItemRect.Bottom;

            if (pureHeight < GetHeight())
            {
                if (ElementsDirection == LayoutElemsDirection.FORWARD)
                {
                    VerticalMove(-topFreeSpace * 0.1f * Time.deltaTime * 50);
                }
                else
                {
                    VerticalMove(bottomFreeSpace * 0.1f * Time.deltaTime * 50);
                }

                return true;
            }

            if (topFreeSpace < 0 && topFreeSpace < bottomFreeSpace)
            {
                VerticalMove(-topFreeSpace * 0.1f * Time.deltaTime * 50);
                return true;
            }
            if (bottomFreeSpace < 0)
            {
                VerticalMove(bottomFreeSpace * 0.1f * Time.deltaTime * 50);
                return true;
            }

            return false;
        }

        private void VerticalMove(float diff)
        {
            if (Childs.Count == 0 || Mathf.Abs(diff) < SLIDER_MIN_DIFF_TO_MOVE)
            {
                return;
            }

            var sliderRect = GetRect();
            UIRect topItemRect, bottomItemRect;

            if (ElementsDirection == LayoutElemsDirection.FORWARD)
            {
                topItemRect = Childs.FirstItem.Value.GetRect();
                bottomItemRect = Childs.LastItem.Value.GetRect();
            }
            else
            {
                topItemRect = Childs.LastItem.Value.GetRect();
                bottomItemRect = Childs.FirstItem.Value.GetRect();
            }

            float freeSpace;
            float newInnerSpace;
            var pureHeight = topItemRect.Top - bottomItemRect.Bottom;

            if (pureHeight < GetHeight())
            {
                if (ElementsDirection == LayoutElemsDirection.FORWARD)
                {
                    freeSpace = topItemRect.Top - sliderRect.Top;
                    newInnerSpace = freeSpace + diff;
                }
                else
                {
                    freeSpace = sliderRect.Bottom - bottomItemRect.Bottom;
                    newInnerSpace = freeSpace - diff;
                }
            }
            else
            {
                // check bottom space
                if (diff > 0)
                {
                    freeSpace = sliderRect.Bottom - bottomItemRect.Bottom;
                    newInnerSpace = freeSpace - diff;
                }
                // check top space
                else
                {
                    freeSpace = topItemRect.Top - sliderRect.Top;
                    newInnerSpace = freeSpace + diff;
                }
            }

            if (freeSpace < 0)
            {
                var innerSpace = Mathf.Abs(newInnerSpace);
                var maxFreeSpace = GetHeight() / 3f;

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
                    child.Translate(0, diff);
                    CheckChildOutsideSlider(child, ref sliderRect);
                });
            }
        }
    }
}