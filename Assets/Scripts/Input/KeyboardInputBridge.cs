using UnityEngine;
using UnityEngine.InputSystem;
using EchoQuest.Input;

namespace EchoQuest.Input
{
    /// <summary>
    /// Optional keyboard / gamepad bridge for Editor testing.
    /// Mobile gameplay does not require a keyboard.
    /// </summary>
    public sealed class KeyboardInputBridge : MonoBehaviour
    {
        [SerializeField] private InputActionRouter router;

        private InputAction _move;
        private InputAction _interact;
        private InputAction _pause;
        private InputAction _cancel;

        private void Awake()
        {
            if (router == null)
            {
                router = GetComponent<InputActionRouter>();
            }

            _move = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            _move.AddBinding("<Gamepad>/leftStick");

            _interact = new InputAction("Interact", InputActionType.Button);
            _interact.AddBinding("<Keyboard>/e");
            _interact.AddBinding("<Keyboard>/space");
            _interact.AddBinding("<Gamepad>/buttonSouth");

            _pause = new InputAction("Pause", InputActionType.Button);
            _pause.AddBinding("<Keyboard>/escape");
            _pause.AddBinding("<Keyboard>/p");
            _pause.AddBinding("<Gamepad>/start");

            _cancel = new InputAction("Cancel", InputActionType.Button);
            _cancel.AddBinding("<Keyboard>/backspace");
            _cancel.AddBinding("<Gamepad>/buttonEast");
        }

        private void OnEnable()
        {
            _move.Enable();
            _interact.Enable();
            _pause.Enable();
            _cancel.Enable();

            _interact.performed += OnInteract;
            _pause.performed += OnPause;
            _cancel.performed += OnCancel;
        }

        private void OnDisable()
        {
            _interact.performed -= OnInteract;
            _pause.performed -= OnPause;
            _cancel.performed -= OnCancel;

            _move.Disable();
            _interact.Disable();
            _pause.Disable();
            _cancel.Disable();
        }

        private void OnDestroy()
        {
            _move?.Dispose();
            _interact?.Dispose();
            _pause?.Dispose();
            _cancel?.Dispose();
        }

        private void Update()
        {
            if (router == null)
            {
                return;
            }

            // Touch holds take priority when non-zero so keyboard doesn't fight the pad.
            if (router.MoveInput.sqrMagnitude > 0.01f)
            {
                return;
            }

            router.SetMoveInput(_move.ReadValue<Vector2>());
        }

        private void OnInteract(InputAction.CallbackContext _)
        {
            router?.Trigger(GameAction.Interact);
        }

        private void OnPause(InputAction.CallbackContext _)
        {
            router?.Trigger(GameAction.Pause);
        }

        private void OnCancel(InputAction.CallbackContext _)
        {
            router?.Trigger(GameAction.Cancel);
        }
    }
}
