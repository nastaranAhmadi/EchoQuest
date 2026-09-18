# Game Design

## Concept

EchoQuest is a short audio-first exploration game. The player moves through a compact 2D space, locates objects by sound, and solves simple interaction tasks.

## Core loop

```text
Explore → Detect → Identify → Interact → Solve → Progress
```

## Touch layout (portrait)

```text
┌─────────────────────────┐
│                         │
│        GAME AREA        │
│   (listen / orient)     │
│                         │
├─────────────┬───────────┤
│             │ Interact  │
│  MOVE ZONE  │ Pause     │
│  (large)    │           │
└─────────────┴───────────┘
```

Design rules:

- No precision tapping of tiny world objects for core actions
- Prefer tap / hold of large zones over complex gestures
- Every critical action has an audio confirmation

## Levels (MVP)

### Tutorial

Nearby beacon. Teach move pad + interact.

### Level 1 — Audio navigation

Farther beacon. Goal: find and interact using directional/distance audio.

### Level 2 — Object identification

Three objects (Bell, Drum, Chime) with distinct pitches. Goal: find the **Drum**.

### Level 3 — Audio puzzle

Crystal → Lantern → Gate in order. Wrong order gives spoken feedback.

Campaign flow: **New Game** runs Tutorial through Level 3 automatically between levels.

## Feedback language

| Event | Feedback |
|-------|----------|
| Move step | Soft footstep / tick (optional) |
| Object nearer | Louder / clearer cue |
| Wrong interact | Distinct failure sound + short narration |
| Correct | Success sound + narration |
| Level complete | Fanfare cue + next-step narration |

## Visual layer (low vision)

- High-contrast optional palette
- Large UI chrome
- Clear focus/selection state
- Color never sole channel for meaning

## Out of scope for MVP

- Branching story
- Combat
- Multiplayer
- Procedural worlds
