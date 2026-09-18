# Mobile Deployment

Step-by-step guide to open EchoQuest and run it on an Android phone.

> Values marked **TODO: Verify** must be confirmed on the machine that installs Unity Hub. This repository was scaffolded without a local Unity Editor present.

## Requirements

### Hardware

- Computer that can run Unity Hub (macOS / Windows / Linux)
- Android phone for device testing
- USB cable (or wireless debugging if you already use it)
- Headphones / earbuds strongly recommended

### Software

| Component | Notes |
|-----------|--------|
| Unity Hub | Latest Hub from Unity |
| Unity Editor | **Recommended: 6000.3 LTS (Unity 6.3)** — scaffold `ProjectSettings/ProjectVersion.txt` targets `6000.3.23f1` |
| Android Build Support | Install with the Editor in Hub |
| Android SDK / NDK / OpenJDK | Install via Hub modules for that Editor |
| USB drivers | Windows may need OEM USB drivers |

**TODO: Verify** exact SDK Build-Tools / NDK / JDK versions against the Editor you install:  
https://docs.unity3d.com/Manual/android-supported-dependency-versions.html

For Unity 6.x family, Unity documentation currently lists OpenJDK **17** and NDK **r27c** for 6000.0+ lines — **confirm in Hub** for your 6000.3 patch.

## Unity setup

1. Install **Unity Hub**.
2. In Hub → **Installs → Install Editor**.
3. Select **Unity 6.3 LTS** (6000.3.x). If the exact patch `6000.3.23f1` is unavailable, install the newest **6000.3** patch and allow Unity to upgrade the project.
4. Add modules:
   - **Android Build Support**
   - **Android SDK & NDK Tools**
   - **OpenJDK**
5. Hub → **Open** → choose this repository root (the folder containing `Assets/` and `ProjectSettings/`).
6. Wait for package resolve and `Library/` generation (local only; not in Git).

## Android phone setup

1. Open **Settings → About phone**.
2. Tap **Build number** seven times to enable Developer options.
3. Open **Developer options**.
4. Enable **USB debugging**.
5. Connect the phone via USB.
6. Accept the **Allow USB debugging?** prompt on the phone.

Alternatives: wireless debugging (Android 11+), or sideload an APK with a file manager after copying it to the device.

## Unity configuration (EchoQuest scaffold)

| Setting | Scaffold value |
|---------|----------------|
| Product name | EchoQuest |
| Package name | `com.echoquest.game` |
| Orientation | **Portrait** only |
| Minimum API Level | **24** (Android 7.0) — TODO: Verify on first build |
| Target API Level | Automatic — TODO: Verify |
| Target architectures | ARM64 |
| Scripting backend (Android) | IL2CPP |
| Active Input Handling | **Input System Package** |
| Internet permission | Not forced (`ForceInternetPermission: 0`) |

### Switch platform

1. **File → Build Settings**
2. Select **Android** → **Switch Platform**

### Build Settings checklist

- Scene `Assets/Scenes/Bootstrap.unity` included
- Development Build optional for debugging
- Compression / Minify left default for early builds

## Development build (APK)

1. Connect the phone with USB debugging authorized.
2. **File → Build Settings → Build** (or **Build And Run**).
3. Choose an output folder **outside** `Library/` (for example `Builds/` — ignored by Git).
4. Install/run on device.
5. View logs:
   - Unity **Android Logcat** package (optional install), or
   - `adb logcat` from Android platform-tools

### Build And Run

**File → Build Settings → Build And Run** builds, installs, and launches in one step when the device is detected.

## Release build (high level)

1. Create a keystore **locally** (never commit `.keystore` / passwords).
2. Player Settings → Publishing Settings → assign keystore.
3. Build **APK** for direct install or **AAB** for Play Store.
4. Keep secrets out of Git (see `.gitignore`).

## Permissions & audio

- MVP should avoid unnecessary permissions (no mic/location/contacts).
- TTS may use the system speech engine; availability varies by device/language pack.
- Test with headphones; phone speakers weaken stereo direction cues.

## Troubleshooting

| Problem | Likely causes | What to try |
|---------|---------------|-------------|
| Device not detected | USB debugging off, cable, drivers, unauthorized RSA prompt | Re-plug, accept prompt, try another cable/port, install OEM drivers |
| Gradle build failure | Corrupt Gradle cache, SDK mismatch | Delete project `Library/Bee` / Gradle caches carefully; reinstall Android modules in Hub |
| SDK / NDK mismatch | Wrong Hub modules for Editor | Reinstall Android Support for the **same** Editor version |
| Black screen | Wrong scene, camera, script error on boot | Check Bootstrap scene in Build Settings; read `adb logcat` |
| No audio | Silent mode, AudioListener missing, volume buses at 0 | Unmute phone; confirm Main Camera AudioListener; check volumes |
| Touch not working | Old input managers, no UI EventSystem, zones not set up | Confirm Input System active; add touch UI in later phases |
| TTS not speaking | Language data missing, no engine, permission | Install Google/system TTS data; use LogNarrator in Editor |
| Crash on startup | IL2CPP/ABI, missing scene, exception in Awake | Development Build + logcat; ensure ARM64 |
| Poor performance | Logging spam, huge textures, too many audio sources | Use Mobile quality tier; profile on device |

## Privacy

Do not add analytics or ad SDKs for the Bachelor MVP. Core play remains offline.
