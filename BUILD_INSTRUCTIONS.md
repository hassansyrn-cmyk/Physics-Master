# دليل بناء APK Debug عبر GitHub Actions

## ما هو جاهز في هذا الملف بالفعل

- كل أكواد C# لنظام الرسم الحر → فيزياء، مع ملفات `.meta` بمعرّفات ثابتة.
- `Assets/Scenes/Main.unity`: مشهد واحد بسيط يحمل `SceneBootstrapper`، الذي يبني
  الكاميرا ومنطقة اللعب والأرضية ونظام الرسم بالكامل تلقائياً عند التشغيل —
  فلا حاجة لتركيب يدوي معقّد داخل المحرر.
- `ProjectSettings/ProjectVersion.txt` و`Packages/manifest.json` و
  `ProjectSettings/EditorBuildSettings.asset` (يسجّل المشهد ضمن قائمة البناء).
- `.github/workflows/android-build.yml`: سير عمل جاهز يبني APK Debug تلقائياً
  عند كل Push، بدون Keystore (يستخدم مفتاح Debug الافتراضي من Unity).

## ما لا يمكنني فعله من هنا (ويجب عمله مرة واحدة فقط)

أنا لا أملك نسخة فعلية من محرر Unity ولا اتصال إنترنت في بيئتي، لذلك لم أستطع
فتح المشروع واختباره أو توليد ملف `ProjectSettings/ProjectSettings.asset`
الكامل (وهو ملف ضخم جداً يحدد اسم الحزمة، رقم الإصدار، Scripting Backend...
إلخ). توليده يدوياً بدون اختبار فعلي مخاطرة عالية بوجود خطأ صامت يفشل البناء.

**الحل الصحيح والآمن:** افتح المشروع مرة واحدة فقط في Unity Editor المجاني على
جهازك (لا يحتاج أكثر من 15 دقيقة)، اضبط 4 إعدادات، ثم ارفعه لـ GitHub —
وبعدها كل بناء تالٍ يحدث تلقائياً بدون فتح Unity مرة أخرى.

### خطوات الإعداد لمرة واحدة

1. ثبّت **Unity Hub** ثم Unity Editor إصدار **6000.0.71f1** (أو أي إصدار Unity 6 LTS متوفر لديك — عدّل رقم الإصدار في `ProjectVersion.txt` وفي `android-build.yml` ليطابق ما تثبّته).
2. افتح المجلد `PhysicsMaster` كمشروع من Unity Hub. سيقوم Unity تلقائياً بإنشاء أي ملفات إعدادات ناقصة بقيمها الافتراضية.
3. من `Edit → Project Settings → Player`:
   - **Company Name / Product Name**: اكتب اسمك/اسم اللعبة.
   - تبويب **Android**: **Package Name** (Other Settings → Identification) بصيغة `com.yourname.physicsmaster`.
   - **Minimum API Level**: اختر Android 8.0 'Oreo' (API level 26) تطبيقاً لما ورد في مواصفاتك الأصلية.
   - **Scripting Backend**: IL2CPP، و **Target Architectures**: فعّل ARM64 (وARMv7 اختياري).
4. من `File → Build Settings`، تأكد أن المنصة Android مُفعّلة (Switch Platform إن لزم) وأن `Assets/Scenes/Main.unity` مُدرج في القائمة (يجب أن يكون مُدرجاً تلقائياً من `EditorBuildSettings.asset`).
5. احفظ (Ctrl+S) — هذا يُنشئ/يُحدّث `ProjectSettings.asset` بالقيم الصحيحة.

### رفع المشروع على GitHub وتفعيل البناء التلقائي

1. أنشئ مستودع GitHub جديد (فارغ)، ثم من مجلد المشروع محلياً:
   ```
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin <رابط-مستودعك>
   git push -u origin main
   ```
2. **رخصة Unity المجانية للـ CI**: هذه الخطوة إلزامية من Unity نفسها (وليست شيئاً أضفته أنا) — GitHub Actions يحتاج ملف ترخيص لتشغيل Unity بدون واجهة. اتبع دليل GameCI الرسمي بالضبط: `game.ci/docs/github/activation`. باختصار:
   - أضف سير عمل تفعيل مؤقت (موثّق بالكامل في الرابط أعلاه) يولّد ملف `.alf`.
   - فعّله على `license.unity.com` بحساب Unity مجاني، حمّل ملف `.ulf` الناتج.
   - أضف محتواه كـ Secret باسم `UNITY_LICENSE` في `Settings → Secrets and variables → Actions` في مستودعك.
3. بمجرد وجود `UNITY_LICENSE`، أي Push جديد على `main` سيُشغّل `android-build.yml` تلقائياً، وستجد ملف الـ APK في تبويب **Actions → (آخر تشغيل) → Artifacts → PhysicsMaster-debug-apk**.

## لماذا APK وليس AAB؟ ولماذا Debug تحديداً؟

سير العمل مضبوط على `androidExportType: androidPackage` (ملف `.apk` مباشر
قابل للتثبيت فوراً على أي جهاز عبر `adb install`)، ودون أي Keystore — ما يجعل
Unity يوقّعه تلقائياً بمفتاح Debug الافتراضي، وهو بالضبط ما طلبته.

## بعد أول بناء ناجح

الخطوة التالية لبناء اللعبة الحقيقية (المراحل، الأدوات، الواجهات...) هي أنظمة
منفصلة تُضاف تباعاً فوق هذا الأساس — أخبرني بأي جزء تريد إكماله بعد أن تتأكد أن هذا البناء الأولي يعمل لديك.
