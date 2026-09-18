using System;
using UnityEngine;
using EchoQuest.Accessibility;

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
            public bool hasSave;
            public bool hasGameplayProgress;
            public int highestCompletedLevel;
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

        public static AccessibilityManager.Settings ReadAccessibility(SaveData data)
        {
            if (data == null || string.IsNullOrEmpty(data.accessibilityJson))
            {
                return new AccessibilityManager.Settings();
            }

            try
            {
                var settings = JsonUtility.FromJson<AccessibilityManager.Settings>(data.accessibilityJson);
                return settings ?? new AccessibilityManager.Settings();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse accessibility settings: {ex.Message}");
                return new AccessibilityManager.Settings();
            }
        }

        public static void WriteAccessibility(SaveData data, AccessibilityManager.Settings settings)
        {
            if (data == null)
            {
                return;
            }

            data.accessibilityJson = JsonUtility.ToJson(settings ?? new AccessibilityManager.Settings());
        }
    }
}
