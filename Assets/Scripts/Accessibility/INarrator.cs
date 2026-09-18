namespace EchoQuest.Accessibility
{
    /// <summary>
    /// Platform-agnostic narration. Gameplay must depend on this interface only.
    /// </summary>
    public interface INarrator
    {
        void Speak(string text);
        void Stop();
        bool IsSpeaking { get; }
    }
}
