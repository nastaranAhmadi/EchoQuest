# Mobile Accessibility Testing

Use this checklist on a real Android device with headphones. Mark each item Pass / Fail / N/A and note the device model and Android version.

**Tester role:** Blind / Low-vision / Sighted proxy  
**Device:** ____________________  
**Android version:** ____________________  
**Build type:** Dev APK / Release  
**Date:** ____________________  

## Blind user (audio-first)

| # | Question | Result | Notes |
|---|----------|--------|-------|
| 1 | Can launch the game and hear that it started? | | |
| 2 | Can navigate the main menu? | | |
| 3 | Can understand the tutorial? | | |
| 4 | Can move intentionally? | | |
| 5 | Can detect that objects exist nearby? | | |
| 6 | Can determine approximate direction (left/right)? | | |
| 7 | Can determine approximate distance (near/far)? | | |
| 8 | Can interact with the intended object? | | |
| 9 | Can solve Level 2 identification? | | |
| 10 | Can solve Level 3 sequence puzzle? | | |
| 11 | Can pause and resume? | | |
| 12 | Can change accessibility / audio settings? | | |
| 13 | Can restart or continue progress? | | |
| 14 | Can complete the MVP campaign? | | |

## Low-vision user

| # | Question | Result | Notes |
|---|----------|--------|-------|
| 1 | Is text large enough (or scalable)? | | |
| 2 | Is high contrast usable? | | |
| 3 | Does UI scaling help without breaking layout? | | |
| 4 | Are interactive targets visually clear? | | |
| 5 | Are objects distinguishable without relying on color alone? | | |
| 6 | Is visual clutter low enough to focus? | | |

## Audio conditions

| Condition | Result | Notes |
|-----------|--------|-------|
| Headphones stereo | | |
| Phone speaker | | |
| Left/right balance understandable | | |
| Narration + SFX together | | |
| Volume buses independently useful | | |
| Noisy environment (optional) | | |

## Severity guide

- **Blocker:** cannot proceed without vision or external help
- **Major:** can proceed but with repeated errors / frustration
- **Minor:** polish or preference issue

Record blockers with steps to reproduce before changing the build.
