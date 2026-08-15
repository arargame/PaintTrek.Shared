using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaintTrek.Shared.Localization
{
    /// <summary>
    /// Çeviri erişim noktası. Platform bağımsız olarak yerelleştirilmiş metinlerin yüklenmesi ve getirilmesini yönetir.
    /// </summary>
    public static class Loc
    {
        public static LanguageCode Current { get; private set; } = LanguageCode.English;

        public static bool IsReady { get; private set; }

        public static bool IsRtl => Languages.IsRtl(Current);

        public static bool ShowMissingKeys { get; set; }

        public static IStreamProvider? StreamProvider { get; private set; }

        private const string PlaceholderPrefix = "⟪PLACEHOLDER⟫";
        private const string MetaPlaceholderKeys = "_meta.placeholderKeys";

        private static Dictionary<string, string> _active = new();
        private static Dictionary<string, string> _fallback = new();
        private static HashSet<string> _placeholders = new(StringComparer.Ordinal);

        /// <summary>
        /// Yerelleştirme motorunu başlatır.
        /// </summary>
        public static void Initialize(LanguageCode language, IStreamProvider streamProvider)
        {
            StreamProvider = streamProvider;

            try
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            }
            catch { /* platform izin vermezse yut */ }

            _fallback = LoadLanguage(LanguageCode.English) ?? new Dictionary<string, string>();

            if (language == LanguageCode.English)
            {
                _active = _fallback;
                Current = LanguageCode.English;
            }
            else
            {
                var loaded = LoadLanguage(language);
                if (loaded != null && loaded.Count > 0)
                {
                    _active = loaded;
                    Current = language;
                }
                else
                {
                    _active = _fallback;
                    Current = LanguageCode.English;
                    Log($"Initialize: {Languages.CodeOf(language)} yuklenemedi -> Ingilizce'ye dusuldu");
                }
            }

            RefreshPlaceholders();
            IsReady = _fallback.Count > 0 || _active.Count > 0;

            Log($"Initialize -> istenen={Languages.CodeOf(language)} aktif={Languages.CodeOf(Current)} " +
                $"(active:{_active.Count} keys, fallback:{_fallback.Count} keys, placeholders:{_placeholders.Count})");
        }

        /// <summary>
        /// Aktif dili değiştirir.
        /// </summary>
        public static bool SetLanguage(LanguageCode language)
        {
            if (language == Current && _active.Count > 0) return true;

            if (language == LanguageCode.English)
            {
                _active = _fallback.Count > 0 ? _fallback : _active;
                Current = LanguageCode.English;
                RefreshPlaceholders();
                return true;
            }

            var loaded = LoadLanguage(language);
            if (loaded == null || loaded.Count == 0)
            {
                Log($"SetLanguage({Languages.CodeOf(language)}) BASARISIZ — {Languages.CodeOf(Current)} korunuyor");
                return false;
            }

            _active = loaded;
            Current = language;
            RefreshPlaceholders();
            Log($"SetLanguage -> {Languages.CodeOf(language)} ({loaded.Count} keys)");
            return true;
        }

        /// <summary>
        /// Anahtarı çevirir. Asla exception fırlatmaz, asla null dönmez.
        /// </summary>
        public static string T(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            if (_placeholders.Contains(key) && _fallback.TryGetValue(key, out var en0))
                return en0;

            if (_active.TryGetValue(key, out var v))
            {
                if (v.Length == 0) return string.Empty;

                if (v.StartsWith(PlaceholderPrefix, StringComparison.Ordinal))
                    return _fallback.TryGetValue(key, out var en1) ? en1 : StripPlaceholder(v);

                return v;
            }

            if (_fallback.TryGetValue(key, out var en2) && !string.IsNullOrEmpty(en2))
                return en2;

            return ShowMissingKeys ? $"⟨{key}⟩" : key;
        }

        /// <summary>
        /// Formatlı çeviri.
        /// </summary>
        public static string T(string key, params object[] args)
        {
            string raw = T(key);
            if (args == null || args.Length == 0) return raw;

            try
            {
                return string.Format(CultureInfo.InvariantCulture, raw, args);
            }
            catch (FormatException)
            {
                if (_fallback.TryGetValue(key, out var en))
                {
                    try { return string.Format(CultureInfo.InvariantCulture, en, args); }
                    catch { /* düş */ }
                }
                Log($"T({key}) format hatasi — ham metin donduruldu");
                return raw;
            }
        }

        public static bool HasKey(string key)
            => !string.IsNullOrEmpty(key) && (_active.ContainsKey(key) || _fallback.ContainsKey(key));

        public static int PlaceholderCount => _placeholders.Count;

        private static Dictionary<string, string>? LoadLanguage(LanguageCode lang)
        {
            if (StreamProvider == null) return null;

            string code = Languages.CodeOf(lang);
            string assetPath = $"Content/Localization/{code}.json";

            // 1) StreamProvider aracılığıyla oku
            var dict = TryReadJson(() => StreamProvider.Open(assetPath));
            if (dict != null) return ApplyShaping(dict, lang);

            // 2) Gömülü kaynak (Assembly'den fallback olarak oku)
            dict = TryReadJson(() => OpenEmbedded($"{code}.json"));
            if (dict != null)
            {
                Log($"{code}.json asset'ten okunamadi, GOMULU kopya kullanildi");
                return ApplyShaping(dict, lang);
            }

            Log($"{code}.json HIC bulunamadi");
            return null;
        }

        private static Dictionary<string, string> ApplyShaping(Dictionary<string, string> dict, LanguageCode lang)
        {
            var info = Languages.Get(lang);

            if (!info.IsRightToLeft && !info.NeedsIndicShaping) return dict;

            if (info.NeedsIndicShaping)
            {
                var indic = new Dictionary<string, string>(dict.Count, StringComparer.Ordinal);
                foreach (var kv in dict)
                    indic[kv.Key] = IndicTextShaper.Process(kv.Value, info.Family, StreamProvider);
                return indic;
            }

            var shaped = new Dictionary<string, string>(dict.Count, StringComparer.Ordinal);
            foreach (var kv in dict)
            {
                shaped[kv.Key] = RtlTextShaper.Process(kv.Value, rtlParagraph: true);
            }
            return shaped;
        }

        private static Dictionary<string, string>? TryReadJson(Func<Stream?> open)
        {
            try
            {
                using var s = open();
                if (s == null) return null;

                using var reader = new StreamReader(s);
                string json = reader.ReadToEnd();
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
                if (parsed == null) return null;

                var result = new Dictionary<string, string>(parsed.Count, StringComparer.Ordinal);
                foreach (var kv in parsed)
                {
                    if (kv.Key == MetaPlaceholderKeys) continue;

                    if (kv.Value.Type == JTokenType.String)
                        result[kv.Key] = kv.Value.Value<string>() ?? string.Empty;
                }
                return result.Count > 0 ? result : null;
            }
            catch (Exception ex)
            {
                Log($"JSON okuma hatasi: {ex.GetType().Name} {ex.Message}");
                return null;
            }
        }

        private static Stream? OpenEmbedded(string fileName)
        {
            try
            {
                var asm = typeof(Loc).Assembly;
                foreach (var name in asm.GetManifestResourceNames())
                {
                    if (name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                        return asm.GetManifestResourceStream(name);
                }
            }
            catch { /* yut */ }
            return null;
        }

        private static void RefreshPlaceholders()
        {
            _placeholders = new HashSet<string>(StringComparer.Ordinal);
            if (Current == LanguageCode.English || StreamProvider == null) return;

            try
            {
                string code = Languages.CodeOf(Current);
                using var s = StreamProvider.Open($"Content/Localization/{code}.json");
                using var reader = new StreamReader(s);
                string json = reader.ReadToEnd();
                var obj = JObject.Parse(json);
                if (obj.TryGetValue(MetaPlaceholderKeys, out var arr) && arr.Type == JTokenType.Array)
                {
                    foreach (var el in arr.Children())
                    {
                        var k = el.Value<string>();
                        if (!string.IsNullOrEmpty(k)) _placeholders.Add(k);
                    }
                }
            }
            catch { /* manifest yoksa sorun değil */ }
        }

        private static string StripPlaceholder(string v)
            => v.StartsWith(PlaceholderPrefix, StringComparison.Ordinal)
                ? v.Substring(PlaceholderPrefix.Length).TrimStart()
                : v;

        private static void Log(string msg)
            => System.Diagnostics.Debug.WriteLine($"[Loc] {msg}");
    }
}
