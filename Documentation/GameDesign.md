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

### Level 1 — Audio navigation

**Teach:** movement, left/right audio, distance, discovery, interact.  
**Goal:** Find one specific sounding object and interact with it.

### Level 2 — Object identification

**Teach:** distinguishing multiple audio identities.  
**Goal:** Find the narrated target among 3–4 objects.

### Level 3 — Audio puzzle

**Teach:** memory + sequence.  
**Goal:** Interact with three objects in a fixed order.

Keep puzzles short enough for a Bachelor evaluation session (~5–15 minutes total once familiar).

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
