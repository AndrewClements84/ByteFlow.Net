# ByteFlow.Net

[![NuGet](https://img.shields.io/nuget/v/ByteFlow.Net.svg)](https://www.nuget.org/packages/ByteFlow.Net)
[![Build](https://github.com/AndrewClements84/ByteFlow.Net/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AndrewClements84/ByteFlow.Net/actions)
[![codecov](https://codecov.io/gh/AndrewClements84/ByteFlow.Net/branch/master/graph/badge.svg)](https://codecov.io/gh/AndrewClements84/ByteFlow.Net)

A lightweight .NET library for converting bytes into **human-readable formats** (KB, MB, GB, etc.) and back again.

---

## ✨ Features

- Convert bytes into human-readable strings (`1234567 → "1.18 MB"`).
- Parse strings back into bytes (`"2.5 GB" → 2684354560`).
- Safe parsing with `TryParseHumanBytes`.
- Zero dependencies, just clean C#.
- Works on .NET Framework (4.6.1+) and all modern .NET versions (Core, 5/6/7/8).

---

## 📦 Installation

Install from [NuGet](https://www.nuget.org/packages/ByteFlow.Net):

```sh
dotnet add package ByteFlow.Net
```

---

## 🚀 Usage

```csharp
using ByteFlowNet;

// Convert bytes to human-readable string
long size = 1234567;
Console.WriteLine(size.ToHumanBytes());    // "1.18 MB"
Console.WriteLine(size.ToHumanBytes(3));   // "1.177 MB"

// Parse string back into bytes
long bytes = "2.5 GB".ToBytes();           // 2684354560
Console.WriteLine(bytes);

// Safe parsing
if ("10 MB".TryParseHumanBytes(out var val))
{
    Console.WriteLine(val);                // 10485760
}
```

---

## 🧪 Unit Tests

Unit tests are included under the `tests/ByteFlowNet.Tests` project using **xUnit**.

Run them with:

```sh
dotnet test
```

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
