using UnityEngine;

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
    /// High-level game state. Scene flow and level control build on this later.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        public GameState State { get; private set; } = GameState.Boot;

        public void SetState(GameState next)
        {
            State = next;
            Debug.Log($"Game state -> {State}");
        }
    }
}
