using UnityEngine;

namespace EchoQuest.Audio
{
    /// <summary>
    /// Owns Music / SFX / Voice / Accessibility audio buses.
    /// Full mixer wiring arrives in the audio phase.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.6f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float voiceVolume = 1f;
        [Range(0f, 1f)] public float accessibilityCueVolume = 1f;

        public void SetBusVolumes(float master, float music, float sfx, float voice, float accessibility)
        {
            masterVolume = Mathf.Clamp01(master);
            musicVolume = Mathf.Clamp01(music);
            sfxVolume = Mathf.Clamp01(sfx);
            voiceVolume = Mathf.Clamp01(voice);
            accessibilityCueVolume = Mathf.Clamp01(accessibility);
            AudioListener.volume = masterVolume;
        }
    }
}
