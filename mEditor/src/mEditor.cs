using System;
using System.Collections.Generic;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    public sealed class mEditor
    {
        public static mEditor Instance { get; }

        static mEditor()
        {
            if (Instance == null)
                Instance = new mEditor();
        }

        public mEditor()
        {
            mCore.Log("mEditor created");
        }
    }
}
