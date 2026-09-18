# Accessibility

## Goals

Blind and low-vision players must be able to launch, learn, play, pause, configure, and complete EchoQuest independently on an Android phone with headphones.

## Blind players

Must be able to:

- Launch and hear that the app started
- Navigate menus with large targets + spoken labels
- Understand tutorial instructions
- Move and know approximate direction of targets
- Judge relative distance via volume
- Interact, pause, change settings, restart, finish levels

## Low-vision players

Provide:

- Large UI and scalable text
- High contrast mode
- Obvious focus/selection
- Simple layout, reduced clutter
- No color-only critical information

## Architectural rule

```text
Gameplay event → AccessibilityManager → Audio / Narration / Visual
```

Avoid gameplay code like:

```csharp
if (blindMode) { ... }
```

## Settings model

```text
Audio: Master, Music, SFX, Narration, Accessibility cues
Visual: UI scale, Text scale, High contrast, Reduced effects
Gameplay: Narration, Audio cues, Simplified interaction
```

## Narration

- Abstraction: `INarrator`
- Prefer **on-device TTS** for Android (offline)
- Current scaffold: `LogNarrator` (editor/debug)
- Document device TTS setup differences in Mobile Deployment

## Touch

- Large fixed control regions
- Minimal gesture vocabulary
- Long-press only if clearly taught

## Privacy & offline

- Core experience works offline
- No personal data collection for accessibility analytics
