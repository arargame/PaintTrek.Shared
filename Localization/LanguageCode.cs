using System;
using System.Collections.Generic;

namespace PaintTrek.Shared.Localization
{
    /// <summary>
    /// Oyunun desteklediği diller.
    /// </summary>
    public enum LanguageCode
    {
        English,
        Turkish,
        BrazilianPortuguese,
        EuropeanPortuguese,
        Spanish,
        LatinAmericanSpanish,
        German,
        Italian,
        French,
        TraditionalChinese,
        SimplifiedChinese,
        Japanese,
        Russian,
        Korean,
        Polish,
        Indonesian,
        Vietnamese,
        Finnish,
        Swedish,
        Danish,
        Dutch,
        Norwegian,
        Icelandic,
        Estonian,
        Thai,
        Arabic,
        Bulgarian,
        Serbian,
        Hebrew,
        Kurdish,
        Greek,
        Georgian,
        Czech,
        Hungarian,
        Romanian,
        Ukrainian,
        Slovak,
        Lithuanian,
        Latvian,
        Croatian,
        Bosnian,
        Slovenian,
        Albanian,
        Macedonian,
        Catalan,
        Belarusian,
        Swahili,
        Uzbek,
        Azerbaijani,
        Amharic,
        Armenian,
        Kazakh,
        Marathi,
        Gujarati,
        Punjabi,
        Kannada,
        Malayalam,
        Sinhala,
        Hindi,
        Urdu,
        Bengali,
        Telugu,
        Tamil
    }

    /// <summary>
    /// Bir dilin metnini çizebilen font ailesi.
    /// </summary>
    public enum ScriptFamily
    {
        Latin,
        Pixel,
        Noto,
        Georgian,
        Thai,
        Arabic,
        Hebrew,
        Ethiopic,
        Armenian,
        Cyrillic,
        Devanagari,
        Gujarati,
        Gurmukhi,
        Kannada,
        Malayalam,
        Sinhala,
        Bengali,
        Telugu,
        Tamil,
        Modern
    }

    /// <summary>
    /// Bir dilin kod, dosya ve gösterim bilgileri.
    /// </summary>
    public sealed class LanguageInfo
    {
        public string Code { get; }
        public string NativeName { get; }
        public string EnglishName { get; }
        public LanguageCode Language { get; }
        public ScriptFamily Family { get; }
        public bool IsRightToLeft { get; }

        public string FontSuffix => Family switch
        {
            ScriptFamily.Pixel    => "_PIXEL",
            ScriptFamily.Noto     => "_NOTO",
            ScriptFamily.Georgian => "_GEORGIAN",
            ScriptFamily.Thai     => "_THAI",
            ScriptFamily.Arabic   => "_ARABIC",
            ScriptFamily.Hebrew   => "_HEBREW",
            ScriptFamily.Ethiopic => "_ETHIOPIC",
            ScriptFamily.Armenian => "_ARMENIAN",
            ScriptFamily.Cyrillic => "_CYRILLIC",
            ScriptFamily.Devanagari => "_DEVANAGARI",
            ScriptFamily.Gujarati   => "_GUJARATI",
            ScriptFamily.Gurmukhi   => "_GURMUKHI",
            ScriptFamily.Kannada    => "_KANNADA",
            ScriptFamily.Malayalam  => "_MALAYALAM",
            ScriptFamily.Sinhala    => "_SINHALA",
            ScriptFamily.Bengali    => "_BENGALI",
            ScriptFamily.Telugu     => "_TELUGU",
            ScriptFamily.Tamil      => "_TAMIL",
            ScriptFamily.Modern     => "_MODERN",
            _                     => string.Empty,
        };

        public bool NeedsIndicShaping => IndicTextShaper.IsShapedFamily(Family);

        private string? _displayName;

        public string DisplayName
        {
            get
            {
                if (_displayName != null) return _displayName;

                if (NeedsIndicShaping)
                    _displayName = IndicTextShaper.Process(NativeName, Family);
                else if (IsRightToLeft)
                    _displayName = RtlTextShaper.Process(NativeName, rtlParagraph: true);
                else
                    _displayName = NativeName;

                return _displayName;
            }
        }

        public LanguageInfo(LanguageCode lang, string code, string nativeName, string englishName,
                            ScriptFamily family = ScriptFamily.Latin, bool isRightToLeft = false)
        {
            Language = lang;
            Code = code;
            NativeName = nativeName;
            EnglishName = englishName;
            Family = family;
            IsRightToLeft = isRightToLeft;
        }
    }

