using UnityEngine;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Core;
using EchoQuest.Gameplay;
using EchoQuest.Input;
using EchoQuest.Player;
using EchoQuest.Save;
using EchoQuest.UI;

namespace EchoQuest.Core
{
    /// <summary>
    /// Wires core services, menus, and the level campaign.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private bool createPlayerIfMissing = true;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.Portrait;

            var systems = EnsureNamedObject("GameSystems");
            var gameManager = EnsureComponentOn<GameManager>(systems);
            var audio = EnsureComponentOn<AudioManager>(systems);
            var accessibility = EnsureComponentOn<AccessibilityManager>(systems);
            EnsureComponentOn<NarrationQueue>(systems);
            var progress = EnsureComponentOn<GameProgressService>(systems);

            progress.Reload();
            progress.ApplyLoadedAccessibility(accessibility);

            var inputGo = EnsureNamedObject("InputSystems");
            var router = EnsureComponentOn<InputActionRouter>(inputGo);
            EnsureComponentOn<KeyboardInputBridge>(inputGo);
            EnsureComponentOn<GameplayInputBinder>(inputGo);

            var touch = EnsureComponentOn<TouchControlSurface>(inputGo);
            touch.Initialize(router);
            touch.SetGameplayVisible(false);

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (createPlayerIfMissing && player == null)
            {
                player = CreateSandboxPlayer();
            }

            var levels = EnsureComponentOn<LevelManager>(systems);
            var interaction = EnsureComponentOn<InteractionSystem>(systems);
            if (player != null)
            {
                levels.Initialize(player.transform, accessibility, audio, gameManager, progress, interaction);
                interaction.Initialize(player.transform, levels);
            }

            var menu = EnsureComponentOn<MenuController>(systems);
            menu.Initialize(gameManager, accessibility, progress, audio, touch, levels);

            gameManager.EnterMainMenu();

            Debug.Log("EchoQuest bootstrap ready. Phase 4–5 levels campaign.");
        }

        private static GameObject EnsureNamedObject(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null)
            {
                return existing;
            }

            return new GameObject(name);
        }

        private static T EnsureComponentOn<T>(GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return go.AddComponent<T>();
        }

        private static PlayerController CreateSandboxPlayer()
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            player.name = "Player";
            player.transform.position = Vector3.zero;
            player.transform.localScale = Vector3.one * 0.7f;

            var collider3d = player.GetComponent<Collider>();
            if (collider3d != null)
            {
                Destroy(collider3d);
            }

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }

            var renderer = player.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.2f, 0.85f, 0.75f, 1f);
            }

            return player.AddComponent<PlayerController>();
        }
    }
}
