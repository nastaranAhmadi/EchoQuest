using UnityEngine;
using UnityEngine.UI;
using EchoQuest.Accessibility;
using EchoQuest.Audio;
using EchoQuest.Core;
using EchoQuest.Gameplay;
using EchoQuest.Save;

namespace EchoQuest.UI
{
    /// <summary>
    /// Accessible main menu, settings, about, pause, and level-complete overlays.
    /// </summary>
    public sealed class MenuController : MonoBehaviour
    {
        private GameManager _gameManager;
        private AccessibilityManager _accessibility;
        private GameProgressService _progress;
        private AudioManager _audio;
        private TouchControlSurface _touch;
        private LevelManager _levels;

        private Canvas _canvas;
        private GameObject _mainPanel;
        private GameObject _settingsPanel;
        private GameObject _aboutPanel;
        private GameObject _pausePanel;
        private GameObject _levelCompletePanel;
        private Button _continueButton;
        private bool _openSettingsAfterRebuild;

        private bool _highContrast;
        private float _uiScale = 1.25f;
        private float _textScale = 1.25f;

        public void Initialize(
            GameManager gameManager,
            AccessibilityManager accessibility,
            GameProgressService progress,
            AudioManager audio,
            TouchControlSurface touch,
            LevelManager levels)
        {
            _gameManager = gameManager;
            _accessibility = accessibility;
            _progress = progress;
            _audio = audio;
            _touch = touch;
            _levels = levels;

            ApplyVisualFromSettings(accessibility.Current);
            Build();
            _gameManager.StateChanged += OnStateChanged;
            OnStateChanged(GameState.Boot, _gameManager.State);
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
            {
                _gameManager.StateChanged -= OnStateChanged;
            }

            if (_canvas != null)
            {
                Destroy(_canvas.gameObject);
            }
        }

        private void ApplyVisualFromSettings(AccessibilityManager.Settings settings)
        {
            _highContrast = settings.highContrast;
            _uiScale = settings.uiScale;
            _textScale = settings.textScale;
        }

        private Color PanelColor => _highContrast ? AccessibleUiFactory.PanelHighContrast : AccessibleUiFactory.PanelDark;
        private Color ButtonColor => _highContrast ? AccessibleUiFactory.ButtonHighContrast : AccessibleUiFactory.ButtonNormal;
        private Color LabelColor => _highContrast ? AccessibleUiFactory.TextOnHighContrast : AccessibleUiFactory.TextLight;
        private int TitleSize => Mathf.RoundToInt(64f * _textScale);
        private int BodySize => Mathf.RoundToInt(42f * _textScale);
        private int ButtonFont => Mathf.RoundToInt(48f * _textScale);

        private void Build()
        {
            _canvas = AccessibleUiFactory.CreateOverlayCanvas("MenuCanvas", 200);
            _canvas.transform.localScale = Vector3.one * Mathf.Clamp(_uiScale, 0.8f, 1.6f);

            _mainPanel = CreateMainPanel();
            _settingsPanel = CreateSettingsPanel();
            _aboutPanel = CreateAboutPanel();
            _pausePanel = CreatePausePanel();
            _levelCompletePanel = CreateLevelCompletePanel();

            ShowOnly(null);
        }

        private GameObject CreateMainPanel()
        {
            var panel = AccessibleUiFactory.CreateFullPanel(_canvas.transform, "MainMenu", PanelColor);
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 80, 80);
            layout.spacing = 28f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var title = new GameObject("Title", typeof(RectTransform));
            title.transform.SetParent(panel, false);
            var titleLe = title.AddComponent<LayoutElement>();
            titleLe.minHeight = 120f;
            titleLe.preferredHeight = 140f;
            AccessibleUiFactory.CreateText(title.transform, "EchoQuest", TitleSize, LabelColor);

            var subtitle = new GameObject("Subtitle", typeof(RectTransform));
            subtitle.transform.SetParent(panel, false);
            var subLe = subtitle.AddComponent<LayoutElement>();
            subLe.minHeight = 80f;
            AccessibleUiFactory.CreateText(
                subtitle.transform,
                "An accessible audio adventure",
                BodySize,
                LabelColor);