    public static class Languages
    {
        public static readonly LanguageInfo[] All =
        {
            new(LanguageCode.English,             "en",    "English",              "English"),
            new(LanguageCode.Turkish,             "tr",    "Türkçe",               "Turkish",              ScriptFamily.Modern),
            new(LanguageCode.BrazilianPortuguese, "pt-BR", "Português (Brasil)",   "Portuguese (Brazil)",  ScriptFamily.Modern),
            new(LanguageCode.EuropeanPortuguese,  "pt-PT", "Português (Portugal)", "Portuguese (Portugal)",ScriptFamily.Modern),
            new(LanguageCode.Spanish,             "es",    "Español",              "Spanish",              ScriptFamily.Modern),
            new(LanguageCode.LatinAmericanSpanish,"es-419","Español (Latinoamérica)","Spanish (Latin America)",ScriptFamily.Modern),
            new(LanguageCode.German,              "de",    "Deutsch",              "German",               ScriptFamily.Modern),
            new(LanguageCode.Italian,             "it",    "Italiano",             "Italian",              ScriptFamily.Modern),
            new(LanguageCode.French,              "fr",    "Français",             "French",               ScriptFamily.Modern),
            new(LanguageCode.TraditionalChinese,  "zh-TW", "繁體中文",             "Chinese (Traditional)", ScriptFamily.Pixel),
            new(LanguageCode.SimplifiedChinese,   "zh-CN", "简体中文",             "Chinese (Simplified)",  ScriptFamily.Pixel),
            new(LanguageCode.Japanese,            "ja",    "日本語",               "Japanese",              ScriptFamily.Pixel),
            new(LanguageCode.Russian,             "ru",    "Русский",              "Russian",               ScriptFamily.Pixel),
            new(LanguageCode.Bulgarian,           "bg",    "Български",            "Bulgarian",             ScriptFamily.Pixel),
            new(LanguageCode.Serbian,             "sr",    "Српски",               "Serbian",               ScriptFamily.Pixel),
            new(LanguageCode.Greek,               "el",    "Ελληνικά",             "Greek",                 ScriptFamily.Pixel),
            new(LanguageCode.Georgian,            "ka",    "ქართული",              "Georgian",              ScriptFamily.Georgian),
            new(LanguageCode.Kurdish,             "ku",    "Kurmancî",             "Kurdish",              ScriptFamily.Modern),
            new(LanguageCode.Korean,              "ko",    "한국어",               "Korean",                ScriptFamily.Noto),
            new(LanguageCode.Polish,              "pl",    "Polski",               "Polish",                ScriptFamily.Pixel),
            new(LanguageCode.Indonesian,          "id",    "Bahasa Indonesia",     "Indonesian",            ScriptFamily.Modern),
            new(LanguageCode.Vietnamese,          "vi",    "Tiếng Việt",           "Vietnamese",            ScriptFamily.Noto),
            new(LanguageCode.Finnish,             "fi",    "Suomi",                "Finnish",               ScriptFamily.Modern),
            new(LanguageCode.Swedish,             "sv",    "Svenska",              "Swedish",               ScriptFamily.Modern),
            new(LanguageCode.Danish,              "da",    "Dansk",                "Danish",                ScriptFamily.Modern),
            new(LanguageCode.Dutch,               "nl",    "Nederlands",           "Dutch",                 ScriptFamily.Modern),
            new(LanguageCode.Norwegian,           "no",    "Norsk",                "Norwegian",             ScriptFamily.Modern),
            new(LanguageCode.Icelandic,           "is",    "Íslenska",             "Icelandic",             ScriptFamily.Modern),
            new(LanguageCode.Estonian,            "et",    "Eesti",                "Estonian",              ScriptFamily.Modern),
            new(LanguageCode.Thai,                "th",    "ไทย",                  "Thai",                  ScriptFamily.Thai),
            new(LanguageCode.Arabic,              "ar",    "العربية",              "Arabic",                ScriptFamily.Arabic, true),
            new(LanguageCode.Hebrew,              "he",    "עברית",                "Hebrew",                ScriptFamily.Hebrew, true),
            new(LanguageCode.Czech,               "cs",    "Čeština",              "Czech",                 ScriptFamily.Modern),
            new(LanguageCode.Hungarian,           "hu",    "Magyar",               "Hungarian",             ScriptFamily.Modern),
            new(LanguageCode.Romanian,            "ro",    "Română",               "Romanian",              ScriptFamily.Modern),
            new(LanguageCode.Ukrainian,           "uk",    "Українська",           "Ukrainian",             ScriptFamily.Pixel),
            new(LanguageCode.Slovak,              "sk",    "Slovenčina",           "Slovak",                ScriptFamily.Modern),
            new(LanguageCode.Lithuanian,          "lt",    "Lietuvių",             "Lithuanian",            ScriptFamily.Modern),
            new(LanguageCode.Latvian,             "lv",    "Latviešu",             "Latvian",               ScriptFamily.Modern),
            new(LanguageCode.Croatian,            "hr",    "Hrvatski",             "Croatian",              ScriptFamily.Modern),
            new(LanguageCode.Bosnian,             "bs",    "Bosanski",             "Bosnian",               ScriptFamily.Modern),
            new(LanguageCode.Slovenian,           "sl",    "Slovenščina",          "Slovenian",             ScriptFamily.Modern),
            new(LanguageCode.Albanian,            "sq",    "Shqip",                "Albanian",              ScriptFamily.Modern),
            new(LanguageCode.Macedonian,          "mk",    "Македонски",           "Macedonian",            ScriptFamily.Pixel),
            new(LanguageCode.Catalan,             "ca",    "Català",               "Catalan",               ScriptFamily.Modern),
            new(LanguageCode.Belarusian,          "be",    "Беларуская",           "Belarusian",            ScriptFamily.Pixel),
            new(LanguageCode.Swahili,             "sw",    "Kiswahili",            "Swahili",               ScriptFamily.Modern),
            new(LanguageCode.Uzbek,               "uz",    "Oʻzbekcha",            "Uzbek",                 ScriptFamily.Modern),
            new(LanguageCode.Azerbaijani,         "az",    "Azərbaycanca",         "Azerbaijani",           ScriptFamily.Modern),
            new(LanguageCode.Amharic,             "am",    "አማርኛ",                "Amharic",               ScriptFamily.Ethiopic),
            new(LanguageCode.Armenian,            "hy",    "Հայերեն",              "Armenian",              ScriptFamily.Armenian),
            new(LanguageCode.Kazakh,              "kk",    "Қазақша",              "Kazakh",                ScriptFamily.Cyrillic),

            // Hint dilleri + Sinhala
            new(LanguageCode.Marathi,             "mr",    "मराठी",                "Marathi",               ScriptFamily.Devanagari),
            new(LanguageCode.Gujarati,            "gu",    "ગુજરાતી",               "Gujarati",              ScriptFamily.Gujarati),
            new(LanguageCode.Punjabi,             "pa",    "ਪੰਜਾਬੀ",                "Punjabi",               ScriptFamily.Gurmukhi),
            new(LanguageCode.Kannada,             "kn",    "ಕನ್ನಡ",                 "Kannada",               ScriptFamily.Kannada),
            new(LanguageCode.Malayalam,           "ml",    "മലയാളം",              "Malayalam",             ScriptFamily.Malayalam),
            new(LanguageCode.Sinhala,             "si",    "සිංහල",                "Sinhala",               ScriptFamily.Sinhala),
            new(LanguageCode.Hindi,               "hi",    "हिन्दी",               "Hindi",                 ScriptFamily.Devanagari),
            new(LanguageCode.Urdu,                "ur",    "اردو",                "Urdu",                  ScriptFamily.Arabic, true),
            new(LanguageCode.Bengali,             "bn",    "বাংলা",                "Bengali",               ScriptFamily.Bengali),
            new(LanguageCode.Telugu,              "te",    "తెలుగు",               "Telugu",                ScriptFamily.Telugu),
            new(LanguageCode.Tamil,               "ta",    "தமிழ்",                "Tamil",                 ScriptFamily.Tamil),
        };

