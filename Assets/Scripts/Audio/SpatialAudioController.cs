using UnityEngine;

namespace EchoQuest.Audio
{
    /// <summary>
    /// Applies stereo pan and distance attenuation for world objects.
    /// Uses custom stereo (spatialBlend = 0) for predictable mobile headphone cues.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class SpatialAudioController : MonoBehaviour
    {
        [SerializeField] private Transform listener;
        [SerializeField] private float maxHearableDistance = 10f;
        [SerializeField] private float fullVolumeDistance = 1.25f;
        [SerializeField] private bool useAccessibilityBus = true;

        private AudioSource _source;
        private AudioManager _audioManager;
        private float _baseVolume = 1f;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.spatialBlend = 0f;
            _source.loop = true;
            _source.playOnAwake = false;
            _audioManager = AudioManager.Instance;
        }

        private void LateUpdate()
        {
            if (listener == null || _source == null || !_source.isPlaying)
            {
                return;
            }

            if (_audioManager != null && !_audioManager.AudioCuesEnabled)
            {
                _source.volume = 0f;
                return;
            }

            Vector2 toSource = (Vector2)(transform.position - listener.position);
            float distance = toSource.magnitude;
            float distanceVolume = 0f;

            if (distance <= fullVolumeDistance)
            {
                distanceVolume = 1f;
            }
            else if (distance < maxHearableDistance)
            {
                distanceVolume = 1f - Mathf.InverseLerp(fullVolumeDistance, maxHearableDistance, distance);
            }

            float bus = 1f;
            if (_audioManager != null)
            {
                bus = useAccessibilityBus
                    ? _audioManager.GetScaledAccessibilityVolume()
                    : _audioManager.GetScaledSfxVolume();
            }

            _source.volume = distanceVolume * _baseVolume * bus;
            float panRange = Mathf.Max(maxHearableDistance * 0.65f, 0.01f);
            _source.panStereo = Mathf.Clamp(toSource.x / panRange, -1f, 1f);
        }

        public void SetListener(Transform listenerTransform)
        {
            listener = listenerTransform;
        }

        public void ConfigureDistances(float fullVolume, float maxHearable)
        {
            fullVolumeDistance = Mathf.Max(0.1f, fullVolume);
            maxHearableDistance = Mathf.Max(fullVolumeDistance + 0.1f, maxHearable);
        }

        public void PlayCue(AudioClip clip, float baseVolume = 1f, bool loop = true)
        {
            if (clip == null)
            {
                return;
            }

            _baseVolume = Mathf.Clamp01(baseVolume);
            _source.clip = clip;
            _source.loop = loop;
            if (!_source.isPlaying)
            {
                _source.Play();
            }
        }

        public void StopCue()
        {
            if (_source != null)
            {
                _source.Stop();
            }
        }

        public float CurrentDistanceToListener()
        {
            if (listener == null)
            {
                return float.MaxValue;
            }

            return Vector2.Distance(transform.position, listener.position);
        }

        public float CurrentPan()
        {
            return _source != null ? _source.panStereo : 0f;
        }
    }
}
