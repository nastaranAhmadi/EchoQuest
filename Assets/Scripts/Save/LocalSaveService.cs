using System;
using UnityEngine;

namespace EchoQuest.Save
{
    /// <summary>
    /// Local-only persistence. No cloud, accounts, or analytics.
    /// </summary>
    public sealed class LocalSaveService
    {
        private const string Key = "echoquest.save.v1";

        [Serializable]
        public sealed class SaveData
        {
            public int highestCompletedLevel;
            public bool hasSave;
            public string accessibilityJson;
        }

        public SaveData Load()
        {
            if (!PlayerPrefs.HasKey(Key))
            {
                return new SaveData();
            }

            try
            {
                return JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key)) ?? new SaveData();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to load save data: {ex.Message}");
                return new SaveData();
            }
        }

        public void Save(SaveData data)
        {
            if (data == null)
            {
                return;
            }

            data.hasSave = true;
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }
}