        private static readonly ScriptFamily[] FamiliesNotSelectable = {};

        public static readonly LanguageInfo[] Selectable = BuildSelectable();

        private static LanguageInfo[] BuildSelectable()
        {
            var list = new List<LanguageInfo>(All.Length);
            foreach (var info in All)
            {
                bool blocked = false;
                for (int i = 0; i < FamiliesNotSelectable.Length; i++)
                    if (FamiliesNotSelectable[i] == info.Family) { blocked = true; break; }

                if (!blocked) list.Add(info);
            }
            list.Sort((a, b) => string.Compare(a.EnglishName, b.EnglishName, StringComparison.OrdinalIgnoreCase));
            return list.ToArray();
        }

        public static bool IsSelectable(LanguageCode lang)
        {
            for (int i = 0; i < Selectable.Length; i++)
                if (Selectable[i].Language == lang) return true;
            return false;
        }

        public static bool IsRtl(LanguageCode lang) => Get(lang).IsRightToLeft;

        public static ScriptFamily FamilyOf(LanguageCode lang) => Get(lang).Family;

        public static string FontSuffixOf(LanguageCode lang) => Get(lang).FontSuffix;

        private static readonly Dictionary<LanguageCode, LanguageInfo> _byLang = Build();
        private static readonly Dictionary<string, LanguageInfo> _byCode = BuildByCode();

        private static Dictionary<LanguageCode, LanguageInfo> Build()
        {
            var d = new Dictionary<LanguageCode, LanguageInfo>();
            foreach (var i in All) d[i.Language] = i;
            return d;
        }

        private static Dictionary<string, LanguageInfo> BuildByCode()
        {
            var d = new Dictionary<string, LanguageInfo>(System.StringComparer.OrdinalIgnoreCase);
            foreach (var i in All) d[i.Code] = i;
            return d;
        }

        public static LanguageInfo Get(LanguageCode lang)
            => _byLang.TryGetValue(lang, out var i) ? i : _byLang[LanguageCode.English];

        public static LanguageCode FromCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return LanguageCode.English;
            return _byCode.TryGetValue(code, out var i) ? i.Language : LanguageCode.English;
        }

        public static string CodeOf(LanguageCode lang) => Get(lang).Code;
    }
}
