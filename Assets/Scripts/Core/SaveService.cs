using UnityEngine;
namespace PhysicsMaster.Core {
    public static class SaveService {
        public static int CurrentLevel { get => Mathf.Clamp(PlayerPrefs.GetInt("current_level", 1), 1, 50); set { PlayerPrefs.SetInt("current_level", Mathf.Clamp(value, 1, 50)); PlayerPrefs.Save(); } }
        public static int Unlocked => Mathf.Clamp(PlayerPrefs.GetInt("unlocked", 1), 1, 50);
        public static int Coins => PlayerPrefs.GetInt("coins", 0);
        public static int Gems => PlayerPrefs.GetInt("gems", 5);
        public static int Stars(int level) => PlayerPrefs.GetInt("stars_" + level, 0);
        public static void Complete(int level, int stars) {
            PlayerPrefs.SetInt("stars_" + level, Mathf.Max(stars, Stars(level)));
            PlayerPrefs.SetInt("unlocked", Mathf.Max(Unlocked, Mathf.Min(50, level + 1)));
            PlayerPrefs.SetInt("coins", Coins + stars * 10);
            PlayerPrefs.Save();
        }
        public static bool ClaimDaily() {
            string today = System.DateTime.UtcNow.ToString("yyyyMMdd");
            if (PlayerPrefs.GetString("daily") == today) return false;
            PlayerPrefs.SetString("daily", today); PlayerPrefs.SetInt("coins", Coins + 50); PlayerPrefs.Save(); return true;
        }
    }
}
