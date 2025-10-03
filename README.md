<p align="center">
  <img src="https://raw.githubusercontent.com/AndrewClements84/ByteFlow.Net/master/assets/logo.png" alt="ByteFlow.Net Logo" width="200"/>
</p>

# ByteFlow.Net

[![Build](https://github.com/AndrewClements84/ByteFlow.Net/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AndrewClements84/ByteFlow.Net/actions)
[![codecov](https://codecov.io/gh/AndrewClements84/ByteFlow.Net/branch/master/graph/badge.svg)](https://codecov.io/gh/AndrewClements84/ByteFlow.Net)
[![NuGet Version](https://img.shields.io/nuget/v/ByteFlow.Net.svg?logo=nuget&cacheSeconds=300)](https://www.nuget.org/packages/ByteFlow.Net)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ByteFlow.Net.svg)](https://www.nuget.org/packages/ByteFlow.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)
![GitHub Repo stars](https://img.shields.io/github/stars/AndrewClements84/ByteFlow.Net?style=flat&color=2bbc8a)

ByteFlow.Net — Convert bytes ⇄ human-readable formats with SI, IEC, culture-aware, and customizable units. Zero dependencies.

---

## ✨ Features

- Convert raw byte counts into human-readable strings (e.g. `1234567 → "1.23 MB"`)  
- Parse human-readable strings back into bytes (`"2.5 GB" → 2684354560`)  
- Safe parsing via `TryParseHumanBytes` (no exception)  
- Support for both **IEC (KiB, MiB, GiB)** and **SI (KB, MB, GB)** unit standards  
- Culture-aware parsing/formatting (e.g. `"1,5 MB"` for cultures with comma decimal separators)  
- Customizable suffix sets (allow defining your own unit names/factors)  
- Alignment/padding helpers for nicely formatted output in tables or logs  
- Fully tested with **100% code coverage**  
- Zero external dependencies — pure C#

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package ByteFlow.Net
```

---

## 🚀 Usage

```csharp
using ByteFlow;

// Basic conversion
long size = 1234567;
Console.WriteLine(size.ToHumanBytes());            // e.g. "1.18 MB" (default settings)
Console.WriteLine(size.ToHumanBytes(3));           // more decimals

// Parsing string to bytes
long bytes = "2.5 GB".ToBytes();                    // default parsing (SI/IEC based on default)
Console.WriteLine(bytes);

// Safe parsing
if ("10 MB".TryParseHumanBytes(out var val))
{
    Console.WriteLine(val);                         // prints bytes if successful
}

// Using IEC explicitly
Console.WriteLine(1536L.ToHumanBytes(2, UnitStandard.IEC)); // "1.50 KiB"
long val2 = "1.50 KiB".ToBytes(UnitStandard.IEC);

// Culture-aware parsing/formatting
var de = new System.Globalization.CultureInfo("de-DE");
Console.WriteLine((1500L).ToHumanBytes(2, UnitStandard.SI, de));   // "1,50 KB"
long val3 = "1,50 KB".ToBytes(UnitStandard.SI, de);

// Custom suffix sets
var custom = new[] { ("X", 1d), ("KX", 1000d), ("MX", 1000000d) };
string customStr = 5000L.ToHumanBytes(2, UnitStandard.SI, null, custom);  // "5.00 KX"
long customBytes = "5 KX".ToBytes(UnitStandard.SI, null, custom);
```

---

## 🧪 Unit Tests & Code Coverage

Unit tests are under `ByteFlow.Tests` (xUnit).  
Run them with:

```bash
dotnet test
```

Coverage is tracked via Codecov — current coverage: **100%** ✅

---

## 🧭 Alternatives / Comparisons

While there are other “bytes to human readable” .NET libraries out there, very few (if any) offer the combined feature set that **ByteFlow.Net** does:

- Support for both **IEC and SI**  
- Culture-aware parsing/formatting  
- Fully customizable suffix sets  
- Alignment/padding helpers  
- Zero dependencies  
- Complete test coverage  

So this library aims to be a robust one-stop solution for byte-size formatting.

---

## 🤝 Contributing

Contributions, issues, and feature requests are always welcome!  
Feel free to open a [discussion](https://github.com/AndrewClements84/ByteFlow.Net/discussions) or a [pull request](https://github.com/AndrewClements84/ByteFlow.Net/pulls).

---

## ⭐ Support

If you enjoy using **ByteFlow.Net**, a GitHub star helps more than you’d think — it boosts visibility and helps others find it.

---

## 📄 License

Licensed under the MIT License — see [LICENSE.txt](LICENSE.txt) for details.
