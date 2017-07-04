using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework
{
    public class djb2Hash
    {
        public static ulong GetHash(string str)
        {
            ulong hash = 5381;

            for (int i = 0; i < str.Length; i++)
                hash = ((hash << 5) + hash) + (ulong)str[i]; /* hash * 33 + c */

            return hash;
        }
    }
}
