using System.Collections.Generic;
using UnityEngine;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Core;
using EchoQuest.Player;
using EchoQuest.Save;

namespace EchoQuest.Gameplay
{
    /// <summary>
    /// Spawns and evaluates Tutorial + Levels 1–3.
    /// </summary>
    public sealed class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        private Transform _player;
        private AccessibilityManager _accessibility;
        private AudioManager _audio;
        private GameManager _gameManager;
        private GameProgressService _progress;
        private InteractionSystem _interaction;
        private readonly List<GameObject> _spawned = new List<GameObject>();
        private readonly List<string> _sequenceRemaining = new List<string>();

        private LevelId _active = LevelId.None;
        private string _targetId;
        private bool _levelActive;

        public LevelId ActiveLevel => _active;
        public bool ProximityHintsEnabled { get; private set; } = true;
        public bool CampaignMode { get; private set; }

        public void Initialize(
            Transform player,
            AccessibilityManager accessibility,
            AudioManager audio,
            GameManager gameManager,
            GameProgressService progress,
            InteractionSystem interaction)
        {
            Instance = this;
            _player = player;
            _accessibility = accessibility;
            _audio = audio;
            _gameManager = gameManager;
            _progress = progress;
            _interaction = interaction;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void StartCampaignFromBeginning()
        {
            CampaignMode = true;
            _progress.ResetProgressKeepSettings();
            StartLevel(LevelId.Tutorial);
        }

        public void StartTutorialOnly()
        {
            CampaignMode = false;
            StartLevel(LevelId.Tutorial);
        }

        public void ContinueCampaign()
        {
            CampaignMode = true;
            LevelId next = GetNextLevel(_progress.Data.highestCompletedLevel);
            if (next == LevelId.None)
            {
                _accessibility.Announce("All levels are already complete. Starting a new campaign.");
                StartCampaignFromBeginning();
                return;
            }

            StartLevel(next);
        }

        public static LevelId GetNextLevel(int highestCompletedLevel)
        {
            // highestCompletedLevel stores completed LevelId int values.
            if (highestCompletedLevel < (int)LevelId.Tutorial)
            {
                return LevelId.Tutorial;
            }

            if (highestCompletedLevel < (int)LevelId.Level1)
            {
                return LevelId.Level1;
            }

            if (highestCompletedLevel < (int)LevelId.Level2)
            {
                return LevelId.Level2;
            }

            if (highestCompletedLevel < (int)LevelId.Level3)
            {
                return LevelId.Level3;
            }

            return LevelId.None;
        }

        public void StartLevel(LevelId level)
        {
            ClearSpawned();
            _sequenceRemaining.Clear();
            _targetId = null;
            _active = level;
            _levelActive = true;
            ProximityHintsEnabled = true;
            _interaction?.ResetProximityMemory();

            if (_player != null)
            {
                _player.position = Vector3.zero;
            }

            switch (level)
            {
                case LevelId.Tutorial:
                    BuildTutorial();
                    break;
                case LevelId.Level1:
                    BuildLevel1();
                    break;
                case LevelId.Level2:
                    BuildLevel2();
                    break;
                case LevelId.Level3:
                    BuildLevel3();
                    break;
            }

            _progress.MarkGameplayStarted();
            _gameManager.StartPlaying(announceDefault: false);
            AnnounceIntro(level);
        }

        public void ClearWorld()
        {
            _levelActive = false;
            _active = LevelId.None;
            ClearSpawned();
            _sequenceRemaining.Clear();
        }

        public void HandleObjectInteracted(AudibleInteractable obj)
        {
            if (!_levelActive || obj == null)
            {
                return;
            }

            switch (_active)
            {
                case LevelId.Tutorial:
                case LevelId.Level1:
                    HandleSingleTarget(obj);
                    break;
                case LevelId.Level2:
                    HandleIdentification(obj);
                    break;
                case LevelId.Level3:
                    HandleSequence(obj);
                    break;
            }
        }

        private void HandleSingleTarget(AudibleInteractable obj)
        {
            if (obj.IsConsumed)
            {
                _audio.PlayFailure();
                _accessibility.Announce($"The {obj.DisplayName} was already used.");
                return;
            }

            obj.MarkConsumed();
            _audio.PlaySuccess();
            _accessibility.Announce($"You found the {obj.DisplayName}. Well done.", urgent: true);
            CompleteActiveLevel();
        }

        private void HandleIdentification(AudibleInteractable obj)
        {
            if (obj.IsConsumed)
            {
                _audio.PlayFailure();
                _accessibility.Announce($"The {obj.DisplayName} was already collected.");
                return;
            }

            if (obj.Id != _targetId)
            {
                _audio.PlayFailure();
                _accessibility.Announce(
                    $"That is the {obj.DisplayName}. You need the {GetDisplayName(_targetId)}.",
                    urgent: true);
                return;
            }

            obj.MarkConsumed();
            _audio.PlaySuccess();
            _accessibility.Announce($"Correct. You found the {obj.DisplayName}.", urgent: true);
            CompleteActiveLevel();
        }

        private void HandleSequence(AudibleInteractable obj)
        {
            if (_sequenceRemaining.Count == 0)
            {
                return;
            }

            string needed = _sequenceRemaining[0];
            if (obj.Id != needed)
            {
                _audio.PlayFailure();
                _accessibility.Announce(
                    $"Wrong order. Next you need the {GetDisplayName(needed)}. That was the {obj.DisplayName}.",
                    urgent: true);
                return;
            }

            if (obj.IsConsumed)
            {
                _audio.PlayFailure();
                _accessibility.Announce($"The {obj.DisplayName} was already activated.");
                return;
            }

            obj.MarkConsumed();
            _sequenceRemaining.RemoveAt(0);
            _audio.PlaySuccess();

            if (_sequenceRemaining.Count == 0)
            {
                _accessibility.Announce("Sequence complete. Puzzle solved.", urgent: true);
                CompleteActiveLevel();
            }
            else
            {
                string next = GetDisplayName(_sequenceRemaining[0]);
                _accessibility.Announce($"Good. Next, find the {next}.", urgent: true);
            }
        }

        private void CompleteActiveLevel()
        {
            _levelActive = false;
            LevelId finished = _active;
            _progress.SetHighestCompletedLevel((int)finished);

            LevelId next = CampaignMode ? GetNextAfter(finished) : LevelId.None;
            string finishedName = GetLevelTitle(finished);

            if (next != LevelId.None)
            {
                _accessibility.Announce($"{finishedName} complete. Loading the next challenge.", urgent: true);
                // Brief delay via coroutine-like invoke
                Invoke(nameof(AdvanceCampaign), 1.25f);
            }
            else
            {
                string msg = CampaignMode
                    ? "Campaign complete. You finished all EchoQuest levels."
                    : $"{finishedName} complete.";
                _gameManager.CompleteLevel(msg);
            }
        }

        private void AdvanceCampaign()
        {
            LevelId next = GetNextAfter(_active);
            if (next == LevelId.None)
            {
                ReturnToMenu();
                return;
            }

            StartLevel(next);
        }

        public void ReturnToMenu()
        {
            CancelInvoke();
            ClearWorld();
            _gameManager.EnterMainMenu();
        }

        public void ContinueAfterLevelComplete()
        {
            CancelInvoke();
            if (CampaignMode)
            {
                LevelId next = GetNextAfter(_active);
                if (next == LevelId.None)
                {
                    ReturnToMenu();
                    return;
                }

                StartLevel(next);
                return;
            }

            ReturnToMenu();
        }

        private static LevelId GetNextAfter(LevelId current)
        {
            return current switch
            {
                LevelId.Tutorial => LevelId.Level1,
                LevelId.Level1 => LevelId.Level2,
                LevelId.Level2 => LevelId.Level3,
                _ => LevelId.None
            };
        }

        private void AnnounceIntro(LevelId level)
        {
            switch (level)
            {
                case LevelId.Tutorial:
                    _accessibility.Announce(
                        "Tutorial. Use the move pad to walk. Listen for a pulsing beacon. When it is near, press interact.",
                        urgent: true);
                    break;
                case LevelId.Level1:
                    _accessibility.Announce(
                        "Level one. Audio navigation. Find the distant Echo Beacon using sound, then interact.",
                        urgent: true);
                    break;
                case LevelId.Level2:
                    _accessibility.Announce(
                        $"Level two. Object identification. There are three objects with different sounds. Find and interact with the {GetDisplayName(_targetId)}.",
                        urgent: true);
                    break;
                case LevelId.Level3:
                    _accessibility.Announce(
                        "Level three. Audio puzzle. Activate Crystal, then Lantern, then Gate, in that order.",
                        urgent: true);
                    break;
            }
        }

        private void BuildTutorial()
        {
            SpawnObject(
                "beacon",
                "Echo Beacon",
                "A nearby practice beacon.",
                new Vector3(2.2f, 1.6f, 0f),
                new Color(0.95f, 0.7f, 0.2f),
                ProceduralClipFactory.CreatePulse("tut_beacon", 520f, 0.12f, 0.5f, 2),
                1f);
            _targetId = "beacon";
        }

        private void BuildLevel1()
        {
            SpawnObject(
                "beacon",
                "Echo Beacon",
                "A distant navigation marker.",
                new Vector3(-3.5f, 3.8f, 0f),
                new Color(0.95f, 0.55f, 0.15f),
                ProceduralClipFactory.CreatePulse("l1_beacon", 480f, 0.1f, 0.65f, 2),
                1f);
            _targetId = "beacon";
        }

        private void BuildLevel2()
        {
            _targetId = "drum";
            SpawnObject(
                "bell",
                "Bell",
                "A high bright ringing sound.",
                new Vector3(-3.2f, 2.8f, 0f),
                new Color(0.9f, 0.9f, 0.3f),
                ProceduralClipFactory.CreatePulse("bell", 880f, 0.08f, 0.45f, 2),
                1.05f);
            SpawnObject(
                "drum",
                "Drum",
                "A low thumping sound.",
                new Vector3(3.4f, 1.2f, 0f),
                new Color(0.55f, 0.3f, 0.15f),
                ProceduralClipFactory.CreatePulse("drum", 160f, 0.14f, 0.4f, 2),
                0.9f);
            SpawnObject(
                "chime",
                "Chime",
                "A medium sparkling tone.",
                new Vector3(0.5f, 3.6f, 0f),
                new Color(0.4f, 0.8f, 1f),
                ProceduralClipFactory.CreatePulse("chime", 660f, 0.07f, 0.5f, 3),
                1.1f);
        }

        private void BuildLevel3()
        {
            _sequenceRemaining.AddRange(new[] { "crystal", "lantern", "gate" });
            SpawnObject(
                "crystal",
                "Crystal",
                "First object in the puzzle sequence.",
                new Vector3(-3.0f, 3.2f, 0f),
                new Color(0.7f, 0.4f, 1f),
                ProceduralClipFactory.CreatePulse("crystal", 740f, 0.1f, 0.55f, 2),
                1f);
            SpawnObject(
                "lantern",
                "Lantern",
                "Second object in the puzzle sequence.",
                new Vector3(3.2f, 2.5f, 0f),
                new Color(1f, 0.75f, 0.2f),
                ProceduralClipFactory.CreatePulse("lantern", 420f, 0.12f, 0.5f, 2),
                1f);
            SpawnObject(
                "gate",
                "Gate",
                "Final object in the puzzle sequence.",
                new Vector3(0f, -1.8f, 0f),
                new Color(0.5f, 0.55f, 0.6f),
                ProceduralClipFactory.CreatePulse("gate", 240f, 0.16f, 0.45f, 2),
                0.95f);
        }

        private void SpawnObject(
            string id,
            string name,
            string description,
            Vector3 position,
            Color color,
            AudioClip loop,
            float pitch)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.55f, 0.14f, 0.55f);

            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }

            go.AddComponent<AudioSource>();
            go.AddComponent<SpatialAudioController>();
            var interactable = go.AddComponent<AudibleInteractable>();
            interactable.Configure(id, name, description, _player, loop, color, pitch);
            _spawned.Add(go);
        }

        private void ClearSpawned()
        {
            foreach (var go in _spawned)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }

            _spawned.Clear();

            // Clean any leftover interactables from older sandboxes.
            foreach (var leftover in FindObjectsByType<AudibleInteractable>(FindObjectsSortMode.None))
            {
                if (leftover != null)
                {
                    Destroy(leftover.gameObject);
                }
            }
        }

        private static string GetDisplayName(string id)
        {
            return id switch
            {
                "beacon" => "Echo Beacon",
                "bell" => "Bell",
                "drum" => "Drum",
                "chime" => "Chime",
                "crystal" => "Crystal",
                "lantern" => "Lantern",
                "gate" => "Gate",
                _ => id
            };
        }

        private static string GetLevelTitle(LevelId id)
        {
            return id switch
            {
                LevelId.Tutorial => "Tutorial",
                LevelId.Level1 => "Level one",
                LevelId.Level2 => "Level two",
                LevelId.Level3 => "Level three",
                _ => "Level"
            };
        }
    }
}
