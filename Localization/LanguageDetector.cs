using System.Collections.Generic;
using System.Globalization;

namespace PaintTrek.Shared.Localization
{
    /// <summary>
    /// Cihaz dilini oyunun desteklediği bir dile eşler.
    /// </summary>
    public static class LanguageDetector
    {
        private static readonly Dictionary<string, LanguageCode> Map =
            new(System.StringComparer.OrdinalIgnoreCase)
        {
            { "pt-BR", LanguageCode.BrazilianPortuguese },
            { "pt-PT", LanguageCode.EuropeanPortuguese },
            { "pt",    LanguageCode.BrazilianPortuguese },

            { "en", LanguageCode.English },
            { "tr", LanguageCode.Turkish },
            { "es-ES", LanguageCode.Spanish },
            { "es-419", LanguageCode.LatinAmericanSpanish },
            { "es-MX", LanguageCode.LatinAmericanSpanish },
            { "es-AR", LanguageCode.LatinAmericanSpanish },
            { "es-CO", LanguageCode.LatinAmericanSpanish },
            { "es-CL", LanguageCode.LatinAmericanSpanish },
            { "es-PE", LanguageCode.LatinAmericanSpanish },
            { "es", LanguageCode.Spanish },
            { "de", LanguageCode.German },
            { "it", LanguageCode.Italian },
            { "zh-TW", LanguageCode.TraditionalChinese },
            { "zh-HK", LanguageCode.TraditionalChinese },
            { "zh-Hant", LanguageCode.TraditionalChinese },
            { "zh-CN", LanguageCode.SimplifiedChinese },
            { "zh-Hans", LanguageCode.SimplifiedChinese },
            { "zh", LanguageCode.SimplifiedChinese },
            { "ja", LanguageCode.Japanese },

            { "ru", LanguageCode.Russian },
            { "ru-RU", LanguageCode.Russian },
            { "ru-MD", LanguageCode.Russian },

            { "bg", LanguageCode.Bulgarian },
            { "bg-BG", LanguageCode.Bulgarian },

            { "he", LanguageCode.Hebrew },
            { "he-IL", LanguageCode.Hebrew },
            { "iw", LanguageCode.Hebrew },
            { "iw-IL", LanguageCode.Hebrew },

            { "ko", LanguageCode.Korean },
            { "pl", LanguageCode.Polish },
            { "vi", LanguageCode.Vietnamese },

            { "id", LanguageCode.Indonesian },
            { "in", LanguageCode.Indonesian },

            { "fi", LanguageCode.Finnish },
            { "sv", LanguageCode.Swedish },
            { "da", LanguageCode.Danish },
            { "nl", LanguageCode.Dutch },
            { "no", LanguageCode.Norwegian },
            { "nb", LanguageCode.Norwegian },
            { "nn", LanguageCode.Norwegian },
            { "is", LanguageCode.Icelandic },
            { "et", LanguageCode.Estonian },
            { "et-EE", LanguageCode.Estonian },

            { "ku", LanguageCode.Kurdish },
            { "el", LanguageCode.Greek },
            { "ka", LanguageCode.Georgian },

            { "sr", LanguageCode.Serbian },
            { "sr-RS", LanguageCode.Serbian },
            { "sr-Cyrl", LanguageCode.Serbian },
            { "sr-Latn", LanguageCode.Serbian },
            { "th", LanguageCode.Thai },
            { "ar", LanguageCode.Arabic },
            { "sw", LanguageCode.Swahili },
            { "uz", LanguageCode.Uzbek },
            { "az", LanguageCode.Azerbaijani },
            { "am", LanguageCode.Amharic },
            { "hy", LanguageCode.Armenian },
            { "kk", LanguageCode.Kazakh },

            { "mr", LanguageCode.Marathi },
            { "gu", LanguageCode.Gujarati },
            { "pa", LanguageCode.Punjabi },
            { "kn", LanguageCode.Kannada },
            { "ml", LanguageCode.Malayalam },
            { "si", LanguageCode.Sinhala },
            { "hi", LanguageCode.Hindi },
            { "hi-IN", LanguageCode.Hindi },
            { "ur", LanguageCode.Urdu },
            { "ur-PK", LanguageCode.Urdu },
            { "ur-IN", LanguageCode.Urdu },
            { "bn", LanguageCode.Bengali },
            { "bn-BD", LanguageCode.Bengali },
            { "bn-IN", LanguageCode.Bengali },
            { "te", LanguageCode.Telugu },
            { "te-IN", LanguageCode.Telugu },
            { "ta", LanguageCode.Tamil },
            { "ta-IN", LanguageCode.Tamil },
            { "ta-LK", LanguageCode.Tamil },
        };

        /// <summary>
        /// Cihaz dilini tespit eder. Belirsizlik durumunda İngilizce döner.
        /// </summary>
        public static LanguageCode Detect()
        {
            string tag;
            try
            {
                tag = CultureInfo.CurrentUICulture.Name;
            }
            catch
            {
                return LanguageCode.English;
            }

            return FromCultureTag(tag);
        }

        public static LanguageCode FromCultureTag(string? tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return LanguageCode.English;

            if (Map.TryGetValue(tag, out var exact)) return Gate(exact);

            int dash = tag.IndexOf('-');
            string two = dash > 0 ? tag.Substring(0, dash) : tag;
            if (Map.TryGetValue(two, out var lang)) return Gate(lang);

            return LanguageCode.English;
        }

        private static LanguageCode Gate(LanguageCode lang)
            => Languages.IsSelectable(lang) ? lang : LanguageCode.English;
    }
}
