using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public abstract class UIObject : ITicking
    {
        public void Tick()
        {
            throw new NotImplementedException();
        }

        public void FixedTick()
        {
            throw new NotImplementedException();
        }

        public void LateTick()
        {
            throw new NotImplementedException();
        }
    }
}
