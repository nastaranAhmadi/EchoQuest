using UnityEngine;
using EchoQuest.Audio;
using EchoQuest.Accessibility;

namespace EchoQuest.Gameplay
{
    /// <summary>
    /// World object that emits a looping spatial cue and can be interacted with.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SpatialAudioController))]
    public sealed class AudibleInteractable : MonoBehaviour
    {
        [SerializeField] private string displayName = "Object";
        [SerializeField] private string description = "An audible object.";
        [SerializeField] private float interactRadius = 1.75f;
        [SerializeField] private AudioClip customLoopClip;
        [SerializeField] private float loopPitch = 1f;

        private SpatialAudioController _spatial;
        private AudioSource _source;
        private bool _collected;

        public string DisplayName => displayName;
        public string Description => description;
        public bool IsCollected => _collected;
        public float InteractRadius => interactRadius;

        public void Configure(string name, string desc, Transform listener, AudioClip loopClip, Color color)
        {
            displayName = name;
            description = desc;
            customLoopClip = loopClip;

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }

            EnsureComponents();
            _spatial.SetListener(listener);
            BeginLoop();
        }

        private void Awake()
        {
            EnsureComponents();
        }

        private void EnsureComponents()
        {
            _source = GetComponent<AudioSource>();
            _spatial = GetComponent<SpatialAudioController>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _source.pitch = loopPitch;
        }

        public void BeginLoop()
        {
            var audio = AudioManager.Instance;
            AudioClip clip = customLoopClip != null
                ? customLoopClip
                : audio != null ? audio.BeaconLoopClip : null;

            if (clip == null)
            {
                clip = ProceduralClipFactory.CreatePulse(displayName + "_loop", 500f, 0.1f, 0.5f, 2);
            }

            _spatial.PlayCue(clip, 1f, true);
        }

        public bool IsInRange(Vector3 playerPosition)
        {
            return Vector2.Distance(transform.position, playerPosition) <= interactRadius;
        }

        public float DistanceTo(Vector3 playerPosition)
        {
            return Vector2.Distance(transform.position, playerPosition);
        }

        public string DirectionHint(Vector3 playerPosition)
        {
            Vector2 delta = (Vector2)(transform.position - playerPosition);
            if (delta.sqrMagnitude < 0.01f)
            {
                return "here";
            }

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0f ? "to your right" : "to your left";
            }

            return delta.y > 0f ? "ahead" : "behind you";
        }

        public void Interact(AccessibilityManager accessibility, AudioManager audio)
        {
            if (_collected)
            {
                accessibility?.Announce($"The {displayName} was already collected.");
                audio?.PlayFailure();
                return;
            }

            _collected = true;
            _spatial.StopCue();
            audio?.PlaySuccess();
            accessibility?.Announce($"Collected {displayName}. {description}", urgent: true);
        }
    }
}
