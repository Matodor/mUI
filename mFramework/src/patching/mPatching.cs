using System;
using System.Reflection;
using UnityEngine;

namespace mFramework.Patching
{
    public static class mPatching
    {
        public static Assembly LoadAssemblyFromResource(string path)
        {
            var textAsset = Resources.Load<TextAsset>(path);
            return AppDomain.CurrentDomain.Load(textAsset.bytes);
        }
    }
} 