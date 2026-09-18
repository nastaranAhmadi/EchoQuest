using System;
using UnityEngine;
using EchoQuest.Audio;

namespace EchoQuest.Gameplay
{
    /// <summary>
    /// World object that emits a looping spatial cue and can be interacted with.
    /// Level logic decides success/failure; this component reports the interaction.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SpatialAudioController))]
    public sealed class AudibleInteractable : MonoBehaviour
    {
        [SerializeField] private string id = "object";
        [SerializeField] private string displayName = "Object";
        [SerializeField] private string description = "An audible object.";
        [SerializeField] private float interactRadius = 1.75f;
        [SerializeField] private AudioClip customLoopClip;
        [SerializeField] private float loopPitch = 1f;

        private SpatialAudioController _spatial;
        private AudioSource _source;
        private bool _consumed;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public bool IsConsumed => _consumed;
        public float InteractRadius => interactRadius;

        public event Action<AudibleInteractable> Interacted;

        public void Configure(
            string objectId,
            string name,
            string desc,
            Transform listener,
            AudioClip loopClip,
            Color color,
            float pitch = 1f)
        {
            id = objectId;
            displayName = name;
            description = desc;
            customLoopClip = loopClip;
            loopPitch = pitch;
            _consumed = false;

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }

            EnsureComponents();
            _source.pitch = loopPitch;
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

        public void StopLoop()
        {
            _spatial?.StopCue();
        }

        public void MarkConsumed()
        {
            _consumed = true;
            StopLoop();
        }

        public void ResetConsumed()
        {
            _consumed = false;
            BeginLoop();
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

        /// <summary>
        /// Called by InteractionSystem when the player interacts in range.
        /// </summary>
        public void NotifyInteract()
        {
            Interacted?.Invoke(this);
        }
    }
}
