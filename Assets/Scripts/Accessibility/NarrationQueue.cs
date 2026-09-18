using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Serializes narration so overlapping Speak calls do not stomp each other.
    /// Critical messages can clear the queue and speak immediately.
    /// </summary>
    public sealed class NarrationQueue : MonoBehaviour
    {
        [SerializeField] private float estimatedCharsPerSecond = 14f;
        [SerializeField] private float minimumUtteranceSeconds = 0.6f;
        [SerializeField] private float maximumUtteranceSeconds = 6f;

        private readonly Queue<string> _queue = new Queue<string>();
        private INarrator _narrator = new LogNarrator();
        private float _speakingUntil;
        private string _current;

        public bool IsSpeaking => Time.unscaledTime < _speakingUntil || (_narrator != null && _narrator.IsSpeaking);
        public string CurrentText => _current;

        public void SetNarrator(INarrator narrator)
        {
            _narrator = narrator ?? new LogNarrator();
        }

        public void Enqueue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            _queue.Enqueue(text.Trim());
            TrySpeakNext();
        }

        public void SpeakNow(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            _queue.Clear();
            _narrator?.Stop();
            _speakingUntil = 0f;
            BeginUtterance(text.Trim());
        }

        public void StopAll()
        {
            _queue.Clear();
            _narrator?.Stop();
            _current = null;
            _speakingUntil = 0f;
        }

        private void Update()
        {
            if (!IsSpeaking)
            {
                _current = null;
                TrySpeakNext();
            }
        }

        private void TrySpeakNext()
        {
            if (IsSpeaking || _queue.Count == 0)
            {
                return;
            }

            BeginUtterance(_queue.Dequeue());
        }

        private void BeginUtterance(string text)
        {
            _current = text;
            float duration = Mathf.Clamp(
                text.Length / Mathf.Max(1f, estimatedCharsPerSecond),
                minimumUtteranceSeconds,
                maximumUtteranceSeconds);

            // Native TTS may finish earlier/later; queue still advances by estimate unless IsSpeaking stays true.
            _speakingUntil = Time.unscaledTime + duration;
            _narrator?.Speak(text);
        }
    }
}
