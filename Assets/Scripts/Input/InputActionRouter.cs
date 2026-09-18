using System;
using UnityEngine;

namespace EchoQuest.Input
{
    /// <summary>
    /// Device-agnostic input hub. Touch UI and Input System both raise actions here.
    /// Gameplay listens to this instead of reading raw touch or keys.
    /// </summary>
    public sealed class InputActionRouter : MonoBehaviour
    {
        public static InputActionRouter Instance { get; private set; }

        /// <summary>Current held move direction in UI space: x = right, y = up. Magnitude 0..1.</summary>
        public Vector2 MoveInput { get; private set; }

        public event Action<GameAction> ActionTriggered;
        public event Action<Vector2> MoveInputChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SetMoveInput(Vector2 input)
        {
            Vector2 clamped = Vector2.ClampMagnitude(input, 1f);
            if (clamped == MoveInput)
            {
                return;
            }

            MoveInput = clamped;
            MoveInputChanged?.Invoke(MoveInput);
        }

        public void ClearMoveInput()
        {
            SetMoveInput(Vector2.zero);
        }

        public void Trigger(GameAction action)
        {
            if (action == GameAction.None)
            {
                return;
            }

            ActionTriggered?.Invoke(action);
        }
    }
}
