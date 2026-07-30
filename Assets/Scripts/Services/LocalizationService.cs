using System.Collections.Generic;
using UnityEngine;
namespace PhysicsMaster.Services {
    public static class LocalizationService {
        static readonly Dictionary<string,string> En = new() {{"continue","CONTINUE"},{"levels","LEVELS"},{"sandbox","SANDBOX"},{"daily","DAILY REWARD"},{"shop","SHOP"},{"settings","SETTINGS"},{"play","PLAY"},{"pause","PAUSE"},{"resume","RESUME"},{"restart","RESTART"},{"menu","MENU"},{"hint","HINT"},{"undo","UNDO"},{"clear","CLEAR"},{"ink","INK"},{"solved","EXPERIMENT SOLVED"},{"next","NEXT"},{"retry","IMPROVE"},{"locked","LOCKED"},{"claimed","50 COINS CLAIMED"},{"tools","TOOLS"},{"objective","Guide the orange ball into the green goal"},{"sandboxInfo","Creative Lab: unlimited ink and no score"}};
        static readonly Dictionary<string,string> Ar = new() {{"continue","متابعة اللعب"},{"levels","المراحل"},{"sandbox","المختبر الحر"},{"daily","المكافأة اليومية"},{"shop","المتجر"},{"settings","الإعدادات"},{"play","تشغيل"},{"pause","إيقاف"},{"resume","متابعة"},{"restart","إعادة"},{"menu","القائمة"},{"hint","تلميح"},{"undo","تراجع"},{"clear","مسح"},{"ink","الحبر"},{"solved","تم حل التجربة"},{"next","التالي"},{"retry","تحسين النتيجة"},{"locked","مقفلة"},{"claimed","حصلت على 50 عملة"},{"tools","الأدوات"},{"objective","وجّه الكرة البرتقالية إلى الهدف الأخضر"},{"sandboxInfo","مختبر إبداعي: حبر غير محدود ودون تقييم"}};
        public static bool Arabic { get; private set; }
        public static void Initialize() { Arabic = Application.systemLanguage == SystemLanguage.Arabic; }
        public static void Toggle() { Arabic = !Arabic; }
        public static string T(string key) { var d = Arabic ? Ar : En; return d.TryGetValue(key, out var v) ? v : key; }
    }
}
