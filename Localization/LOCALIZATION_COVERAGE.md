# Paint Trek Localization Coverage Report

## Summary
- **Canonical Key Count:** 174
- **Supported Languages Count:** 64
- **Generated Language Count:** 62 (excluding canonical `en` and `tr`)
- **Validation Result (Keys):** Pass (0 missing, 0 extra keys)
- **Validation Result (Placeholders):** Pass (All placeholders match en.json)

## Supported Language Codes (64)
`en`, `tr`, `pt-BR`, `pt-PT`, `es`, `es-419`, `de`, `it`, `fr`, `zh-TW`, `zh-CN`, `ja`, `ru`, `bg`, `sr`, `el`, `ka`, `ku`, `ko`, `pl`, `id`, `vi`, `fi`, `sv`, `da`, `nl`, `no`, `is`, `et`, `th`, `ar`, `fa`, `he`, `cs`, `hu`, `ro`, `uk`, `sk`, `lt`, `lv`, `hr`, `bs`, `sl`, `sq`, `mk`, `ca`, `be`, `sw`, `uz`, `az`, `am`, `hy`, `kk`, `mr`, `gu`, `pa`, `kn`, `ml`, `si`, `hi`, `ur`, `bn`, `te`, `ta`

## Generated Language Codes (62)
`pt-BR`, `pt-PT`, `es`, `es-419`, `de`, `it`, `fr`, `zh-TW`, `zh-CN`, `ja`, `ru`, `bg`, `sr`, `el`, `ka`, `ku`, `ko`, `pl`, `id`, `vi`, `fi`, `sv`, `da`, `nl`, `no`, `is`, `et`, `th`, `ar`, `fa`, `he`, `cs`, `hu`, `ro`, `uk`, `sk`, `lt`, `lv`, `hr`, `bs`, `sl`, `sq`, `mk`, `ca`, `be`, `sw`, `uz`, `az`, `am`, `hy`, `kk`, `mr`, `gu`, `pa`, `kn`, `ml`, `si`, `hi`, `ur`, `bn`, `te`, `ta`

## Translation Caveats & Intentionally Untranslated Terms
1. **Proper Nouns & Credits:** Developer names (`Koray Arar`, `Sahin Meric` / `Şahin Meriç`) and cat names (`Aslan`, `Karakuzu`, `Kibar`, `Mazlum`, `Mahsun`, `Anne`, `Keçi`, `Portakal`, `Minnak`) are intentionally preserved across all target languages.
2. **Game Terms & Acronyms:** `HP`, `UFO`, `Boss`, `Cacao`, and `Mr. Brain` are kept consistent with standard game localization conventions for sci-fi/arcade action games.
3. **Key Shortcuts & Formatting Tokens:** Keyboard inputs (`'W'`, `'A'`, `'S'`, `'D'`, `'Space'`, `'K'`, `'P'`, `'Esc'`) and formatting tokens (`{0}`, `{1:00}`, `{2:00}`, `\n`) are strictly preserved to ensure UI layout stability and runtime formatting compatibility.
4. **RTL & Script Shaping:** Right-to-Left (Arabic `ar`, Hebrew `he`, Urdu `ur`) and Indic/Asian text shaping options are handled dynamically at runtime by `PaintTrek.Shared.Localization` shapers (`RtlTextShaper`, `IndicTextShaper`).
