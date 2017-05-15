using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public struct SpriteStates
    {
        public Sprite Default { get; set; }
        public Sprite Highlighted { get; set; }
        public Sprite Selected { get; set; }
    }

    public class StateableSprite
    {
        public enum State
        {
            DEFAULT = 0,
            HIGHLIGHTED = 1,
            SELECTED = 2
        }

        private readonly Sprite _defaultState, _highlightedState, _selectedState;
        private State _state;

        private StateableSprite(SpriteStates spriteStates)
        {
            _defaultState = spriteStates.Default;
            _highlightedState = spriteStates.Highlighted;
            _selectedState = spriteStates.Selected;

            _state = State.DEFAULT;
        }

        public static StateableSprite Create(SpriteStates spriteStates)
        {
            return new StateableSprite(spriteStates);
        }
    }
}
