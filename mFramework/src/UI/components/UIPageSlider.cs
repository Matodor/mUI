using UnityEngine;

namespace mFramework.UI
{

    /*public class UIPageSlider : UISlider
    {
        public event UIEventHandler<UIPageSlider> PageChanged = delegate { };

        public EasingType BackEasingType = EasingType.easeOutBack;
        public EasingType NextPageEasingType = EasingType.easeOutBack;
        public float NextPageDuration = 1f;

        private UILinearAnimation _nextAnimation;
        private UILinearAnimation _prevAnimation;
        private bool _isAnimated;

        private int _currentIndex;
        private float _diff;

        protected override void Init()
        {
            _currentIndex = -1;
            base.Init();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            base.ApplySettings(settings);
        }

        protected override void SetupChildrenHorizontal(IUIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                {
                    if (Childs.Count <= 1)
                    {
                        obj.Position(Position());
                        _currentIndex = 0;
                    }
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = last.Position().x + GetWidth(),
                            y = rect.Position.y,
                        });

                        if (Childs.Count > 2)
                            obj.Hide();
                    }
                }
                break;

                case DirectionOfAddingSlides.BACKWARD:
                {
                    if (Childs.Count <= 1)
                    {
                        obj.Position(Position());
                        _currentIndex = 0;
                    }
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = last.Position().x - GetWidth(),
                            y = rect.Position.y,
                        });
                        
                        if (Childs.Count > 2)
                            obj.Hide();
                    }
                }
                break;
            }
        }

        protected override void SetupChildrenVertical(IUIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                {
                    if (Childs.Count <= 1)
                    {
                        obj.Position(Position());
                        _currentIndex = 0;
                    }
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.Position().y - GetHeight(),
                        });
                        
                        if (Childs.Count > 2)
                            obj.Hide();
                    }
                }
                break;

                case DirectionOfAddingSlides.BACKWARD:
                {
                    if (Childs.Count <= 1)
                    {
                        obj.Position(Position());
                        _currentIndex = 0;
                    }
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.Position().y + GetHeight(),
                        });
                        
                        if (Childs.Count > 2)
                            obj.Hide();
                    }
                }
                break;
            }
        }

        public override void MouseUp(Vector2 worldPos)
        {
            if (_isPressed)
            {
                if (Mathf.Abs(_diff) < GetWidth() / 3)
                {
                    _isAnimated = true;
                    var current = Childs[_currentIndex];
                    var anim = current.LinearAnimation(new UILinearAnimationSettings
                    {
                        StartPos = current.Position(),
                        EndPos = Position(),
                        Duration = 0.3f,
                        EasingType = BackEasingType
                    });
                    anim.AnimationEnded += s =>
                    {
                        _isAnimated = false;
                    };
                }
                else
                {
                    if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
                    {
                        if (_diff > 0)
                            MovePrev();
                        else
                            MoveNext();
                    }
                    else
                    {
                        if (_diff > 0)
                            MoveNext();
                        else
                            MovePrev();
                    }
                }

                _diff = 0f;
            }

            base.MouseUp(worldPos);
        }

        private void Move(IUIObject current, IUIObject next, bool toNext)
        {
            next.Show();
            Vector2 endCurrentPos;
            
            if (Orientation == UIObjectOrientation.HORIZONTAL)
            {
                endCurrentPos = Position() + new Vector2(
                    _directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                        ? (toNext ? -GetWidth() : +GetWidth())
                        : (toNext ? +GetWidth() : -GetWidth()),
                0);

                if (toNext)
                {
                    next.Position(new Vector2
                    {
                        x = Position().x + (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                            ? +GetWidth()
                            : -GetWidth()
                        ),
                        y = Position().y
                    });
                }
                else
                {
                    next.Position(new Vector2
                    {
                        x = Position().x + (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                            ? -GetWidth()
                            : +GetWidth()
                        ),
                        y = Position().y
                    });
                }
            }
            else
            {
                endCurrentPos = Position() + new Vector2(0,
                    _directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                        ? (toNext ? +GetHeight() : -GetHeight())
                        : (toNext ? -GetHeight() : +GetHeight())
                );

                if (toNext)
                {
                    next.Position(new Vector2
                    {
                        x = Position().x,
                        y = Position().y + (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                            ? -GetHeight()
                            : +GetHeight()
                        )
                    });
                }
                else
                {
                    next.Position(new Vector2
                    {
                        x = Position().x,
                        y = Position().y + (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD
                            ? +GetHeight()
                            : -GetHeight()
                        )
                    });
                }
            }

            _prevAnimation = current.LinearAnimation(new UILinearAnimationSettings
            {
                StartPos = current.Position(),
                EndPos = endCurrentPos,
                Duration = NextPageDuration,
                EasingType = NextPageEasingType
            });

            _prevAnimation.AnimationEnded += anim =>
            {
                anim.AnimatedObject.Hide();
            };

            _nextAnimation = next.LinearAnimation(new UILinearAnimationSettings
            {
                StartPos = next.Position(),
                EndPos = Position(),
                Duration = NextPageDuration,
                EasingType = NextPageEasingType
            });

            _nextAnimation.AnimationEnded += anim =>
            {
                _isAnimated = false;

                _currentIndex = toNext ? _currentIndex + 1 : _currentIndex - 1;
                PageChanged.Invoke(this);
            };

            _isAnimated = true;
        }

        private void MoveNext()
        {
            if (_isAnimated || _currentIndex == Childs.Count - 1)
                return;

            var current = Childs[_currentIndex];
            var next = Childs[_currentIndex + 1];

            Move(current, next, true);
        }

        private void MovePrev()
        {
            if (_isAnimated || _currentIndex == 0)
                return;

            var current = Childs[_currentIndex];
            var next = Childs[_currentIndex - 1];

            Move(current, next, false);
        }

        private bool CanNextToMove()
        {
            return Mathf.Abs(_diff) > GetWidth() / 3;
        }

        protected override void HorizontalMove(float diff)
        {
            if (_isAnimated)
                return;

            if (CanNextToMove())
            {
                if (diff > 0)
                {
                    if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
                        MovePrev();
                    else
                        MoveNext();
                }
                else
                {
                    if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
                        MoveNext();
                    else
                        MovePrev();
                }
                return;    
            }

            _diff += diff;
            Childs[_currentIndex].Translate(diff, 0);
        }

        protected override void VerticalMove(float diff)
        {
            if (_isAnimated)
                return;

            if (CanNextToMove())
            {
                if (diff > 0)
                {
                    if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
                        MoveNext();
                    else
                        MovePrev();
                }
                else
                {
                    if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
                        MovePrev();
                    else
                        MoveNext();
                }
                return;
            }

            _diff += diff;
            Childs[_currentIndex].Translate(0, diff);
        }
    }*/
}