            AccessibleUiFactory.CreateLargeButton(panel, "NewGame", "New Game", OnNewGame, ButtonColor, LabelColor, ButtonFont);
            _continueButton = AccessibleUiFactory.CreateLargeButton(panel, "Continue", "Continue", OnContinue, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "Tutorial", "Tutorial", OnTutorial, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "Settings", "Settings", OnOpenSettings, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "About", "About", OnOpenAbout, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "Exit", "Exit", OnExit, ButtonColor, LabelColor, ButtonFont);

            RefreshContinueButton();
            return panel.gameObject;
        }

        private GameObject CreateSettingsPanel()
        {
            var panel = AccessibleUiFactory.CreateFullPanel(_canvas.transform, "Settings", PanelColor);

            var outer = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            outer.padding = new RectOffset(36, 36, 48, 48);
            outer.spacing = 16f;
            outer.childAlignment = TextAnchor.UpperCenter;
            outer.childControlHeight = true;
            outer.childControlWidth = true;
            outer.childForceExpandWidth = true;
            outer.childForceExpandHeight = false;

            var title = new GameObject("SettingsTitle", typeof(RectTransform));
            title.transform.SetParent(panel, false);
            title.AddComponent<LayoutElement>().minHeight = 90f;
            AccessibleUiFactory.CreateText(title.transform, "Settings", TitleSize, LabelColor);

            var scrollGo = new GameObject("Scroll", typeof(RectTransform));
            scrollGo.transform.SetParent(panel, false);
            var scrollLe = scrollGo.AddComponent<LayoutElement>();
            scrollLe.flexibleHeight = 1f;
            scrollLe.minHeight = 900f;

            var scroll = scrollGo.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            var viewport = new GameObject("Viewport", typeof(RectTransform));
            viewport.transform.SetParent(scrollGo.transform, false);
            AccessibleUiFactory.StretchFull(viewport.GetComponent<RectTransform>());
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.02f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            scroll.viewport = viewport.GetComponent<RectTransform>();

            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0f, 1600f);
            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(8, 8, 8, 8);
            contentLayout.spacing = 12f;
            contentLayout.childControlHeight = true;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = contentRect;

