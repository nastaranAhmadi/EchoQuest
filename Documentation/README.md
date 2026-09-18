# EchoQuest Documentation

This folder contains academic and engineering documentation for **EchoQuest**, an accessible audio-based 2D Android game for blind and low-vision players.

## Contents

| File | Purpose |
|------|---------|
| [Architecture.md](Architecture.md) | Components, data flow, mobile & accessibility architecture |
| [GameDesign.md](GameDesign.md) | Gameplay loop, three levels, touch layout |
| [Accessibility.md](Accessibility.md) | Accessibility requirements and design rules |
| [MobileDeployment.md](MobileDeployment.md) | Install Unity, build APK, run on Android |
| [MobileAccessibilityTesting.md](MobileAccessibilityTesting.md) | Blind / low-vision / audio checklists |
| [UserStudy.md](UserStudy.md) | Bachelor usability evaluation protocol |
| [Testing.md](Testing.md) | Engineering and playtest notes |
| [FutureWork.md](FutureWork.md) | Post-MVP ideas |

## Quick start

1. Read the root [README.md](../README.md).
2. Install Unity **6.3 LTS** (see Mobile Deployment).
3. Open this repository in Unity Hub.
4. Open `Assets/Scenes/Bootstrap.unity`.

## Project principles

- **Offline-first** core gameplay
- **No personal data collection**
- **Accessibility through architecture**, not scattered `if (blindMode)` checks
- **Bachelor MVP scope** — small, complete, evaluable
