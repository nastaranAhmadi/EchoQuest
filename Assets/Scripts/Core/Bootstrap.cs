using UnityEngine;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Core;
using EchoQuest.Gameplay;
using EchoQuest.Input;
using EchoQuest.Player;
using EchoQuest.UI;

namespace EchoQuest.Core
{
    /// <summary>
    /// Wires core services and a playable audio sandbox for Phase 2.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private bool autoStartPlaying = true;
        [SerializeField] private bool createPlayerIfMissing = true;
        [SerializeField] private bool createBeaconIfMissing = true;

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

            // Re-apply after AudioManager exists.
            accessibility.ApplySettings(accessibility.Current);

            var inputGo = EnsureNamedObject("InputSystems");
            var router = EnsureComponentOn<InputActionRouter>(inputGo);
            EnsureComponentOn<KeyboardInputBridge>(inputGo);
            EnsureComponentOn<GameplayInputBinder>(inputGo);

            var touch = EnsureComponentOn<TouchControlSurface>(inputGo);
            touch.Initialize(router);

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (createPlayerIfMissing && player == null)
            {
                player = CreateSandboxPlayer();
            }

            var interaction = EnsureComponentOn<InteractionSystem>(systems);
            if (player != null)
            {
                interaction.Initialize(player.transform);
            }

            if (createBeaconIfMissing && FindFirstObjectByType<AudibleInteractable>() == null && player != null)
            {
                CreateBeacon(player.transform, audio);
            }

            if (autoStartPlaying)
            {
                gameManager.StartPlaying();
            }
            else
            {
                gameManager.EnterMainMenu();
            }

            accessibility.Announce(
                "EchoQuest ready. Listen for the beacon. Move with the pad, then press interact when near.",
                urgent: true);

            Debug.Log("EchoQuest bootstrap ready. Phase 2 audio + narration.");
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

        private static void CreateBeacon(Transform listener, AudioManager audio)
        {
            var beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beacon.name = "Beacon";
            beacon.transform.position = new Vector3(3.2f, 2.4f, 0f);
            beacon.transform.localScale = new Vector3(0.6f, 0.15f, 0.6f);

            var collider3d = beacon.GetComponent<Collider>();
            if (collider3d != null)
            {
                Destroy(collider3d);
            }

            var rb = beacon.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }

            beacon.AddComponent<AudioSource>();
            beacon.AddComponent<SpatialAudioController>();
            var interactable = beacon.AddComponent<AudibleInteractable>();

            AudioClip loop = audio != null
                ? audio.BeaconLoopClip
                : ProceduralClipFactory.CreatePulse("beacon", 520f, 0.12f, 0.55f, 2);

            interactable.Configure(
                "Echo Beacon",
                "A pulsing sound marker used to practice audio navigation.",
                listener,
                loop,
                new Color(0.95f, 0.7f, 0.2f, 1f));
        }
    }
}
