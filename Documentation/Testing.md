# Testing

## Engineering tests

Folder: `Tests/` (Unity Test Framework package is listed in `Packages/manifest.json`).

### Planned coverage

| Area | Examples |
|------|----------|
| Save | Load default, save progress, clear |
| Accessibility settings | Reset defaults, announce when enabled/disabled |
| Spatial audio | Volume falls with distance; pan sign follows X |
| Level logic | Level 2 target selection; Level 3 sequence order |

Automated tests will be added as systems stabilize (Phases 4–6).

## Manual playtests

1. Editor Play Mode on Bootstrap / level scenes
2. Android Development Build on at least one physical device
3. Accessibility checklist: [MobileAccessibilityTesting.md](MobileAccessibilityTesting.md)

## Definition of done (MVP)

- Blindfolded or vision-blocked pass of Levels 1–3 with headphones
- Settings persist after app restart
- No internet required for core loop
- APK installs and launches on a mid-range Android phone

## Known issues

- Unity Editor may adjust package versions / `.meta` files on first import
- In Editor, narration uses `LogNarrator` (Console). On Android builds, native TTS is attempted
- Changing High Contrast rebuilds the menu UI (brief refresh)
- UI Scale applies to the menu canvas; gameplay touch pad uses a fixed layout for large targets
- Tutorial currently opens the practice sandbox (full tutorial level comes later)
- Three thesis levels not implemented yet
- Procedural beeps are placeholders, not final sound design
