# Physics Master: Draw & Solve

Unity 6 LTS Android puzzle game foundation. Current version includes a modern animated laboratory menu, 50 data-driven levels, chapter colors, freehand/line/circle/box/pin tools, undo, clear, pause, hints, scoring, daily reward, shop shell, sandbox, Arabic/English UI, particles, save data, EditMode tests and GitHub Actions builds.

## Requirements
- Unity 6000.0.80f1 with Android Build Support
- Android API 36, NDK and OpenJDK installed by Unity Hub
- Minimum Android API 26

## GitHub Actions
Add `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` in Repository Settings > Secrets and variables > Actions. Push to `main`. The workflow runs EditMode tests first, then produces separate APK and AAB artifacts.

## Production integrations
`DevelopmentMonetization` is intentionally a safe adapter, not a fake live integration. Before publishing, connect your own Google AdMob/LevelPlay IDs, Google Play Billing products, consent flow, analytics, cloud save, release keystore and privacy disclosures. Never commit a keystore or passwords.

## Content pipeline
Levels are JSON files in `Assets/Resources/Levels`. Duplicate a file, increment `id`, and tune positions, ink, time, friction, bounce, obstacles and hazards. The included 50 levels are a structured starting campaign and should receive device playtesting and final hand-authored balancing before store release.
