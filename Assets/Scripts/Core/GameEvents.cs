using System;
namespace PhysicsMaster.Core {
    public static class GameEvents {
        public static event Action<int> LevelCompleted;
        public static event Action LevelFailed;
        public static void Complete(int stars) { LevelCompleted?.Invoke(stars); }
        public static void Fail() { LevelFailed?.Invoke(); }
    }
}
