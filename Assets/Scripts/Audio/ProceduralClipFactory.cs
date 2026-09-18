using UnityEngine;

namespace EchoQuest.Audio
{
    /// <summary>
    /// Builds simple offline tones so the project needs no binary audio assets for MVP demos.
    /// </summary>
    public static class ProceduralClipFactory
    {
        public static AudioClip CreateTone(
            string name,
            float frequencyHz,
            float durationSeconds,
            float amplitude = 0.35f,
            int sampleRate = 22050)
        {
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(durationSeconds * sampleRate));
            var samples = new float[sampleCount];
            float fadeSamples = Mathf.Min(sampleCount * 0.1f, sampleRate * 0.02f);

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float envelope = 1f;
                if (i < fadeSamples)
                {
                    envelope = i / fadeSamples;
                }
                else if (i > sampleCount - fadeSamples)
                {
                    envelope = (sampleCount - i) / fadeSamples;
                }

                samples[i] = Mathf.Sin(2f * Mathf.PI * frequencyHz * t) * amplitude * envelope;
            }

            var clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public static AudioClip CreatePulse(
            string name,
            float frequencyHz,
            float beepSeconds,
            float gapSeconds,
            int repeats,
            float amplitude = 0.3f)
        {
            int sampleRate = 22050;
            int beepSamples = Mathf.RoundToInt(beepSeconds * sampleRate);
            int gapSamples = Mathf.RoundToInt(gapSeconds * sampleRate);
            int total = Mathf.Max(1, repeats * (beepSamples + gapSamples));
            var samples = new float[total];

            for (int r = 0; r < repeats; r++)
            {
                int offset = r * (beepSamples + gapSamples);
                for (int i = 0; i < beepSamples; i++)
                {
                    float t = i / (float)sampleRate;
                    float envelope = 1f;
                    float fade = beepSamples * 0.15f;
                    if (i < fade)
                    {
                        envelope = i / fade;
                    }
                    else if (i > beepSamples - fade)
                    {
                        envelope = (beepSamples - i) / fade;
                    }

                    samples[offset + i] = Mathf.Sin(2f * Mathf.PI * frequencyHz * t) * amplitude * envelope;
                }
            }

            var clip = AudioClip.Create(name, total, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
