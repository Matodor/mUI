using System.Collections.Generic;

namespace mFramework.Saves
{
    public static class SaveableFieldsHelper
    {
        public static void Save(this IEnumerable<SaveableFields> fields)
        {
            foreach (var v in fields)
            {
                v.Save();
            }
        }

        public static void Load(this IEnumerable<SaveableFields> fields)
        {
            foreach (var v in fields)
            {
                v.Load();
            }
        }
    }
}
