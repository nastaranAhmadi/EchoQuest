using UnityEngine;

namespace EchoQuest.Audio
{
    /// <summary>
    /// Applies stereo pan and distance attenuation for world objects.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class SpatialAudioController : MonoBehaviour
    {
        [SerializeField] private Transform listener;
        [SerializeField] private float maxHearableDistance = 12f;
        [SerializeField] private float fullVolumeDistance = 1.5f;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.spatialBlend = 0f; // Custom stereo pan for reliable mobile headphones.
            _source.loop = true;
            _source.playOnAwake = false;
        }

        private void LateUpdate()
        {
            if (listener == null || _source == null)
            {
                return;
            }

            Vector2 toSource = (Vector2)(transform.position - listener.position);
            float distance = toSource.magnitude;
            float volume = 0f;

            if (distance <= fullVolumeDistance)
            {
                volume = 1f;
            }
            else if (distance < maxHearableDistance)
            {
                volume = 1f - Mathf.InverseLerp(fullVolumeDistance, maxHearableDistance, distance);
            }

            _source.volume = volume;
            _source.panStereo = Mathf.Clamp(toSource.x / Mathf.Max(maxHearableDistance, 0.01f), -1f, 1f);
        }

        public void SetListener(Transform listenerTransform)
        {
            listener = listenerTransform;
        }

        public void PlayCue(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            _source.clip = clip;
            if (!_source.isPlaying)
            {
                _source.Play();
            }
        }

        public void StopCue()
        {
            _source.Stop();
        }
    }
}