            var s = _accessibility.Current;
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "Master Volume", 0f, 1f, s.masterVolume, v =>
            {
                s.masterVolume = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "Music Volume", 0f, 1f, s.musicVolume, v =>
            {
                s.musicVolume = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "SFX Volume", 0f, 1f, s.sfxVolume, v =>
            {
                s.sfxVolume = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "Narration Volume", 0f, 1f, s.narrationVolume, v =>
            {
                s.narrationVolume = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "Accessibility Cues", 0f, 1f, s.accessibilityCueVolume, v =>
            {
                s.accessibilityCueVolume = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "UI Scale", 0.8f, 1.6f, s.uiScale, v =>
            {
                s.uiScale = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLabeledSlider(content.transform, "Text Scale", 0.8f, 1.8f, s.textScale, v =>
            {
                s.textScale = v;
                CommitSettings(s, announce: false);
            }, LabelColor, BodySize);

            AccessibleUiFactory.CreateLargeToggle(content.transform, "Narration", s.narrationEnabled, v =>
            {
                s.narrationEnabled = v;
                CommitSettings(s, announce: true);
                _accessibility.Announce(v ? "Narration enabled." : "Narration disabled.", urgent: true);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLargeToggle(content.transform, "Audio Cues", s.audioCuesEnabled, v =>
            {
                s.audioCuesEnabled = v;
                CommitSettings(s, announce: true);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLargeToggle(content.transform, "High Contrast", s.highContrast, v =>
            {
                s.highContrast = v;
                CommitSettings(s, announce: true);
                RebuildForTheme();
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLargeToggle(content.transform, "Reduced Visual Effects", s.reducedVisualEffects, v =>
            {
                s.reducedVisualEffects = v;
                CommitSettings(s, announce: true);
            }, LabelColor, BodySize);
            AccessibleUiFactory.CreateLargeToggle(content.transform, "Simplified Interaction", s.simplifiedInteraction, v =>
            {
                s.simplifiedInteraction = v;
                CommitSettings(s, announce: true);
            }, LabelColor, BodySize);

            AccessibleUiFactory.CreateLargeButton(content.transform, "ResetSettings", "Reset Settings", () =>
            {
                _accessibility.ResetToDefaults();
                _progress.SaveAccessibility(_accessibility.Current);
                _audio?.PlayUiSelect();
                _accessibility.Announce("Settings reset to defaults. Re-open settings to refresh controls.", urgent: true);
                RebuildForTheme();
            }, ButtonColor, LabelColor, ButtonFont);

            AccessibleUiFactory.CreateLargeButton(panel, "BackSettings", "Back", OnBackFromSettings, ButtonColor, LabelColor, ButtonFont);
            return panel.gameObject;
        }

        private GameObject CreateAboutPanel()
        {
            var panel = AccessibleUiFactory.CreateFullPanel(_canvas.transform, "About", PanelColor);
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 80, 80);
            layout.spacing = 24f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;

            var title = new GameObject("AboutTitle", typeof(RectTransform));
            title.transform.SetParent(panel, false);
            title.AddComponent<LayoutElement>().minHeight = 100f;
            AccessibleUiFactory.CreateText(title.transform, "About EchoQuest", TitleSize, LabelColor);

            var body = new GameObject("AboutBody", typeof(RectTransform));
            body.transform.SetParent(panel, false);
            body.AddComponent<LayoutElement>().minHeight = 600f;
            AccessibleUiFactory.CreateText(
                body.transform,
                "Bachelor project: an accessible audio-based 2D game for blind and low-vision players.\n\n" +
                "No personal data is collected. Core gameplay works offline.\n\n" +
                "Use headphones for the best directional audio.",
                BodySize,
                LabelColor,
                TextAnchor.UpperLeft);

            AccessibleUiFactory.CreateLargeButton(panel, "BackAbout", "Back", OnBackFromAbout, ButtonColor, LabelColor, ButtonFont);
            return panel.gameObject;
        }

        private GameObject CreatePausePanel()
        {
            var panel = AccessibleUiFactory.CreateFullPanel(_canvas.transform, "Pause", new Color(0f, 0f, 0f, 0.82f));
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 200, 80);
            layout.spacing = 28f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;

            var title = new GameObject("PauseTitle", typeof(RectTransform));
            title.transform.SetParent(panel, false);
            title.AddComponent<LayoutElement>().minHeight = 100f;
            AccessibleUiFactory.CreateText(title.transform, "Paused", TitleSize, Color.white);

            AccessibleUiFactory.CreateLargeButton(panel, "Resume", "Resume", () =>
            {
                _audio?.PlayUiSelect();
                _gameManager.Resume();
            }, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "PauseSettings", "Settings", OnOpenSettings, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "PauseMainMenu", "Main Menu", () =>
            {
                _audio?.PlayUiSelect();
                _levels?.ReturnToMenu();
            }, ButtonColor, LabelColor, ButtonFont);

            return panel.gameObject;
        }

        private GameObject CreateLevelCompletePanel()
        {
            var panel = AccessibleUiFactory.CreateFullPanel(_canvas.transform, "LevelComplete", PanelColor);
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 200, 80);
            layout.spacing = 28f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;

            var title = new GameObject("CompleteTitle", typeof(RectTransform));
            title.transform.SetParent(panel, false);
            title.AddComponent<LayoutElement>().minHeight = 120f;
            AccessibleUiFactory.CreateText(title.transform, "Level Complete", TitleSize, LabelColor);

            AccessibleUiFactory.CreateLargeButton(panel, "CompleteContinue", "Continue", () =>
            {
                _audio?.PlayUiSelect();
                _levels?.ContinueAfterLevelComplete();
            }, ButtonColor, LabelColor, ButtonFont);
            AccessibleUiFactory.CreateLargeButton(panel, "CompleteMenu", "Main Menu", () =>
            {
                _audio?.PlayUiSelect();
                _levels?.ReturnToMenu();
            }, ButtonColor, LabelColor, ButtonFont);

            return panel.gameObject;
        }

        private void RebuildForTheme()
        {
            _openSettingsAfterRebuild = _settingsPanel != null && _settingsPanel.activeSelf;
            ApplyVisualFromSettings(_accessibility.Current);
            if (_canvas != null)
            {
                Destroy(_canvas.gameObject);
                _canvas = null;
            }

            Build();
            OnStateChanged(GameState.Boot, _gameManager.State);
            if (_openSettingsAfterRebuild)
            {
                ShowOnly(_settingsPanel);
                _openSettingsAfterRebuild = false;
            }
        }

        private void CommitSettings(AccessibilityManager.Settings settings, bool announce)
        {
            _accessibility.ApplySettings(settings);
            _progress.SaveAccessibility(settings);
            ApplyVisualFromSettings(settings);
            if (_canvas != null)
            {
                _canvas.transform.localScale = Vector3.one * Mathf.Clamp(_uiScale, 0.8f, 1.6f);
            }

            if (announce)
            {
                _audio?.PlayUiSelect();
            }
        }

        private void RefreshContinueButton()
        {
            if (_continueButton != null)
            {
                _continueButton.interactable = _progress != null && _progress.CanContinue;
            }
        }

        private void OnStateChanged(GameState previous, GameState next)
        {
            RefreshContinueButton();
            switch (next)
            {
                case GameState.MainMenu:
                    _levels?.ClearWorld();
                    _touch?.SetGameplayVisible(false);
                    ShowOnly(_mainPanel);
                    _accessibility?.Announce("Main menu. Choose New Game, Continue, Tutorial, Settings, About, or Exit.", urgent: true);
                    break;
                case GameState.Playing:
                    _touch?.SetGameplayVisible(true);
                    ShowOnly(null);
                    break;
                case GameState.Paused:
                    _touch?.SetGameplayVisible(true);
                    ShowOnly(_pausePanel);
                    break;
                case GameState.LevelComplete:
                    _touch?.SetGameplayVisible(false);
                    ShowOnly(_levelCompletePanel);
                    break;
                default:
                    ShowOnly(null);
                    break;
            }
        }

        private void ShowOnly(GameObject panel)
        {
            SetActive(_mainPanel, panel == _mainPanel);
            SetActive(_settingsPanel, panel == _settingsPanel);
            SetActive(_aboutPanel, panel == _aboutPanel);
            SetActive(_pausePanel, panel == _pausePanel);
            SetActive(_levelCompletePanel, panel == _levelCompletePanel);
        }

        private static void SetActive(GameObject go, bool active)
        {
            if (go != null)
            {
                go.SetActive(active);
            }
        }

        private void OnNewGame()
        {
            _audio?.PlayUiSelect();
            _accessibility.Announce("Starting a new campaign from the tutorial.", urgent: true);
            _levels.StartCampaignFromBeginning();
        }

        private void OnContinue()
        {
            if (_progress == null || !_progress.CanContinue)
            {
                _accessibility.Announce("No saved progress to continue.");
                _audio?.PlayFailure();
                return;
            }

            _audio?.PlayUiSelect();
            _accessibility.Announce("Continuing your campaign.", urgent: true);
            _levels.ContinueCampaign();
        }

        private void OnTutorial()
        {
            _audio?.PlayUiSelect();
            _levels.StartTutorialOnly();
        }

        private void OnOpenSettings()
        {
            _audio?.PlayUiSelect();
            ShowOnly(_settingsPanel);
            _accessibility.Announce("Settings. Adjust volumes, narration, and visual accessibility.", urgent: true);
        }

        private void OnOpenAbout()
        {
            _audio?.PlayUiSelect();
            ShowOnly(_aboutPanel);
            _accessibility.Announce(
                "About EchoQuest. Bachelor project for blind and low-vision players. Offline. No personal data collected.",
                urgent: true);
        }

        private void OnBackFromSettings()
        {
            _audio?.PlayUiSelect();
            if (_gameManager.State == GameState.Paused)
            {
                ShowOnly(_pausePanel);
                _accessibility.Announce("Paused.");
            }
            else
            {
                ShowOnly(_mainPanel);
                _accessibility.Announce("Main menu.");
            }
        }

        private void OnBackFromAbout()
        {
            _audio?.PlayUiSelect();
            ShowOnly(_mainPanel);
            _accessibility.Announce("Main menu.");
        }

        private void OnExit()
        {
            _accessibility.Announce("Exiting EchoQuest.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
