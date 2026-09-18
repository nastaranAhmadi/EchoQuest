# EchoQuest

**An Accessible Audio-Based 2D Game for Blind and Low-Vision Players**

EchoQuest is a Bachelor-level graduation project: a small, complete 2D game designed so blind and low-vision players can independently navigate, interact, and solve simple puzzles using audio, narration, and accessible visual feedback.

## Target users

- Blind players (audio-first, no reliance on vision)
- Low-vision players (large UI, high contrast, scalable text)
- Primary platform: **Android** (portrait)

## Accessibility goals

- Complete the game without vision
- Large touch targets and fixed control zones
- Directional and distance-based audio cues
- Offline narration (device TTS preferred)
- Settings for audio buses, UI/text scale, high contrast, and cue intensity
- No critical information conveyed by color alone
- No personal data collection, ads, or analytics

## Core gameplay

```text
Explore → Detect → Identify → Interact → Solve → Progress
```

Three short levels:

1. **Audio navigation** — find an object using spatial audio
2. **Object identification** — choose the correct object among several
3. **Audio puzzle** — interact with objects in a simple sequence

## Technology stack

| Item | Value |
|------|--------|
| Engine | Unity (**recommended: 6000.3 LTS / Unity 6.3**) |
| Render | Built-in 2D (orthographic) |
| Input | Unity Input System + large on-screen touch zones |
| Audio | Custom buses + stereo spatial cues |
| Narration | `INarrator` abstraction (log fallback now; Android TTS later) |
| Save | Local `PlayerPrefs` / JSON only |
| Analytics / ads | None |

> **TODO: Verify** the exact Unity Editor patch after first open in Unity Hub (scaffold targets `6000.3.23f1`).

## Project status

**Phase 3 — Menus + accessibility settings (current)**

- Main menu: New Game, Continue, Tutorial, Settings, About, Exit
- Settings: volume buses, narration, cues, contrast, UI/text scale, reset
- Local save for settings + continue progress
- Pause overlay with Resume / Settings / Main Menu
- Practice sandbox with spatial beacon still available after New Game / Tutorial

Three scripted thesis levels are **not** implemented yet.

## Screenshots

_Placeholder — add screenshots or short clips after playable builds._

| Menu | Gameplay | Settings |
|------|----------|----------|
| TBD  | TBD      | TBD      |

## How to open the project

1. Install [Unity Hub](https://unity.com/download).
2. Install **Unity 6.3 LTS** (6000.3.x) with **Android Build Support** (SDK, NDK, OpenJDK modules).
3. In Hub: **Open** → select this repository folder.
4. Allow Unity to import packages and regenerate `Library/` (not committed).
5. Open scene `Assets/Scenes/Bootstrap.unity` and press Play.

See [Documentation/MobileDeployment.md](Documentation/MobileDeployment.md) for Android device steps.

## Android build (overview)

Detailed steps live in the mobile deployment guide. High level:

1. Switch platform to **Android**.
2. Confirm package name `com.echoquest.game` and **Portrait** orientation.
3. **Build** or **Build And Run** to generate an APK on a USB-debuggable phone.

Exact SDK/NDK versions: **TODO: Verify** against the Editor you install ([Unity Android dependency versions](https://docs.unity3d.com/Manual/android-supported-dependency-versions.html)).

## Documentation

| Document | Description |
|----------|-------------|
| [Documentation/README.md](Documentation/README.md) | Full project overview |
| [Documentation/Architecture.md](Documentation/Architecture.md) | System architecture |
| [Documentation/GameDesign.md](Documentation/GameDesign.md) | Levels and loop |
| [Documentation/Accessibility.md](Documentation/Accessibility.md) | A11y design |
| [Documentation/MobileDeployment.md](Documentation/MobileDeployment.md) | Android setup & build |
| [Documentation/MobileAccessibilityTesting.md](Documentation/MobileAccessibilityTesting.md) | Device testing checklist |
| [Documentation/UserStudy.md](Documentation/UserStudy.md) | Usability evaluation protocol |
| [Documentation/Testing.md](Documentation/Testing.md) | Engineering tests |
| [Documentation/FutureWork.md](Documentation/FutureWork.md) | Out-of-scope ideas |

## Testing

- Engineering: see `Tests/` and `Documentation/Testing.md`
- Accessibility: `Documentation/MobileAccessibilityTesting.md`
- User study: `Documentation/UserStudy.md` (no fabricated results)

## Privacy

The game is **offline-first** and does **not** collect personal data. No analytics SDKs, advertising SDKs, accounts, or cloud saves are included.

## Academic note

This repository demonstrates **Design → Implementation → Evaluation** for a Bachelor thesis: accessible interaction design, a constrained MVP implementation, and a prepared user-study protocol.

## Future work

See [Documentation/FutureWork.md](Documentation/FutureWork.md) (iOS, richer TTS, controller polish, larger level set).

## License

**TODO: Verify** — add a license chosen by the project authors (e.g. MIT for code, separate asset licenses).
