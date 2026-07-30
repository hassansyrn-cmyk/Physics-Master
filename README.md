# Physics Master compiler fix

This patch fixes the two errors reported by Unity 6000.0.80f1:

1. In `Assets/Scripts/Core/AppController.cs`, replace `SleepTimeout.Never` with `SleepTimeout.NeverSleep`.
2. Replace the existing `Assets/Scripts/Gameplay/Effects.cs` with the file included in this patch.

The new effects implementation uses only `SpriteRenderer` and `Rigidbody2D`, so it does not require the 3D `UnityEngine.PhysicsModule` assembly.
