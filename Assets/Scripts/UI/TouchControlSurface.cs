using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using EchoQuest.Input;

namespace EchoQuest.UI
{
    /// <summary>
    /// Portrait-first accessible touch layout with large fixed zones.
    /// Built at runtime so the Bootstrap scene stays simple.
    /// </summary>
    public sealed class TouchControlSurface : MonoBehaviour
    {
        [SerializeField] private float controlPanelHeightNormalized = 0.38f;
        [SerializeField] private Color panelColor = new Color(0.05f, 0.06f, 0.08f, 0.92f);
        [SerializeField] private Color buttonColor = new Color(0.18f, 0.45f, 0.55f, 1f);
        [SerializeField] private Color buttonPressedColor = new Color(0.35f, 0.75f, 0.55f, 1f);

        private InputActionRouter _router;
        private Canvas _canvas;

        public void Initialize(InputActionRouter router)
        {
            _router = router;
            EnsureEventSystem();
            BuildUi();
        }

        public void SetGameplayVisible(bool visible)
        {
            if (_canvas != null)
            {
                _canvas.gameObject.SetActive(visible);
            }

            if (!visible)
            {
                _router?.ClearMoveInput();
            }
        }

        private void OnDestroy()
        {
            if (_canvas != null)
            {
                Destroy(_canvas.gameObject);
            }
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<InputSystemUIInputModule>();
        }

        private void BuildUi()
        {
            var canvasGo = new GameObject("TouchControlCanvas");
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = CreatePanel(canvasGo.transform);
            CreateMovePad(panel.transform);
            CreateActionColumn(panel.transform);
        }

        private RectTransform CreatePanel(Transform parent)
        {
            var panelGo = new GameObject("ControlPanel", typeof(RectTransform));
            panelGo.transform.SetParent(parent, false);

            var rect = panelGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, controlPanelHeightNormalized);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = panelGo.AddComponent<Image>();
            image.color = panelColor;
            image.raycastTarget = true;

            var layout = panelGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 24, 24);
            layout.spacing = 24f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            return rect;
        }

        private void CreateMovePad(Transform parent)
        {
            var pad = CreateContainer(parent, "MovePad", 1.4f);
            var grid = pad.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            grid.spacing = new Vector2(12f, 12f);
            grid.padding = new RectOffset(8, 8, 8, 8);
            grid.cellSize = new Vector2(140f, 140f);
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;

            // 3x3: empty, up, empty / left, idle label, right / empty, down, empty
            CreateSpacer(pad);
            CreateHoldMoveButton(pad, "Up", "UP", Vector2.up);
            CreateSpacer(pad);
            CreateHoldMoveButton(pad, "Left", "LEFT", Vector2.left);
            CreateCenterLabel(pad, "MOVE");
            CreateHoldMoveButton(pad, "Right", "RIGHT", Vector2.right);
            CreateSpacer(pad);
            CreateHoldMoveButton(pad, "Down", "DOWN", Vector2.down);
            CreateSpacer(pad);
        }

        private void CreateActionColumn(Transform parent)
        {
            var column = CreateContainer(parent, "ActionColumn", 1f);
            var layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20f;
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateTapButton(column, "Interact", "INTERACT", GameAction.Interact);
            CreateTapButton(column, "Pause", "PAUSE", GameAction.Pause);
        }

        private RectTransform CreateContainer(Transform parent, string name, float flexibleWidth)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.flexibleWidth = flexibleWidth;
            le.flexibleHeight = 1f;
            return go.GetComponent<RectTransform>();
        }

        private void CreateSpacer(Transform parent)
        {
            var go = new GameObject("Spacer", typeof(RectTransform));
            go.transform.SetParent(parent, false);
        }

        private void CreateCenterLabel(Transform parent, string text)
        {
            var go = new GameObject("MoveLabel", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.12f, 0.14f, 0.18f, 1f);
            var label = CreateText(go.transform, text, 36);
            label.color = Color.white;
        }

        private void CreateHoldMoveButton(Transform parent, string name, string label, Vector2 direction)
        {
            var button = CreateBaseButton(parent, name, label);
            var hold = button.gameObject.AddComponent<HoldMoveButton>();
            hold.Initialize(_router, direction, button, buttonColor, buttonPressedColor);
        }

        private void CreateTapButton(Transform parent, string name, string label, GameAction action)
        {
            var button = CreateBaseButton(parent, name, label);
            button.onClick.AddListener(() =>
            {
                if (_router != null)
                {
                    _router.Trigger(action);
                }
            });
        }

        private Button CreateBaseButton(Transform parent, string name, string label)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = buttonColor;

            var button = go.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = buttonColor * 1.1f;
            colors.pressedColor = buttonPressedColor;
            colors.selectedColor = buttonPressedColor;
            colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
            button.colors = colors;
            button.targetGraphic = image;

            var text = CreateText(go.transform, label, 42);
            text.color = Color.white;

            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 160f;
            le.minWidth = 160f;
            le.flexibleHeight = 1f;
            le.flexibleWidth = 1f;

            return button;
        }

        private static Text CreateText(Transform parent, string content, int fontSize)
        {
            var go = new GameObject("Label", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var text = go.AddComponent<Text>();
            text.text = content;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 24;
            text.resizeTextMaxSize = fontSize;
            text.raycastTarget = false;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return text;
        }
    }

    /// <summary>
    /// Hold-to-move control for large directional pads.
    /// </summary>
    public sealed class HoldMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private InputActionRouter _router;
        private Vector2 _direction;
        private Button _button;
        private Color _normal;
        private Color _pressed;
        private bool _held;

        public void Initialize(
            InputActionRouter router,
            Vector2 direction,
            Button button,
            Color normal,
            Color pressed)
        {
            _router = router;
            _direction = direction;
            _button = button;
            _normal = normal;
            _pressed = pressed;
            _button.transition = Selectable.Transition.None;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _held = true;
            ApplyVisual(true);
            ApplyMove();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Release();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_held)
            {
                Release();
            }
        }

        private void OnDisable()
        {
            if (_held)
            {
                Release();
            }
        }

        private void ApplyMove()
        {
            if (_router == null)
            {
                return;
            }

            // Combine with any existing orthogonal hold by replacing with this direction.
            // Multi-direction chord support can be added later if needed.
            _router.SetMoveInput(_direction);
        }

        private void Release()
        {
            _held = false;
            ApplyVisual(false);
            _router?.ClearMoveInput();
        }

        private void ApplyVisual(bool pressed)
        {
            if (_button != null && _button.targetGraphic != null)
            {
                _button.targetGraphic.color = pressed ? _pressed : _normal;
            }
        }
    }
}
