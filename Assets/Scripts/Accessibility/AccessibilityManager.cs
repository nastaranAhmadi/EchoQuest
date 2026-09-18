using UnityEngine;
using EchoQuest.Audio;

namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Central accessibility settings and feedback routing.
    /// Gameplay should raise semantic events; this manager decides how to present them.
    /// </summary>
    public sealed class AccessibilityManager : MonoBehaviour
    {
        public static AccessibilityManager Instance { get; private set; }

        [System.Serializable]
        public sealed class Settings
        {
            [Header("Audio")]
            [Range(0f, 1f)] public float masterVolume = 1f;
            [Range(0f, 1f)] public float musicVolume = 0.35f;
            [Range(0f, 1f)] public float sfxVolume = 1f;
            [Range(0f, 1f)] public float narrationVolume = 1f;
            [Range(0f, 1f)] public float accessibilityCueVolume = 1f;

            [Header("Visual")]
            [Range(0.8f, 2f)] public float uiScale = 1.25f;
            [Range(0.8f, 2f)] public float textScale = 1.25f;
            public bool highContrast = true;
            public bool reducedVisualEffects = true;

            [Header("Gameplay")]
            public bool narrationEnabled = true;
            public bool audioCuesEnabled = true;
            public bool simplifiedInteraction = true;
        }

        public Settings Current { get; private set; } = new Settings();

        [SerializeField] private NarrationQueue narrationQueue;
        [SerializeField] private AudioManager audioManager;

        private INarrator _narrator;

        public INarrator Narrator => _narrator;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            if (narrationQueue == null)
            {
                narrationQueue = gameObject.GetComponent<NarrationQueue>();
                if (narrationQueue == null)
                {
                    narrationQueue = gameObject.AddComponent<NarrationQueue>();
                }
            }

            if (audioManager == null)
            {
                audioManager = FindFirstObjectByType<AudioManager>();
            }

            _narrator = NarratorFactory.CreateDefault();
            narrationQueue.SetNarrator(_narrator);
            ApplySettings(Current);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Announce(string message, bool urgent = false)
        {
            if (!Current.narrationEnabled || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (narrationQueue == null)
            {
                _narrator?.Speak(message);
                return;
            }

            if (urgent)
            {
                narrationQueue.SpeakNow(message);
            }
            else
            {
                narrationQueue.Enqueue(message);
            }
        }

        public void StopSpeech()
        {
            narrationQueue?.StopAll();
            _narrator?.Stop();
        }

        public void ApplySettings(Settings settings)
        {
            Current = settings ?? new Settings();

            if (audioManager == null)
            {
                audioManager = AudioManager.Instance ?? FindFirstObjectByType<AudioManager>();
            }

            if (audioManager != null)
            {
                audioManager.SetBusVolumes(
                    Current.masterVolume,
                    Current.musicVolume,
                    Current.sfxVolume,
                    Current.narrationVolume,
                    Current.accessibilityCueVolume);
                audioManager.AudioCuesEnabled = Current.audioCuesEnabled;
            }
        }

        public void ResetToDefaults()
        {
            ApplySettings(new Settings());
        }
    }
}
