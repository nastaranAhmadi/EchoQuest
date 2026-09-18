using UnityEngine;

namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Android TextToSpeech via JNI. No network. Falls back silently if the engine is missing.
    /// </summary>
    public sealed class AndroidNativeNarrator : INarrator
    {
        private AndroidJavaObject _tts;
        private bool _initialized;
        private bool _available;

        public bool IsAvailable => _available;
        public bool IsSpeaking { get; private set; }

        public AndroidNativeNarrator()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, new TtsInitListener(this));
                _available = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"AndroidNativeNarrator init failed: {ex.Message}");
                _available = false;
            }
#else
            _available = false;
#endif
        }

        public void Speak(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            if (_tts == null || !_initialized)
            {
                Debug.Log($"[Narrator/Android-pending] {text}");
                return;
            }

            IsSpeaking = true;
            // QUEUE flush previous optional — use QUEUE_FLUSH = 0
            _tts.Call<int>("speak", text, 0, null, "echoquest");
            IsSpeaking = false;
#else
            Debug.Log($"[Narrator] {text}");
#endif
        }

        public void Stop()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _tts?.Call<int>("stop");
#endif
            IsSpeaking = false;
        }

        internal void OnTtsInit(int status)
        {
            // SUCCESS = 0
            _initialized = status == 0;
            _available = _initialized;
            Debug.Log(_initialized
                ? "Android TTS initialized."
                : $"Android TTS init status={status}. Install a TTS engine / language pack on device.");
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private sealed class TtsInitListener : AndroidJavaProxy
        {
            private readonly AndroidNativeNarrator _owner;

            public TtsInitListener(AndroidNativeNarrator owner)
                : base("android.speech.tts.TextToSpeech$OnInitListener")
            {
                _owner = owner;
            }

            // ReSharper disable once InconsistentNaming
            public void onInit(int status)
            {
                _owner.OnTtsInit(status);
            }
        }
#endif
    }
}
