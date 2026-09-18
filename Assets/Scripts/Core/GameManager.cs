using System;
using UnityEngine;
using EchoQuest.Accessibility;

namespace EchoQuest.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Playing,
        Paused,
        LevelComplete
    }

    /// <summary>
    /// High-level game state and pause ownership.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.Boot;

        public event Action<GameState, GameState> StateChanged;

        [SerializeField] private AccessibilityManager accessibility;

        private float _timeScaleBeforePause = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (accessibility == null)
            {
                accessibility = FindFirstObjectByType<AccessibilityManager>();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SetState(GameState next)
        {
            if (State == next)
            {
                return;
            }

            GameState previous = State;
            State = next;
            Debug.Log($"Game state {previous} -> {State}");
            StateChanged?.Invoke(previous, State);
        }

        public void EnterMainMenu()
        {
            Time.timeScale = 1f;
            accessibility?.StopSpeech();
            SetState(GameState.MainMenu);
        }

        public void StartPlaying(bool announceDefault = true)
        {
            Time.timeScale = 1f;
            SetState(GameState.Playing);
            if (announceDefault)
            {
                accessibility?.Announce(
                    "Game started. Listen for the beacon. Move with the pad, then press interact when near.");
            }
        }

        public void Pause()
        {
            if (State != GameState.Playing)
            {
                return;
            }

            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            SetState(GameState.Paused);
            accessibility?.Announce("Paused. Press pause again to resume.");
        }

        public void Resume()
        {
            if (State != GameState.Paused)
            {
                return;
            }

            Time.timeScale = _timeScaleBeforePause > 0f ? _timeScaleBeforePause : 1f;
            SetState(GameState.Playing);
            accessibility?.Announce("Resumed.");
        }

        public void TogglePause()
        {
            if (State == GameState.Playing)
            {
                Pause();
            }
            else if (State == GameState.Paused)
            {
                Resume();
            }
        }

        public void CompleteLevel(string message = null)
        {
            Time.timeScale = 1f;
            SetState(GameState.LevelComplete);
            if (!string.IsNullOrWhiteSpace(message))
            {
                accessibility?.Announce(message, urgent: true);
            }
            else
            {
                accessibility?.Announce("Level complete.", urgent: true);
            }
        }
    }
}
