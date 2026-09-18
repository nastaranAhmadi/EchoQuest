using UnityEngine;
using EchoQuest.Core;
using EchoQuest.Input;
using EchoQuest.Accessibility;

namespace EchoQuest.Input
{
    /// <summary>
    /// Routes semantic actions to game systems (pause, interact hooks, etc.).
    /// </summary>
    public sealed class GameplayInputBinder : MonoBehaviour
    {
        [SerializeField] private InputActionRouter router;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AccessibilityManager accessibility;

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

        private void OnEnable()
        {
            if (router != null)
            {
                router.ActionTriggered += OnAction;
            }
        }

        private void OnDisable()
        {
            if (router != null)
            {
                router.ActionTriggered -= OnAction;
            }
        }

        private void OnAction(GameAction action)
        {
            switch (action)
            {
                case GameAction.Pause:
                    gameManager?.TogglePause();
                    break;
                case GameAction.Cancel:
                case GameAction.Back:
                    if (gameManager != null && gameManager.State == GameState.Paused)
                    {
                        gameManager.Resume();
                    }
                    break;
                case GameAction.Interact:
                    accessibility?.Announce("Interact.");
                    break;
            }
        }
    }
}
