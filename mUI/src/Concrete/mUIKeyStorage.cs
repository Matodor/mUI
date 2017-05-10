using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp
{
    public class mUIKeyStorage : IKeyStorage
    {
        public event Action<mUIKeyStorage> OnBeforeSave;
        public event Action<mUIKeyStorage> OnAfterSave;
        public event Action<mUIKeyStorage> OnBeforeLoad;
        public event Action<mUIKeyStorage> OnAfterLoad;

        public string Password { get; set; } = "default_password";

        private const string _loadKey = "!_mUI_load_storage_key_!";
        private readonly Dictionary<string, string> _dictionary; 

        public mUIKeyStorage()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public bool Load()
        {
            OnBeforeLoad?.Invoke(this);
            if (!PlayerPrefs.HasKey(_loadKey))
                return false;

            var db = PlayerPrefs.GetString(_loadKey);
            var decompressed = Unzip(Convert.FromBase64String(db));
            mUI.Log("mUIKeyStorage load: {0}", db);
            mUI.Log("mUIKeyStorage load decompressed: {0}", decompressed);

            using (TextReader reader = new StringReader(decompressed))
            { 
                string line = "";
                string currentKey = "";
                bool key = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (key)
                    {
                        currentKey = line;
                        key = false;
                    }
                    else
                    {
                        _dictionary.Add(currentKey, line);
                        key = true;
                        mUI.Log("mUIKeyStorage add ({0}:{1})", currentKey, line);
                    }
                }
            }

            OnAfterLoad?.Invoke(this);
            return true;
        }

        public void Save()
        {
            OnBeforeSave?.Invoke(this);

            string db = "";
            foreach (var kvp in _dictionary)
            {
                db += kvp.Key + '\n';
                db += kvp.Value.ToString().Replace('\n', ' ') + '\n';
            }

            var toSave = Convert.ToBase64String(Zip(db));
            PlayerPrefs.SetString(_loadKey, toSave);
            PlayerPrefs.Save();
            mUI.Log("mUIKeyStorage: {0}", db);
            mUI.Log("mUIKeyStorage compressed: {0}", toSave);

            OnAfterSave?.Invoke(this);
        }

        public void DeleteKey(string key)
        {
            if (_dictionary.ContainsKey(key))
                _dictionary.Remove(key);
        }

        public T GetValue<T>(string key) where T : struct
        {
            if (_dictionary.ContainsKey(key))
                return (T)Convert.ChangeType(_dictionary[key], typeof (T));
            return default(T);
        }

        public void SetValue<T>(string key, T value) where T : struct
        {
            var strValue = (string) Convert.ChangeType(value, TypeCode.String);
            if (_dictionary.ContainsKey(key))
                _dictionary[key] = strValue;
            else
                _dictionary.Add(key, strValue);
        }

        ////////////////////////////////////////////
        /////  Compress and decompress string  /////
        ////////////////////////////////////////////
        
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
