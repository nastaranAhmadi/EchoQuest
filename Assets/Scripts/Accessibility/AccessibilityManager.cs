using System;
using UnityEngine;

namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Central accessibility settings and feedback routing.
    /// Gameplay should raise semantic events; this manager decides how to present them.
    /// </summary>
    public sealed class AccessibilityManager : MonoBehaviour
    {
        [Serializable]
        public sealed class Settings
        {
            [Header("Audio")]
            [Range(0f, 1f)] public float masterVolume = 1f;
            [Range(0f, 1f)] public float musicVolume = 0.6f;
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

        private INarrator _narrator = new LogNarrator();

        public INarrator Narrator
        {
            get => _narrator;
            set => _narrator = value ?? new LogNarrator();
        }

        public void Announce(string message)
        {
            if (!Current.narrationEnabled || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _narrator.Speak(message);
        }

        public void ApplySettings(Settings settings)
        {
            Current = settings ?? new Settings();
            // Volume / visual application is implemented in later phases.
        }

        public void ResetToDefaults()
        {
            Current = new Settings();
        }
    }
}
