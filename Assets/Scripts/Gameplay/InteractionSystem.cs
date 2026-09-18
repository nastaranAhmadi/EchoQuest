using UnityEngine;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Core;

namespace EchoQuest.Gameplay
{
    /// <summary>
    /// Finds the nearest audible object and handles Interact.
    /// </summary>
    public sealed class InteractionSystem : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float scanRadius = 12f;
        [SerializeField] private float proximityAnnounceCooldown = 2.5f;

        private AccessibilityManager _accessibility;
        private AudioManager _audio;
        private GameManager _gameManager;
        private float _nextProximityAnnounce;
        private AudibleInteractable _lastAnnounced;

        public void Initialize(Transform playerTransform)
        {
            player = playerTransform;
            _accessibility = AccessibilityManager.Instance ?? FindFirstObjectByType<AccessibilityManager>();
            _audio = AudioManager.Instance ?? FindFirstObjectByType<AudioManager>();
            _gameManager = GameManager.Instance ?? FindFirstObjectByType<GameManager>();
        }

        private void Update()
        {
            if (player == null || _gameManager == null || _gameManager.State != GameState.Playing)
            {
                return;
            }

            if (Time.unscaledTime < _nextProximityAnnounce)
            {
                return;
            }

            AudibleInteractable nearest = FindNearest(includeCollected: false);
            if (nearest == null)
            {
                return;
            }

            float distance = nearest.DistanceTo(player.position);
            if (distance > scanRadius)
            {
                return;
            }

            // Soft proximity narration when entering a closer band or new target.
            if (nearest != _lastAnnounced || distance < 2.5f)
            {
                _lastAnnounced = nearest;
                _nextProximityAnnounce = Time.unscaledTime + proximityAnnounceCooldown;
                string nearFar = distance < 2f ? "very near" : distance < 4f ? "near" : "in the distance";
                _accessibility?.Announce(
                    $"{nearest.DisplayName} is {nearFar}, {nearest.DirectionHint(player.position)}.");
            }
        }

        public void HandleInteract()
        {
            if (player == null)
            {
                return;
            }

            if (_gameManager != null && _gameManager.State != GameState.Playing)
            {
                _accessibility?.Announce("Cannot interact while paused.");
                return;
            }

            AudibleInteractable nearest = FindNearest(includeCollected: true);
            if (nearest == null)
            {
                _audio?.PlayFailure();
                _accessibility?.Announce("Nothing nearby to interact with.");
                return;
            }

            if (!nearest.IsInRange(player.position))
            {
                _audio?.PlayFailure();
                _accessibility?.Announce(
                    $"{nearest.DisplayName} is too far. It is {nearest.DirectionHint(player.position)}.");
                return;
            }

            _audio?.PlayInteract();
            nearest.Interact(_accessibility, _audio);
        }

        private AudibleInteractable FindNearest(bool includeCollected)
        {
            var all = FindObjectsByType<AudibleInteractable>(FindObjectsSortMode.None);
            AudibleInteractable best = null;
            float bestDistance = float.MaxValue;

            foreach (var item in all)
            {
                if (!includeCollected && item.IsCollected)
                {
                    continue;
                }

                float d = item.DistanceTo(player.position);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = item;
                }
            }

            return best;
        }
    }
}
