using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaintTrek.Shared.Localization
{
    /// <summary>
    /// Hint yazıları ve Sinhala için metin şekillendirici arama tablosu.
    /// XNA bağımlılıkları tamamen kaldırılmış ve platform bağımsız hale getirilmiştir.
    /// </summary>
    public static class IndicTextShaper
    {
        private const int PuaStart = 0xE000;
        private const int PuaEnd = 0xF8FF;

        private sealed class ShapeMap
        {
            public Dictionary<string, string> Map = new(StringComparer.Ordinal);
            public int MaxKeyLength;
            public bool IsUsable => MaxKeyLength > 0 && Map.Count > 0;
        }

        private static readonly Dictionary<ScriptFamily, ShapeMap> _cache = new();

        public static bool IsShapedFamily(ScriptFamily family) => MapFileOf(family) != null;

        private static string? MapFileOf(ScriptFamily family) => family switch
        {
            ScriptFamily.Devanagari => "devanagari.json",
            ScriptFamily.Gujarati   => "gujarati.json",
            ScriptFamily.Gurmukhi   => "gurmukhi.json",
            ScriptFamily.Kannada    => "kannada.json",
            ScriptFamily.Malayalam  => "malayalam.json",
            ScriptFamily.Sinhala    => "sinhala.json",
            ScriptFamily.Bengali    => "bengali.json",
            ScriptFamily.Telugu     => "telugu.json",
            ScriptFamily.Tamil      => "tamil.json",
            _                       => null,
        };

        public static string Process(string text, ScriptFamily family, IStreamProvider? streamProvider = null)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var map = Load(family, streamProvider);
            if (map == null || !map.IsUsable) return text;

            var sb = new StringBuilder(text.Length);
            int i = 0;
            int n = text.Length;

            while (i < n)
            {
                char c = text[i];

                if (c < 128)
                {
                    sb.Append(c);
                    i++;
                    continue;
                }

                int maxLen = Math.Min(map.MaxKeyLength, n - i);
                bool matched = false;
                for (int len = maxLen; len >= 1; len--)
                {
                    if (map.Map.TryGetValue(text.Substring(i, len), out var pua))
                    {
                        sb.Append(pua);
                        i += len;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    sb.Append(c);
                    i++;
                }
            }

            return sb.ToString();
        }

        private static ShapeMap? Load(ScriptFamily family, IStreamProvider? streamProvider)
        {
            if (_cache.TryGetValue(family, out var cached)) return cached;

            string? file = MapFileOf(family);
            if (file == null) return null;

            // StreamProvider verilmemişse Loc.StreamProvider kullanılır.
            var provider = streamProvider ?? Loc.StreamProvider;
            if (provider == null)
            {
                Log($"StreamProvider yok. {file} yuklenemedi.");
                return null;
            }

            var result = ReadMap($"Content/Localization/shaping/{file}", provider);
            if (result == null)
            {
                Log($"{file} okunamadi -> {family} metni SEKILLENDIRILMEDEN cizilecek.");
                result = new ShapeMap();
            }
            else
            {
                Log($"{file} yuklendi ({result.Map.Count} kume, en uzun anahtar {result.MaxKeyLength})");
            }

            _cache[family] = result;
            return result;
        }

        private static ShapeMap? ReadMap(string assetPath, IStreamProvider streamProvider)
        {
            try
            {
                using Stream stream = streamProvider.Open(assetPath);
                using StreamReader reader = new StreamReader(stream);
                using JsonTextReader jsonReader = new JsonTextReader(reader);

                JObject root = JObject.Load(jsonReader);
                if (root == null) return null;

                JObject map = root["map"] as JObject;
                if (map == null) return null;

                var shape = new ShapeMap();
                foreach (var property in map.Properties())
                {
                    string key = property.Name;
                    string? value = property.Value?.ToString();
                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) continue;

                    if (value.Length != 1 || value[0] < PuaStart || value[0] > PuaEnd)
                        continue;

                    shape.Map[key] = value;
                    if (key.Length > shape.MaxKeyLength) shape.MaxKeyLength = key.Length;
                }

                return shape.IsUsable ? shape : null;
            }
            catch (Exception ex)
            {
                Log($"{assetPath}: {ex.GetType().Name} {ex.Message}");
                return null;
            }
        }

        public static void ClearCache() => _cache.Clear();

        private static void Log(string msg)
            => System.Diagnostics.Debug.WriteLine($"[IndicTextShaper] {msg}");
    }
}
