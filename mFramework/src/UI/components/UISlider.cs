using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISliderSettings : UIComponentSettings
    {
        public float Height = 0;
        public float Width = 0;
        public float Offset = 0;
        public DirectionOfAddingSlides DirectionOfAddingSlides = DirectionOfAddingSlides.FORWARD;
        public UIObjectOrientation SliderType = UIObjectOrientation.HORIZONTAL;
        public ushort StencilId = 4;
    }

    public enum DirectionOfAddingSlides
    {
        FORWARD = 0,
        BACKWARD = 1,
    }

    public class UISlider : UIComponent, IUIClickable, IView
    {
        public ushort? StencilId => _stencilId;

        public UIObjectOrientation Orientation { get; private set; }
        public UIClickable UIClickable { get; private set; }

        public const float SLIDER_MAX_PATH_TO_CLICK = 0.03f;
        public const float SLIDER_MIN_DIFF_TO_MOVE = 0.0001f;

        protected DirectionOfAddingSlides _directionOfAddingSlides;
        protected bool _isPressed;

        private ushort _stencilId;

        private float _height;
        private float _width;
        private float _offset;
        private float _lastMoveDiff;
        private float _dragPath;
        private float _lastFreeSpace;

        private Vector2 _lastMousePos;
        private List<Pair<IUIClickable, MouseEvent>> _clickNext;

        protected override void Init()
        {
            _lastMoveDiff = 0;
            _isPressed = false;
            _clickNext = new List<Pair<IUIClickable, MouseEvent>>();

            ChildObjectAdded += OnChildObjectAdded;
            base.Init();
        }

        protected virtual void OnChildObjectAdded(UIObject sender, AddedСhildObjectEventArgs e)
        {
            if (Orientation == UIObjectOrientation.HORIZONTAL)
                SetupChildrenHorizontal(e.AddedObject);
            else
                SetupChildrenVertical(e.AddedObject);

            SetupChilds(sender, e);
        }

        private void SetupChilds(UIObject sender, AddedСhildObjectEventArgs e)
        {
            if (e.AddedObject is IUIClickable uiClickable)
            {
                uiClickable.UIClickable.CanMouseDown += CanChildsMouseDown;
                uiClickable.UIClickable.CanMouseUp += CanChildsMouseUp;
            }
            
            e.AddedObject.ChildObjectAdded += SetupChilds;
        }
        
        /*private void UpdateViewport()
        {
            var viewportPos = mUI.UICamera.Camera.WorldToViewportPoint(Position());
            var lb = new Vector2(mUI.UICamera.Left, mUI.UICamera.Bottom);

            var sliderScreenWidthScale = GetWidth() / mUI.UICamera.Width;
            var sliderScreenHeightScale = GetHeight() / mUI.UICamera.Height;

            var minViewport = mUI.UICamera.Camera.WorldToViewportPoint(
                new Vector3(lb.x + GetWidth() / 2, lb.y + GetHeight() / 2, 0));

            _camera.Camera.rect = new Rect(
                viewportPos.x - minViewport.x,
                viewportPos.y - minViewport.y,
                sliderScreenWidthScale,
                sliderScreenHeightScale
            );
        }*/
      
        private static bool CanChildsMouseUp(IUIClickable handler, MouseEvent @event)
        {
            return false;
        }

        private bool CanChildsMouseDown(IUIClickable handler, MouseEvent @event)
        {
            if (UIClickable.Area2D.InArea(mUI.UICamera.ScreenToWorldPoint(@event.MouseScreenPos)) &&
                _dragPath < SLIDER_MAX_PATH_TO_CLICK)
            {
                _clickNext.Add(new Pair<IUIClickable, MouseEvent>(handler, @event));
                return true;
            }
            return false;   
        }

        protected virtual void SetupChildrenHorizontal(UIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                {
                    if (Childs.Count <= 1)
                        obj.Position(rect.Left + obj.GetWidth() / 2, rect.Position.y);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = last.GetRect().Right + obj.GetWidth() / 2 + _offset,
                            y = rect.Position.y,
                        });
                    }
                }
                break;

                case DirectionOfAddingSlides.BACKWARD:
                {
                    if (Childs.Count <= 1)
                        obj.Position(rect.Right - obj.GetWidth() / 2, rect.Position.y);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = last.GetRect().Left - obj.GetWidth() / 2 - _offset,
                            y = rect.Position.y,
                        });
                    }
                }
                break;
            }

            CheckHRect(obj, ref rect);
        }

        protected virtual void SetupChildrenVertical(UIObject obj)
        {
            var rect = GetRect();
            switch (_directionOfAddingSlides)
            {
                case DirectionOfAddingSlides.FORWARD:
                {
                    if (Childs.Count <= 1)
                        obj.Position(rect.Position.x, rect.Top - obj.GetHeight() / 2);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.GetRect().Bottom - obj.GetHeight() / 2 - _offset,
                        });
                    }
                }
                break;

                case DirectionOfAddingSlides.BACKWARD:
                {
                    if (Childs.Count <= 1)
                        obj.Position(rect.Position.x, rect.Bottom + obj.GetHeight() / 2);
                    else
                    {
                        var last = Childs.LastItem.Prev.Value;
                        obj.Position(new Vector2
                        {
                            x = rect.Position.x,
                            y = last.GetRect().Top + obj.GetHeight() / 2 + _offset,
                        });
                    }
                }
                break;
            }

            CheckVRect(obj, ref rect);
        }

        protected override void ApplySettings(UIComponentSettings settings)
        { 
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UISliderSettings sliderSettings))
                throw new ArgumentException("UISlider: The given settings is not UIComponentSettings");

            _stencilId = sliderSettings.StencilId;

            Orientation = sliderSettings.SliderType;
            _directionOfAddingSlides = sliderSettings.DirectionOfAddingSlides;
            _height = sliderSettings.Height;
            _width = sliderSettings.Width;
            _offset = sliderSettings.Offset;

            var area = new RectangleArea2D();
            area.Update += a =>
            {
                area.Width = GetWidth();
                area.Height = GetHeight();
            };

            UIClickable = new UIClickable(this, area);

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(_stencilId).CanvasMaterial;
            meshRenderer.sharedMaterial.SetTexture("_MainTex", Texture2D.blackTexture);
            meshRenderer.sortingOrder = SortingOrder();
            UIStencilMaterials.CreateMesh(this);

            SortingOrderChanged += sender =>
            {
                meshRenderer.sortingOrder = SortingOrder();
            };

            base.ApplySettings(settings);
        }
        
        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }

        public virtual void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
            _lastMousePos = worldPos;
        }

        public virtual void MouseUp(Vector2 worldPos)
        {
            _isPressed = false;
            _lastMousePos = worldPos;
            
            if (_clickNext.Count > 0)
            {
                if (_dragPath < SLIDER_MAX_PATH_TO_CLICK)
                    _clickNext.ForEach(e => e.First.MouseUp(worldPos));
                else 
                    _clickNext.ForEach(e => e.First.MouseUp(new Vector2(float.MinValue, float.MinValue))); // fake pos
                _clickNext.Clear();
            }
        
            _dragPath = 0;
        }

        public virtual void MouseDrag(Vector2 worldPos)
        {
            if (!_isPressed)
                return;

            var diff = worldPos - _lastMousePos;
            if (Orientation == UIObjectOrientation.HORIZONTAL)
            {
                _dragPath += Math.Abs(diff.x);
                HorizontalMove(diff.x);
            }
            else
            {
                _dragPath += Math.Abs(diff.y);
                VerticalMove(diff.y);
            }

            _lastMousePos = worldPos;
        }

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
                    firstFreeSpace = rect.Bottom - secondRect.Bottom;
                    secondFreeSpace = firstRect.Top - rect.Top;
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

        protected virtual void VerticalMove(float diff)
        {
            if (Childs.Count == 0)
                return;

            UIRect topItemRect, bottomItemRect;
            if (_directionOfAddingSlides == DirectionOfAddingSlides.FORWARD)
            {
                topItemRect = Childs.FirstItem.Value.GetRect();
                bottomItemRect = Childs.LastItem.Value.GetRect();
            }
            else
            {
                topItemRect = Childs.LastItem.Value.GetRect();
                bottomItemRect = Childs.FirstItem.Value.GetRect();
            }

            var rect = GetRect();
            var pureHeight = topItemRect.Top - bottomItemRect.Bottom;

            if (pureHeight <= GetHeight())
                return;

            float freeSpace;

            // move top
            // check bottom space
            if (diff > 0)
            { 
                freeSpace = rect.Bottom - bottomItemRect.Bottom;
                freeSpace -= diff;
            }
            // move bottom
            // check top space
            else
            {
                freeSpace = topItemRect.Top - rect.Top;
                freeSpace += diff;
            }

            // if freeSpace > 0 outside slider
            // if freeSpace < 0 inside slider

            if (freeSpace < 0)
            {
                var absFreeSpace = Mathf.Abs(freeSpace);
                var t = mMath.Clamp(absFreeSpace / (GetHeight() / 3f), 0f, 1f);

                if (Mathf.Abs(_lastFreeSpace) < absFreeSpace)
                    diff *= mMath.Clamp(1f - t, 0f, 1f);
            }

            Childs.ForEach(c =>
            {
                c.Translate(0, diff);
                CheckVRect(c, ref rect);
            });

            _lastMoveDiff = diff;
            _lastFreeSpace = freeSpace;
        }

        private static void CheckVRect(UIObject c, ref UIRect sliderRect)
        {
            var r = c.GetRect();
            if (r.Bottom > sliderRect.Top || r.Top < sliderRect.Bottom)
            {
                if (c.gameObject.activeSelf)
                    c.Hide();
            }
            else if (!c.gameObject.activeSelf)
            {
                c.Show();
            }
        }

        private static void CheckHRect(UIObject c, ref UIRect sliderRect)
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
    }
}
