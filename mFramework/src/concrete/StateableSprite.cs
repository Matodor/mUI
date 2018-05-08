using System;
using UnityEngine;

namespace mFramework
{
    public struct SpriteStates
    {
        public Sprite Default;
        public Sprite Highlighted;
        public Sprite Selected;
    }

    public class StateableSprite
    {
        public enum State
        {
            DEFAULT = 0,
            HIGHLIGHTED = 1,
            SELECTED = 2
        }

        public event Action<StateableSprite, Sprite> StateChanged = delegate { };
        public State CurrentState { get; private set; }
        public SpriteStates SpriteStates { get; set; }

        private StateableSprite(SpriteStates spriteStates)
        {
            SpriteStates = spriteStates;
            CurrentState = State.DEFAULT;
        }

        public static StateableSprite Create(SpriteStates spriteStates)
        {
            return new StateableSprite(spriteStates);
        }

        public void SetDefault()
        {
            CurrentState = State.DEFAULT;
            StateChanged.Invoke(this, SpriteStates.Default);
        }

        public void SetHighlighted()
        {
            CurrentState = State.HIGHLIGHTED;
            StateChanged.Invoke(this, SpriteStates.Highlighted);
        }

        public void SetSelected()
        {
            CurrentState = State.SELECTED;
            StateChanged.Invoke(this, SpriteStates.Selected);
        }
    }
}
