using UnityEngine;
using EchoQuest.Core;
using EchoQuest.Input;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Gameplay;

namespace EchoQuest.Input
{
    /// <summary>
    /// Routes semantic actions to game systems (pause, interact, etc.).
    /// </summary>
    public sealed class GameplayInputBinder : MonoBehaviour
    {
        [SerializeField] private InputActionRouter router;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AccessibilityManager accessibility;
        [SerializeField] private InteractionSystem interactionSystem;
        [SerializeField] private AudioManager audioManager;

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

            if (interactionSystem == null)
            {
                interactionSystem = FindFirstObjectByType<InteractionSystem>();
            }

            if (audioManager == null)
            {
                audioManager = FindFirstObjectByType<AudioManager>();
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
                    audioManager?.PlayPauseCue();
                    break;
                case GameAction.Cancel:
                case GameAction.Back:
                    if (gameManager != null && gameManager.State == GameState.Paused)
                    {
                        gameManager.Resume();
                        audioManager?.PlayUiSelect();
                    }
                    break;
                case GameAction.Interact:
                    if (interactionSystem != null)
                    {
                        interactionSystem.HandleInteract();
                    }
                    else
                    {
                        audioManager?.PlayInteract();
                        accessibility?.Announce("Interact.");
                    }
                    break;
            }
        }
    }
}
