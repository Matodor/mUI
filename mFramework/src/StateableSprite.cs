using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public class StateableSprite
    {

        private StateableSprite()
        {
            
        }

        public static StateableSprite Create(Sprite defaultState, Sprite highlighted = null, Sprite selected = null)
        {
            return new StateableSprite();
        }
    }
}
