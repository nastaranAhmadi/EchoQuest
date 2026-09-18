using UnityEngine;
using EchoQuest.Core;
using EchoQuest.Input;
using EchoQuest.Accessibility;

namespace EchoQuest.Player
{
    /// <summary>
    /// Simple top-down mover driven by <see cref="InputActionRouter"/> MoveInput.
    /// </summary>
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private Vector2 boundsMin = new Vector2(-4.5f, -2.5f);
        [SerializeField] private Vector2 boundsMax = new Vector2(4.5f, 4.5f);
        [SerializeField] private InputActionRouter router;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AccessibilityManager accessibility;
        [SerializeField] private float announceMoveCooldown = 0.85f;

        private float _nextAnnounceTime;
        private Vector2 _lastAnnouncedDirection;

        private void Awake()
        {
            if (router == null)
            {
                router = FindFirstObjectByType<InputActionRouter>();
            }

            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (accessibility == null)
            {
                accessibility = FindFirstObjectByType<AccessibilityManager>();
            }
        }

        private void Update()
        {
            if (router == null)
            {
                return;
            }

            if (gameManager != null && gameManager.State != GameState.Playing)
            {
                return;
            }

            Vector2 input = router.MoveInput;
            if (input.sqrMagnitude < 0.01f)
            {
                return;
            }

            Vector3 delta = new Vector3(input.x, input.y, 0f) * (moveSpeed * Time.deltaTime);
            Vector3 next = transform.position + delta;
            next.x = Mathf.Clamp(next.x, boundsMin.x, boundsMax.x);
            next.y = Mathf.Clamp(next.y, boundsMin.y, boundsMax.y);
            transform.position = next;

            MaybeAnnounceDirection(input);
        }

        private void MaybeAnnounceDirection(Vector2 input)
        {
            if (accessibility == null || Time.unscaledTime < _nextAnnounceTime)
            {
                return;
            }

            Vector2 cardinal = ToCardinal(input);
            if (cardinal == _lastAnnouncedDirection)
            {
                return;
            }

            _lastAnnouncedDirection = cardinal;
            _nextAnnounceTime = Time.unscaledTime + announceMoveCooldown;
            accessibility.Announce($"Moving {DirectionName(cardinal)}.");
        }

        private static Vector2 ToCardinal(Vector2 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return input.x > 0f ? Vector2.right : Vector2.left;
            }

            return input.y > 0f ? Vector2.up : Vector2.down;
        }

        private static string DirectionName(Vector2 cardinal)
        {
            if (cardinal == Vector2.up)
            {
                return "up";
            }

            if (cardinal == Vector2.down)
            {
                return "down";
            }

            if (cardinal == Vector2.left)
            {
                return "left";
            }

            if (cardinal == Vector2.right)
            {
                return "right";
            }

            return "forward";
        }
    }
}
