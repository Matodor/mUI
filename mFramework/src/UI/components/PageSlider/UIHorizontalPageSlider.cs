using mFramework.UI.Layouts;
using UnityEngine;
/*
namespace mFramework.UI
{
    public class UIHorizontalPageSlider : UIBasePageSlider
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
                                Pos().x + GetWidth(),
                                sliderRect.Position.y
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
                                Pos().x - GetWidth(),
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

        public override bool CanMoveNext()
        {
            return !IsAnimated && CurrentPage + 1 < Childs.Count && BeforeMove();
        }

        public override bool MoveNext()
        {
            if (!CanMoveNext())
                return false;

            var current = Childs[CurrentPage];
            var next = Childs[CurrentPage + 1];
            next.Show();

            Animate(current, false, -GetWidth(), EasingCurrentPageType).AnimationEnded += _ =>
            {
                current.Hide();
                IsAnimated = false;
                OnPageChanged(current, next);
            };
            Animate(next, false, -GetWidth(), EasingNextPageType);
            OnBeforePageChange(current, next);

            CurrentPage++;
            IsAnimated = true;
            return true;
        }

        public override bool CanMovePrev()
        {
            return !IsAnimated && CurrentPage - 1 >= 0 && BeforeMove();
        }

        public override bool MovePrev()
        {
            if (!CanMovePrev())
                return false;

            var current = Childs[CurrentPage];
            var next = Childs[CurrentPage - 1];
            next.Show();

            Animate(current, false, GetWidth(), EasingCurrentPageType).AnimationEnded += _ =>
            {
                current.Hide();
                IsAnimated = false;
                OnPageChanged(current, next);
            };
            Animate(next, false, GetWidth(), EasingNextPageType);
            OnBeforePageChange(current, next);

            CurrentPage--;
            IsAnimated = true;
            return true;
        }

        protected override void SliderDrag(Vector2 drag)
        {
            if (IsAnimated || Mathf.Abs(drag.x) < 0.05f)
                return;

            // move prev
            if (drag.x < 0)
            {
                MoveNext();
            }
            // move next
            else if (drag.x > 0)
            {
                MovePrev();
            }
        }
    }
}*/