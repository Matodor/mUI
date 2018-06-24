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
                 //   SizeY += marginBetween + child.LocalHeight;
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

            Vector2 localPos;
            if (Childs.Count > 1)
            {
                var prevChild = Childs.LastItem.Prev.Value;

                child.Position(prevChild.Position(UIAnchor.LowerCenter, Space.Self), 
                    UIAnchor.UpperCenter, Space.Self);

                return;
                var w = marginBetween + child.LocalWidth / 2f;
                var h = marginBetween + child.LocalHeight / 2f;
                
                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        localPos = prevChild.Position(UIAnchor.MiddleRight, Space.Self);
                        localPos.x += w;
                        localPos.y = 0f;
                        break;

                    case FlexboxDirection.ROW_REVERSE:
                        localPos = prevChild.Position(UIAnchor.MiddleLeft, Space.Self);
                        localPos.x -= w;
                        localPos.y = 0f;
                        break;

                    case FlexboxDirection.COLUMN:
                        localPos = prevChild.Position(UIAnchor.LowerCenter, Space.Self);
                        localPos.y -= h;
                        localPos.x = 0f;
                        break;

                    case FlexboxDirection.COLUMN_REVERSE:
                        localPos = prevChild.Position(UIAnchor.UpperCenter, Space.Self);
                        localPos.y += h;
                        localPos.x = 0f;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                localPos = UnscaledCenterOffset;

                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        localPos.x += Padding.Left * Scale.x;
                        break;
                    case FlexboxDirection.ROW_REVERSE:
                        localPos.x -= Padding.Right * Scale.x;
                        break;
                    case FlexboxDirection.COLUMN:
                        localPos.y -= Padding.Top * Scale.y;
                        break;
                    case FlexboxDirection.COLUMN_REVERSE:
                        localPos.y += Padding.Bottom * Scale.y;
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

            //child.Position(localOffset, UIAnchor.MiddleCenter, Space.Self);
        }
    }
}