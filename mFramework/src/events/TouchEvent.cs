using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public class TouchEvent : MouseEvent
    {
        public Touch Touch { get; set; }
    }
}
