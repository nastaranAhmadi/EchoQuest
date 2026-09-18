using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace EchoQuest.UI
{
    /// <summary>
    /// Shared helpers for large, high-contrast UI controls.
    /// </summary>
    public static class AccessibleUiFactory
    {
        public static readonly Color PanelDark = new Color(0.06f, 0.07f, 0.1f, 0.96f);
        public static readonly Color PanelHighContrast = new Color(0f, 0f, 0f, 0.98f);
        public static readonly Color ButtonNormal = new Color(0.16f, 0.42f, 0.52f, 1f);
        public static readonly Color ButtonHighContrast = new Color(1f, 1f, 0.15f, 1f);
        public static readonly Color ButtonPressed = new Color(0.3f, 0.75f, 0.55f, 1f);
        public static readonly Color TextLight = Color.white;
        public static readonly Color TextOnHighContrast = Color.black;

        public static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
        }

        public static Canvas CreateOverlayCanvas(string name, int sortingOrder)
        {
            EnsureEventSystem();
            var canvasGo = new GameObject(name);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvasGo.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static RectTransform CreateFullPanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            StretchFull(rect);
            var image = go.AddComponent<Image>();
            image.color = color;
            return rect;
        }

        public static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static Text CreateText(Transform parent, string content, int fontSize, Color color, TextAnchor align = TextAnchor.MiddleCenter)
        {
            var go = new GameObject("Label", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            StretchFull(go.GetComponent<RectTransform>());

            var text = go.AddComponent<Text>();
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = align;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(18, fontSize / 3);
            text.resizeTextMaxSize = fontSize;
            text.raycastTarget = false;
            text.font = ResolveFont();
            return text;
        }

        public static Button CreateLargeButton(
            Transform parent,
            string name,
            string label,
            UnityAction onClick,
            Color buttonColor,
            Color labelColor,
            int fontSize = 48)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = buttonColor;

            var button = go.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = buttonColor * 1.08f;
            colors.pressedColor = ButtonPressed;
            colors.selectedColor = ButtonPressed;
            colors.disabledColor = new Color(0.25f, 0.25f, 0.25f, 0.7f);
            button.colors = colors;
            button.targetGraphic = image;
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            CreateText(go.transform, label, fontSize, labelColor);

            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 140f;
            le.preferredHeight = 160f;
            le.flexibleWidth = 1f;
            return button;
        }

        public static Slider CreateLabeledSlider(
            Transform parent,
            string label,
            float min,
            float max,
            float value,
            UnityAction<float> onChanged,
            Color labelColor,
            int fontSize)
        {
            var row = new GameObject(label + "Row", typeof(RectTransform));
            row.transform.SetParent(parent, false);
            var rowLayout = row.AddComponent<VerticalLayoutGroup>();
            rowLayout.spacing = 8f;
            rowLayout.childForceExpandHeight = false;
            rowLayout.childForceExpandWidth = true;
            rowLayout.padding = new RectOffset(8, 8, 4, 4);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(row.transform, false);
            var labelText = labelGo.AddComponent<Text>();
            labelText.text = $"{label}: {value:0.00}";
            labelText.fontSize = fontSize;
            labelText.color = labelColor;
            labelText.alignment = TextAnchor.MiddleLeft;
            labelText.font = ResolveFont();
            labelText.raycastTarget = false;
            var labelLe = labelGo.AddComponent<LayoutElement>();
            labelLe.minHeight = 48f;
            labelLe.preferredHeight = 56f;

            var sliderGo = new GameObject("Slider", typeof(RectTransform));
            sliderGo.transform.SetParent(row.transform, false);
            var sliderLe = sliderGo.AddComponent<LayoutElement>();
            sliderLe.minHeight = 72f;
            sliderLe.preferredHeight = 80f;

            var bg = new GameObject("Background", typeof(RectTransform));
            bg.transform.SetParent(sliderGo.transform, false);
            StretchFull(bg.GetComponent<RectTransform>());
            var bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.22f, 0.26f, 1f);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGo.transform, false);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(12f, 0f);
            fillAreaRect.offsetMax = new Vector2(-12f, 0f);

            var fill = new GameObject("Fill", typeof(RectTransform));
            fill.transform.SetParent(fillArea.transform, false);
            StretchFull(fill.GetComponent<RectTransform>());
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.35f, 0.75f, 0.7f, 1f);

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderGo.transform, false);
            StretchFull(handleArea.GetComponent<RectTransform>());
            var handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.offsetMin = new Vector2(12f, 0f);
            handleAreaRect.offsetMax = new Vector2(-12f, 0f);

            var handle = new GameObject("Handle", typeof(RectTransform));
            handle.transform.SetParent(handleArea.transform, false);
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(48f, 48f);
            var handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;

            var slider = sliderGo.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = false;
            slider.value = value;
            slider.onValueChanged.AddListener(v =>
            {
                labelText.text = $"{label}: {v:0.00}";
                onChanged?.Invoke(v);
            });

            var rowLe = row.AddComponent<LayoutElement>();
            rowLe.minHeight = 140f;
            rowLe.preferredHeight = 150f;
            return slider;
        }

        public static Toggle CreateLargeToggle(
            Transform parent,
            string label,
            bool isOn,
            UnityAction<bool> onChanged,
            Color labelColor,
            int fontSize)
        {
            var go = new GameObject(label + "Toggle", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.16f, 0.2f, 1f);

            var box = new GameObject("Box", typeof(RectTransform));
            box.transform.SetParent(go.transform, false);
            var boxLe = box.AddComponent<LayoutElement>();
            boxLe.minWidth = 72f;
            boxLe.minHeight = 72f;
            boxLe.preferredWidth = 72f;
            boxLe.preferredHeight = 72f;
            var boxImage = box.AddComponent<Image>();
            boxImage.color = new Color(0.25f, 0.28f, 0.32f, 1f);

            var check = new GameObject("Checkmark", typeof(RectTransform));
            check.transform.SetParent(box.transform, false);
            StretchFull(check.GetComponent<RectTransform>());
            var checkImage = check.AddComponent<Image>();
            checkImage.color = ButtonPressed;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var text = labelGo.AddComponent<Text>();
            text.text = label;
            text.fontSize = fontSize;
            text.color = labelColor;
            text.alignment = TextAnchor.MiddleLeft;
            text.font = ResolveFont();
            text.raycastTarget = false;

            var toggle = go.AddComponent<Toggle>();
            toggle.targetGraphic = boxImage;
            toggle.graphic = checkImage;
            toggle.isOn = isOn;
            toggle.onValueChanged.AddListener(v => onChanged?.Invoke(v));

            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 120f;
            le.preferredHeight = 130f;
            return toggle;
        }

        public static Font ResolveFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }
    }
}
