using UnityEngine;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Core;
using EchoQuest.Input;
using EchoQuest.Player;
using EchoQuest.UI;

namespace EchoQuest.Core
{
    /// <summary>
    /// Wires core services and a playable sandbox for Phase 1.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private bool autoStartPlaying = true;
        [SerializeField] private bool createPlayerIfMissing = true;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.Portrait;

            var gameManager = EnsureComponentOnNewOrExisting<GameManager>("GameSystems");
            EnsureComponentOn<AccessibilityManager>(gameManager.gameObject);
            EnsureComponentOn<AudioManager>(gameManager.gameObject);

            var inputGo = EnsureNamedObject("InputSystems");
            var router = EnsureComponentOn<InputActionRouter>(inputGo);
            EnsureComponentOn<KeyboardInputBridge>(inputGo);
            EnsureComponentOn<GameplayInputBinder>(inputGo);

            var touch = EnsureComponentOn<TouchControlSurface>(inputGo);
            touch.Initialize(router);

            if (createPlayerIfMissing && FindFirstObjectByType<PlayerController>() == null)
            {
                CreateSandboxPlayer();
            }

            if (autoStartPlaying)
            {
                gameManager.StartPlaying();
            }
            else
            {
                gameManager.EnterMainMenu();
            }

            FindFirstObjectByType<AccessibilityManager>()?.Announce(
                "EchoQuest ready. Use the move pad and interact button.");

            Debug.Log("EchoQuest bootstrap ready. Phase 1 input + movement.");
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

        private static T EnsureComponentOnNewOrExisting<T>(string objectName) where T : Component
        {
            var existing = FindFirstObjectByType<T>();
            if (existing != null)
            {
                return existing;
            }

            var go = EnsureNamedObject(objectName);
            return go.AddComponent<T>();
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

        private static void CreateSandboxPlayer()
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            player.name = "Player";
            player.transform.position = Vector3.zero;

            // 2D-friendly: remove 3D collider physics noise for the sandbox.
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

            player.AddComponent<PlayerController>();
        }
    }
}
