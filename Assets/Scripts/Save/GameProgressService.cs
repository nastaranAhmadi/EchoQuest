using UnityEngine;
using EchoQuest.Accessibility;

namespace EchoQuest.Save
{
    /// <summary>
    /// Loads/saves progress and accessibility preferences through <see cref="LocalSaveService"/>.
    /// </summary>
    public sealed class GameProgressService : MonoBehaviour
    {
        public static GameProgressService Instance { get; private set; }

        private readonly LocalSaveService _save = new LocalSaveService();
        private LocalSaveService.SaveData _data = new LocalSaveService.SaveData();

        public LocalSaveService.SaveData Data => _data;
        public bool CanContinue => _data.hasGameplayProgress;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            Reload();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Reload()
        {
            _data = _save.Load() ?? new LocalSaveService.SaveData();
        }

        public void ApplyLoadedAccessibility(AccessibilityManager accessibility)
        {
            if (accessibility == null)
            {
                return;
            }

            accessibility.ApplySettings(LocalSaveService.ReadAccessibility(_data));
        }

        public void SaveAccessibility(AccessibilityManager.Settings settings)
        {
            LocalSaveService.WriteAccessibility(_data, settings);
            _save.Save(_data);
        }

        public void MarkGameplayStarted()
        {
            _data.hasGameplayProgress = true;
            _save.Save(_data);
        }

        public void SetHighestCompletedLevel(int levelIndex)
        {
            _data.highestCompletedLevel = Mathf.Max(_data.highestCompletedLevel, levelIndex);
            _data.hasGameplayProgress = true;
            _save.Save(_data);
        }

        public void ResetProgressKeepSettings()
        {
            var settingsJson = _data.accessibilityJson;
            _data = new LocalSaveService.SaveData
            {
                accessibilityJson = settingsJson,
                hasGameplayProgress = false,
                highestCompletedLevel = 0
            };
            _save.Save(_data);
        }

        public void ResetAll()
        {
            _save.Clear();
            _data = new LocalSaveService.SaveData();
        }
    }
}
