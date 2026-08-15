using System;
using System.Collections.Generic;
using System.Text;

namespace PaintTrek.Shared.Localization
{
    /// <summary>
    /// SAĞDAN SOLA (RTL) METİN İŞLEYİCİ — Arapça şekillendirme + basit BiDi.
    /// Blocked projesindeki doğrulanmış kütüphane yapısı.
    /// </summary>
    public static class RtlTextShaper
    {
        private const char NoForm = '\0';

        private static readonly Dictionary<char, char[]> Forms = new()
        {
            { '\u0621', new[] { '\uFE80', NoForm  , NoForm  , NoForm   } },   // hamza
            { '\u0622', new[] { '\uFE81', '\uFE82', NoForm  , NoForm   } },   // alef madda
            { '\u0623', new[] { '\uFE83', '\uFE84', NoForm  , NoForm   } },   // alef hamza ustte
            { '\u0624', new[] { '\uFE85', '\uFE86', NoForm  , NoForm   } },   // waw hamza
            { '\u0625', new[] { '\uFE87', '\uFE88', NoForm  , NoForm   } },   // alef hamza altta
            { '\u0626', new[] { '\uFE89', '\uFE8A', '\uFE8B', '\uFE8C' } },   // yeh hamza
            { '\u0627', new[] { '\uFE8D', '\uFE8E', NoForm  , NoForm   } },   // alef
            { '\u0628', new[] { '\uFE8F', '\uFE90', '\uFE91', '\uFE92' } },   // beh
            { '\u0629', new[] { '\uFE93', '\uFE94', NoForm  , NoForm   } },   // teh marbuta
            { '\u062A', new[] { '\uFE95', '\uFE96', '\uFE97', '\uFE98' } },   // teh
            { '\u062B', new[] { '\uFE99', '\uFE9A', '\uFE9B', '\uFE9C' } },   // theh
            { '\u062C', new[] { '\uFE9D', '\uFE9E', '\uFE9F', '\uFEA0' } },   // jeem
            { '\u062D', new[] { '\uFEA1', '\uFEA2', '\uFEA3', '\uFEA4' } },   // hah
            { '\u062E', new[] { '\uFEA5', '\uFEA6', '\uFEA7', '\uFEA8' } },   // khah
            { '\u062F', new[] { '\uFEA9', '\uFEAA', NoForm  , NoForm   } },   // dal
            { '\u0630', new[] { '\uFEAB', '\uFEAC', NoForm  , NoForm   } },   // thal
            { '\u0631', new[] { '\uFEAD', '\uFEAE', NoForm  , NoForm   } },   // reh
            { '\u0632', new[] { '\uFEAF', '\uFEB0', NoForm  , NoForm   } },   // zain
            { '\u0633', new[] { '\uFEB1', '\uFEB2', '\uFEB3', '\uFEB4' } },   // seen
            { '\u0634', new[] { '\uFEB5', '\uFEB6', '\uFEB7', '\uFEB8' } },   // sheen
            { '\u0635', new[] { '\uFEB9', '\uFEBA', '\uFEBB', '\uFEBC' } },   // sad
            { '\u0636', new[] { '\uFEBD', '\uFEBE', '\uFEBF', '\uFEC0' } },   // dad
            { '\u0637', new[] { '\uFEC1', '\uFEC2', '\uFEC3', '\uFEC4' } },   // tah
            { '\u0638', new[] { '\uFEC5', '\uFEC6', '\uFEC7', '\uFEC8' } },   // zah
            { '\u0639', new[] { '\uFEC9', '\uFECA', '\uFECB', '\uFECC' } },   // ain
            { '\u063A', new[] { '\uFECD', '\uFECE', '\uFECF', '\uFED0' } },   // ghain
            { '\u0640', new[] { '\u0640', '\u0640', '\u0640', '\u0640' } },   // tatweel
            { '\u0641', new[] { '\uFED1', '\uFED2', '\uFED3', '\uFED4' } },   // feh
            { '\u0642', new[] { '\uFED5', '\uFED6', '\uFED7', '\uFED8' } },   // qaf
            { '\u0643', new[] { '\uFED9', '\uFEDA', '\uFEDB', '\uFEDC' } },   // kaf
            { '\u0644', new[] { '\uFEDD', '\uFEDE', '\uFEDF', '\uFEE0' } },   // lam
            { '\u0645', new[] { '\uFEE1', '\uFEE2', '\uFEE3', '\uFEE4' } },   // meem
            { '\u0646', new[] { '\uFEE5', '\uFEE6', '\uFEE7', '\uFEE8' } },   // noon
            { '\u0647', new[] { '\uFEE9', '\uFEEA', '\uFEEB', '\uFEEC' } },   // heh
            { '\u0648', new[] { '\uFEED', '\uFEEE', NoForm  , NoForm   } },   // waw
            { '\u0649', new[] { '\uFEEF', '\uFEF0', NoForm  , NoForm   } },   // alef maksura
            { '\u064A', new[] { '\uFEF1', '\uFEF2', '\uFEF3', '\uFEF4' } },   // yeh
        };

