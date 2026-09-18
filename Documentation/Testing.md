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

## Known issues (Phase 0)

- Unity Editor not yet opened on the scaffold machine — package versions may resolve/upgrade on first import
- Narration is log-only (`LogNarrator`)
- No gameplay levels yet
- Touch control surface not yet implemented
