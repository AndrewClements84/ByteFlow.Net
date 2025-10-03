# Changelog

All notable changes to **ByteFlow.Net** will be documented in this file.

## [0.2.0] — 2025-10-03  
**Added**  
- Support for both **IEC (KiB, MiB, GiB)** and **SI (KB, MB, GB)** unit standards  
- Culture‑aware parsing and formatting (e.g. `1,5 MB` in de‑DE)  
- Customizable suffix sets (allowing domain‑specific units)  
- Alignment / padding helper (`ToHumanBytesAligned`)  

**Changed**  
- Updated existing methods to include optional `IFormatProvider` parameter  
- XML documentation updated to reflect new API  
- Default behavior remains backward compatible  

**Fixed**  
- Off‑by‑one padding in alignment helper  
- Improved test coverage to ensure all code paths (especially `TryParseHumanBytes` default overload) are exercised  

## [0.1.1] — 2025-09-30  
Initial release of ByteFlow.Net  
- Convert bytes into human‑readable strings (`1234567 → "1.18 MB"`)  
- Parse strings back into bytes (`"2.5 GB" → 2684354560`)  
- Safe parsing with `TryParseHumanBytes`  
- Zero dependencies, just clean C#  
