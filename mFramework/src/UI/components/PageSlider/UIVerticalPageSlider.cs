using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class UIVerticalPageSlider : UIBasePageSlider
    {
        protected override void OnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            var sliderRect = GetRect();
            switch (ElementsDirection)
            {
                case LayoutElemsDirection.FORWARD:
                {
                    if (Childs.Count <= 1)
                    {
                        CurrentPage = 0;
                        child.LocalPos(0f, 0f);
                    }
                    else
                    {
                        child.Pos(new Vector2(
                            sliderRect.Position.x,
                            Pos().y - GetHeight()
                        ));
                    }
                }
                    break;

                case LayoutElemsDirection.BACKWARD:
                {
                    if (Childs.Count <= 1)
                    {
                        CurrentPage = 0;
                        child.LocalPos(0f, 0f);
                    }
                    else
                    {
                        child.Pos(new Vector2(
                            sliderRect.Position.x,
                            Pos().y + GetHeight()
                        ));
                    }
                }
                    break;
            }

            CheckChildOutsideSlider(child, ref sliderRect);
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

        public override void MoveNext()
        {
            if (IsAnimated || CurrentPage + 1 >= Childs.Count)
                return;

            var current = Childs[CurrentPage];
            var next = Childs[CurrentPage + 1];
            next.Show();

            Animate(current, true, GetHeight(), EasingCurrentPageType).AnimationEnded += _ =>
            {
                current.Hide();
                IsAnimated = false;
                OnPageChanged();
            };
            Animate(next, true, GetHeight(), EasingNextPageType);

            CurrentPage++;
            IsAnimated = true;
        }

        public override void MovePrev()
        {
            if (IsAnimated || CurrentPage - 1 < 0)
                return;

            var current = Childs[CurrentPage];
            var prev = Childs[CurrentPage - 1];
            prev.Show();

            Animate(current, true, -GetHeight(), EasingCurrentPageType).AnimationEnded += _ =>
            {
                current.Hide();
                IsAnimated = false;
                OnPageChanged();
            };
            Animate(prev, true, -GetHeight(), EasingNextPageType);

            CurrentPage--;
            IsAnimated = true;
        }

        protected override void SliderDrag(Vector2 drag)
        {
            if (IsAnimated || Mathf.Abs(drag.y) < 0.05f)
                return;

            // move prev
            if (drag.y < 0)
            {
                MovePrev();
            }
            // move next
            else if (drag.y > 0)
            {
                MoveNext();
            }
        }
    }
}