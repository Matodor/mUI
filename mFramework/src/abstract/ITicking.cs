using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    interface ITicking
    {
        void Tick();
        void FixedTick();
        void LateTick();
    }
}
