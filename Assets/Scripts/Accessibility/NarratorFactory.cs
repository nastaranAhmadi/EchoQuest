using UnityEngine;

namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Creates the best available offline narrator for the current platform.
    /// </summary>
    public static class NarratorFactory
    {
        public static INarrator CreateDefault()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                var android = new AndroidNativeNarrator();
                if (android.IsAvailable)
                {
                    Debug.Log("EchoQuest: using Android native TTS narrator.");
                    return android;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"EchoQuest: Android TTS unavailable, falling back to log narrator. {ex.Message}");
            }
#endif
            Debug.Log("EchoQuest: using LogNarrator (offline fallback).");
            return new LogNarrator();
        }
    }
}
