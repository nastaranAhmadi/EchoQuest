using UnityEngine;

namespace EchoQuest.Audio
{
    /// <summary>
    /// Owns Music / SFX / Voice / Accessibility audio buses and one-shot playback.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.35f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float voiceVolume = 1f;
        [Range(0f, 1f)] public float accessibilityCueVolume = 1f;

        public bool AudioCuesEnabled { get; set; } = true;

        private AudioSource _musicSource;
        private AudioSource _sfxSource;
        private AudioSource _voiceSource;
        private AudioSource _a11ySource;

        public AudioClip InteractClip { get; private set; }
        public AudioClip SuccessClip { get; private set; }
        public AudioClip FailureClip { get; private set; }
        public AudioClip PauseClip { get; private set; }
        public AudioClip BeaconLoopClip { get; private set; }
        public AudioClip UiSelectClip { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            BuildSources();
            BuildDefaultClips();
            ApplyListenerVolume();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SetBusVolumes(float master, float music, float sfx, float voice, float accessibility)
        {
            masterVolume = Mathf.Clamp01(master);
            musicVolume = Mathf.Clamp01(music);
            sfxVolume = Mathf.Clamp01(sfx);
            voiceVolume = Mathf.Clamp01(voice);
            accessibilityCueVolume = Mathf.Clamp01(accessibility);
            ApplyListenerVolume();

            if (_musicSource != null)
            {
                _musicSource.volume = musicVolume;
            }
        }

        public float GetScaledSfxVolume()
        {
            return masterVolume * sfxVolume;
        }

        public float GetScaledAccessibilityVolume()
        {
            return masterVolume * accessibilityCueVolume;
        }

        public void PlaySfx(AudioClip clip, float volumeScale = 1f)
        {
            if (!AudioCuesEnabled || clip == null || _sfxSource == null)
            {
                return;
            }

            _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * sfxVolume);
        }

        public void PlayAccessibilityCue(AudioClip clip, float volumeScale = 1f)
        {
            if (!AudioCuesEnabled || clip == null || _a11ySource == null)
            {
                return;
            }

            _a11ySource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * accessibilityCueVolume);
        }

        public void PlayInteract()
        {
            PlaySfx(InteractClip);
        }

        public void PlaySuccess()
        {
            PlayAccessibilityCue(SuccessClip);
        }

        public void PlayFailure()
        {
            PlayAccessibilityCue(FailureClip);
        }

        public void PlayPauseCue()
        {
            PlayAccessibilityCue(PauseClip, 0.8f);
        }

        public void PlayUiSelect()
        {
            PlaySfx(UiSelectClip, 0.7f);
        }

        private void BuildSources()
        {
            _musicSource = CreateChildSource("MusicBus", false);
            _sfxSource = CreateChildSource("SfxBus", false);
            _voiceSource = CreateChildSource("VoiceBus", false);
            _a11ySource = CreateChildSource("AccessibilityBus", false);

            _musicSource.loop = true;
            _musicSource.volume = musicVolume;
            _sfxSource.volume = 1f;
            _voiceSource.volume = voiceVolume;
            _a11ySource.volume = 1f;
        }

        private AudioSource CreateChildSource(string name, bool playOnAwake)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = playOnAwake;
            source.spatialBlend = 0f;
            source.loop = false;
            return source;
        }

        private void BuildDefaultClips()
        {
            InteractClip = ProceduralClipFactory.CreateTone("interact", 660f, 0.08f, 0.4f);
            SuccessClip = ProceduralClipFactory.CreatePulse("success", 880f, 0.07f, 0.05f, 3, 0.35f);
            FailureClip = ProceduralClipFactory.CreateTone("failure", 180f, 0.2f, 0.4f);
            PauseClip = ProceduralClipFactory.CreateTone("pause", 420f, 0.12f, 0.3f);
            BeaconLoopClip = ProceduralClipFactory.CreatePulse("beacon", 520f, 0.12f, 0.55f, 2, 0.28f);
            UiSelectClip = ProceduralClipFactory.CreateTone("ui_select", 740f, 0.05f, 0.25f);
        }

        private void ApplyListenerVolume()
        {
            AudioListener.volume = masterVolume;
        }
    }
}
