using UnityEngine;

namespace mFramework
{
    public sealed class PlayerPrefsStorage : IKeyValueStorage
    {
        public PlayerPrefsStorage()
        {
            
        }

        public void Save()
        {
            PlayerPrefs.Save();   
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        public void RemoveKey(string key)
        {
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }

        public bool GetInt(string key, out int value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetInt(key);
                return true;
            }

            value = default(int);
            return false;
        }

        public bool GetFloat(string key, out float value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key);
                return true;
            }

            value = default(float);
            return false;
        }

        public bool GetString(string key, out string value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetString(key);
                return true;
            }

            value = default(string);
            return false;
        }

        public bool SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            return true;
        }

        public bool SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            return true;
        }

        public bool SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            return true;
        }
    }
}
