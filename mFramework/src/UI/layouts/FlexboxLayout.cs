using System;
using UnityEngine;

namespace mFramework.UI.Layouts
{
    public class FlexboxLayoutProps : UILayoutProps
    {
        public virtual FlexboxDirection Direction { get; set; } = FlexboxDirection.COLUMN;
        public virtual FlexboxAlignItems AlignItems { get; set; } = FlexboxAlignItems.CENTER;
        public virtual float MarginBetween { get; set; } = 0f;
    }

    public enum FlexboxAlignItems
    {
        FLEX_START = 0,
        FLEX_END,
        CENTER,
        STRETCH,
    }

    public enum FlexboxDirection
    {
        ROW = 0,
        ROW_REVERSE,
        COLUMN,
        COLUMN_REVERSE
    }

    public class FlexboxLayout : UILayout
    {
        public FlexboxDirection Direction { get; private set; }
        public FlexboxAlignItems AlignItems { get; private set; }
        public float MarginBetween { get; private set; }

        protected override void CreateInterface(object[] @params)
        {

        }

        protected override void ApplyProps(UIViewProps props, IView parent)
        {
            if (props == null)
                throw new ArgumentNullException(nameof(props));

            if (!(props is FlexboxLayoutProps layoutSettings))
                throw new ArgumentException("FlexboxLayout: The given settings is not FlexboxLayoutSettings");

            base.ApplyProps(props, parent);

            AlignItems = layoutSettings.AlignItems;
            Direction = layoutSettings.Direction;
            MarginBetween = layoutSettings.MarginBetween;

            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
                SizeY = 0f;
            else
                SizeX = 0f;
        }

        protected override void OnChildAdded(IUIObject sender, IUIObject child)
        {
            var marginBetween = MarginBetween;

            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                if (Childs.Count > 1)
                {
                    SizeY += marginBetween + child.LocalHeight;
                }
                else
                    SizeY += child.LocalHeight;

                UnscaledCenterOffset = new Vector2(
                    0,
                    Direction == FlexboxDirection.COLUMN
                        ? -SizeY / 2
                        : +SizeY / 2
                );
            }
            else
            {
                if (Childs.Count > 1)
                    SizeX += marginBetween + child.LocalWidth;
                else
                    SizeX += child.LocalWidth;

                UnscaledCenterOffset = new Vector2(
                    Direction == FlexboxDirection.ROW
                        ? +SizeX / 2
                        : -SizeX / 2,
                    0
                );
            }
            
            var localShift = Vector2.zero;

            if (Childs.Count > 1)
            {
                var prevChild = Childs.LastItem.Prev.Value;
                
                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        child.Position(prevChild.Position(UIAnchor.MiddleRight, Space.Self), 
                            UIAnchor.MiddleLeft, Space.Self);
                        localShift.x += marginBetween;
                        break;

                    case FlexboxDirection.ROW_REVERSE:
                        child.Position(prevChild.Position(UIAnchor.MiddleLeft, Space.Self), 
                            UIAnchor.MiddleRight, Space.Self);
                        localShift.x -= marginBetween;
                        break;

                    case FlexboxDirection.COLUMN:
                        child.Position(prevChild.Position(UIAnchor.LowerCenter, Space.Self), 
                            UIAnchor.UpperCenter, Space.Self);
                        localShift.y -= marginBetween;
                        break;

                    case FlexboxDirection.COLUMN_REVERSE:
                        child.Position(prevChild.Position(UIAnchor.UpperCenter, Space.Self), 
                            UIAnchor.LowerCenter, Space.Self);
                        localShift.y += marginBetween;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        localShift.x += Padding.Left * Scale.x;
                        break;
                    case FlexboxDirection.ROW_REVERSE:
                        localShift.x -= Padding.Right * Scale.x;
                        break;
                    case FlexboxDirection.COLUMN:
                        localShift.y -= Padding.Top * Scale.y;
                        child.Position(this.Position(UIAnchor.UpperCenter), UIAnchor.UpperCenter);
                        break;
                    case FlexboxDirection.COLUMN_REVERSE:
                        localShift.y += Padding.Bottom * Scale.y;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            /* TODO FIX
            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                switch (AlignItems)
                {
                    case FlexboxAlignItems.FLEX_START:
                        localOffset.x = localOffset.x - SizeX / 2f + child.Width / 2f;
                        break;
                    case FlexboxAlignItems.FLEX_END:
                        localOffset.x = localOffset.x + SizeX / 2f - child.Width / 2f;
                        break;
                    case FlexboxAlignItems.STRETCH:
                        child.Scale(SizeX / child.Width, child.Scale.y);
                        break;
                }
            }
            else
            {
                switch (AlignItems)
                {
                    case FlexboxAlignItems.FLEX_START:
                        localOffset.y = localOffset.y - SizeY / 2f + child.Height / 2f;
                        break;
                    case FlexboxAlignItems.FLEX_END:
                        localOffset.y = localOffset.y + SizeY / 2f - child.Height / 2f;
                        break;
                    case FlexboxAlignItems.STRETCH:
                        child.Scale(child.Scale.x, SizeY / child.Height);
                        break;
                }
            }*/

            child.Translate(localShift, Space.Self);
        }
    }
}