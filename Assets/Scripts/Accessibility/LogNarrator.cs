using UnityEngine;

namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Editor / fallback narrator that logs text. Replaced by platform TTS later.
    /// </summary>
    public sealed class LogNarrator : INarrator
    {
        public bool IsSpeaking { get; private set; }

        public void Speak(string text)
        {
            IsSpeaking = true;
            Debug.Log($"[Narrator] {text}");
            IsSpeaking = false;
        }

        public void Stop()
        {
            IsSpeaking = false;
        }
    }
}
