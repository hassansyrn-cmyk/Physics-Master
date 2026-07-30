using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using ArabicSupport;

namespace PhysicsMaster.Services {
    public static class LocalizationService {
        static readonly Dictionary<string, string> En = new() {
            {"continue", "CONTINUE"},
            {"levels", "LEVELS"},
            {"sandbox", "SANDBOX"},
            {"daily", "DAILY REWARD"},
            {"shop", "SHOP"},
            {"settings", "SETTINGS"},
            {"play", "PLAY"},
            {"pause", "PAUSE"},
            {"resume", "RESUME"},
            {"restart", "RESTART"},
            {"menu", "MENU"},
            {"hint", "HINT"},
            {"undo", "UNDO"},
            {"clear", "CLEAR"},
            {"ink", "INK"},
            {"solved", "EXPERIMENT SOLVED"},
            {"next", "NEXT"},
            {"retry", "IMPROVE"},
            {"locked", "LOCKED"},
            {"claimed", "50 COINS CLAIMED"},
            {"tools", "TOOLS"},
            {"objective", "Guide the orange ball into the green goal"},
            {"sandboxInfo", "Creative Lab: unlimited ink and no score"}
        };

        static readonly Dictionary<string, string> Ar = new() {
            {"continue", "متابعة اللعب"},
            {"levels", "المراحل"},
            {"sandbox", "المختبر الحر"},
            {"daily", "المكافأة اليومية"},
            {"shop", "المتجر"},
            {"settings", "الإعدادات"},
            {"play", "تشغيل"},
            {"pause", "إيقاف"},
            {"resume", "متابعة"},
            {"restart", "إعادة"},
            {"menu", "القائمة"},
            {"hint", "تلميح"},
            {"undo", "تراجع"},
            {"clear", "مسح"},
            {"ink", "الحبر"},
            {"solved", "تم حل التجربة"},
            {"next", "التالي"},
            {"retry", "تحسين النتيجة"},
            {"locked", "مقفلة"},
            {"claimed", "حصلت على 50 عملة"},
            {"tools", "الأدوات"},
            {"objective", "وجّه الكرة البرتقالية إلى الهدف الأخضر"},
            {"sandboxInfo", "مختبر إبداعي: حبر غير محدود ودون تقييم"}
        };

        public static bool Arabic { get; private set; }

        public static void Initialize() {
            Arabic = Application.systemLanguage == SystemLanguage.Arabic;
        }

        public static void Toggle() {
            Arabic = !Arabic;
        }

        public static string T(string key) {
            var d = Arabic ? Ar : En;
            string val = d.TryGetValue(key, out var v) ? v : key;
            if (Arabic) {
                return ShapeText(val);
            }
            return val;
        }

        public static string ShapeText(string text) {
            if (string.IsNullOrEmpty(text)) return text;

            // Split into lines so we process them individually
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++) {
                if (HasArabic(lines[i])) {
                    // Check for Arabic characters, apply fixer to Arabic portions, or the entire line
                    lines[i] = ArabicFixer.Fix(lines[i], false, false);
                }
            }
            return string.Join("\n", lines);
        }

        public static bool HasArabic(string s) {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char c in s) {
                // Arabic Unicode blocks range: 0x0600 - 0x06FF
                if (c >= 0x0600 && c <= 0x06FF) {
                    return true;
                }
            }
            return false;
        }
    }
}
