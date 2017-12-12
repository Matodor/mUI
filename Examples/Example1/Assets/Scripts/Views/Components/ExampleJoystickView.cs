using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleJoystickView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var joystick1 = CreateJoystick(0f, 0f);
            joystick1.PosY(this.RelativeY(1f) - joystick1.GetHeight() / 2);

            var joystick2 = CreateJoystick(Game.GetSprite("GreenCircle100px").WorldSize().x / 2, 0.5f);
            joystick2.PosY(this.RelativeY(0f) + joystick2.GetHeight() / 2);

            base.CreateInterface(@params);
        }

        private UIJoystick CreateJoystick(float paddingRadius, float friction)
        {
            var joystick = this.Joystick(new UIJoystickSettings
            {
                BackgroundSprite = Game.GetSprite("editor_drawcircle"),
                PointSprite = Game.GetSprite("GreenCircle100px"),
                PaddingRadius = paddingRadius,
                Friction = friction
            });

            var label = joystick.Label(new UILabelSettings
            {
                Text = $"pos={joystick.NormilizedPointPos}",
                Color = UIColors.Black,
                TextAnchor = TextAnchor.MiddleCenter,
                TextAlignment = TextAlignment.Center,
                Size = 40,
            });
            label.SortingOrder(3);
            joystick.PointMoved += _ => label.SetText($"pos={joystick.NormilizedPointPos}");

            return joystick;
        }
    }
}