        private static readonly Dictionary<char, char[]> LamAlef = new()
        {
            { '\u0622', new[] { '\uFEF5', '\uFEF6' } },
            { '\u0623', new[] { '\uFEF7', '\uFEF8' } },
            { '\u0625', new[] { '\uFEF9', '\uFEFA' } },
            { '\u0627', new[] { '\uFEFB', '\uFEFC' } },
        };

        private const char Lam = '\u0644';

        public static bool IsArabicLetter(char c) => Forms.ContainsKey(c);
        public static bool IsHebrewLetter(char c) => c >= '\u0590' && c <= '\u05FF';

        private static bool IsTransparent(char c)
            => (c >= '\u064B' && c <= '\u065F') || c == '\u0670';

        private static char Mirror(char c) => c switch
        {
            '(' => ')', ')' => '(',
            '[' => ']', ']' => '[',
            '{' => '}', '}' => '{',
            '<' => '>', '>' => '<',
            _ => c,
        };

        private static bool JoinsForward(char c)
            => Forms.TryGetValue(c, out var f) && f[2] != NoForm;

        private static bool JoinsBackward(char c)
            => Forms.TryGetValue(c, out var f) && f[1] != NoForm;

        public static string Process(string text, bool rtlParagraph = false)
        {
            if (string.IsNullOrEmpty(text)) return text;

            if (!rtlParagraph)
            {
                bool hasRtl = false;
                for (int i = 0; i < text.Length; i++)
                {
                    if (Forms.ContainsKey(text[i]) || IsHebrewLetter(text[i])) { hasRtl = true; break; }
                }
                if (!hasRtl) return text;
            }

            var sb = new StringBuilder(text.Length + 8);
            int start = 0;
            while (true)
            {
                int nl = text.IndexOf('\n', start);
                string line = nl < 0 ? text.Substring(start) : text.Substring(start, nl - start);

                sb.Append(ToVisualOrder(Shape(line)));
                if (nl < 0) break;

                sb.Append('\n');
                start = nl + 1;
            }
            return sb.ToString();
        }

        private static string Shape(string text)
        {
            var sb = new StringBuilder(text.Length);
            int i = 0;

            while (i < text.Length)
            {
                char ch = text[i];
                if (!Forms.TryGetValue(ch, out var forms))
                {
                    sb.Append(ch);
                    i++;
                    continue;
                }

                int p = i - 1;
                while (p >= 0 && IsTransparent(text[p])) p--;
                char prev = p >= 0 ? text[p] : NoForm;

                int k = i + 1;
                while (k < text.Length && IsTransparent(text[k])) k++;
                char next = k < text.Length ? text[k] : NoForm;

                bool prevJoins = prev != NoForm && JoinsForward(prev);

                if (ch == Lam && next != NoForm && LamAlef.TryGetValue(next, out var lig))
                {
                    sb.Append(prevJoins ? lig[1] : lig[0]);
                    i = k + 1;
                    continue;
                }

                bool nextJoins = next != NoForm && JoinsForward(ch) && JoinsBackward(next);

                char form;
                if (prevJoins && nextJoins) form = Pick(forms[3], forms[1], forms[0]);
                else if (prevJoins)         form = Pick(forms[1], forms[0], forms[0]);
                else if (nextJoins)         form = Pick(forms[2], forms[0], forms[0]);
                else                        form = forms[0];

                sb.Append(form);
                i++;
            }

            return sb.ToString();
        }

        private static char Pick(char first, char second, char third)
            => first != NoForm ? first : (second != NoForm ? second : third);

        private static string ToVisualOrder(string shaped)
        {
            int n = shaped.Length;
            var rev = new char[n];
            for (int i = 0; i < n; i++) rev[i] = shaped[n - 1 - i];

            var sb = new StringBuilder(n);
            int j = 0;
            while (j < n)
            {
                if (IsLtrRunChar(rev[j]))
                {
                    int end = j;
                    while (end < n &&
                           (IsLtrRunChar(rev[end]) ||
                            (IsLtrNeutral(rev[end]) && end + 1 < n && IsLtrRunChar(rev[end + 1]))))
                    {
                        end++;
                    }

                    for (int k = end - 1; k >= j; k--) sb.Append(rev[k]);
                    j = end;
                }
                else
                {
                    sb.Append(Mirror(rev[j]));
                    j++;
                }
            }

            return sb.ToString();
        }

        private static bool IsLtrRunChar(char c)
            => (c >= '0' && c <= '9') ||
               (c >= 'A' && c <= 'Z') ||
               (c >= 'a' && c <= 'z') ||
               c == '{' || c == '}';

        private static bool IsLtrNeutral(char c)
            => c == ' ' || c == '.' || c == ',' || c == ':' ||
               c == '%' || c == '-' || c == '/' || c == '+';
    }
}
