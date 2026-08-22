# AssemblyInfo

AssemblyInfo is a source generator that exposes assembly attributes and custom assembly metadata through generated constants. The values are captured at compile time and require no runtime reflection, making it suitable for AOT.

> This project exists to provide access to assembly information without the need for reflection. If your build target supports reflection, you can still use this project but it would be for its convenience only.

## Installation

Add the package to the project whose assembly information you want to expose:

```shell
dotnet add package SlusserLabs.AssemblyInfo
```

## Usage

Mark a partial `class` or `record` as the destination for generated metadata using the `GenerateAssemblyInfo` attribute:

```csharp
using SlusserLabs.AssemblyInfo;

[GenerateAssemblyInfo]
public static partial class AssemblyInfo;
```

The following `.csproj` project properties are supported as outlined in [MSBuild documentation](https://learn.microsoft.com/en-us/dotnet/standard/assembly/set-attributes-project-file):
```xml
<PropertyGroup>
  <Company></Company>
  <Configuration></Configuration>
  <Copyright></Copyright>
  <Description></Description>
  <FileVersion></FileVersion>
  <InformationalVersion></InformationalVersion>
  <Product></Product>
  <AssemblyTitle></AssemblyTitle>
  <AssemblyVersion></AssemblyVersion>
</PropertyGroup>
```

The result is a generated class with the following `public const` fields:
```cs
public static partial class AssemblyInfo
{
  public const string Company;
  public const string Configuration;
  public const string Copyright;
  public const string Description;
  public const string FileVersion;
  public const string InformationalVersion;
  public const string Product;
  public const string AssemblyTitle;
  public const string AssemblyVersion;
}
```

That's it!

### AssemblyMetadata

Custom `AssemblyMetadataAttribute` values are also supported and makes it easy to embed [arbitrary strings](https://learn.microsoft.com/en-us/dotnet/standard/assembly/set-attributes-project-file#set-arbitrary-attributes). For example, this project configuration:

```xml
<ItemGroup>
  <AssemblyAttribute Include="System.Reflection.AssemblyMetadataAttribute">
    <_Parameter1>BuildDate</_Parameter1>
    <_Parameter2>2026-08-17</_Parameter2>
  </AssemblyAttribute>
</ItemGroup>
```

generates a `BuildDate` constant.

I would recommend always using key names that are also valid C# identifiers because they will be generated as the name of a `const`. When a key name is not a valid identifier, the generator will attempt to create a valid C# identifier by replacing unsupported characters.

### Advanced Options

The `GenerateAssemblyInfo` attribute accepts an optional `GenerateAssemblyInfoOptions` flags enum if you want want more control over which assembly metadata you want included. For example, if you wanted to put assembly attributes in one class and metadata in another to avoid naming collisions, you might do that like this (my usual preference):

```cs
using SlusserLabs.AssemblyInfo;

// Contains only title, product, version, etc...
[GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AllAssemblyAttributes)]
public static partial class AssemblyInfo
{
    // Contains only metadata key-value pairs
    [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AssemblyMetadata)]
    public static partial class Metadata;
}
```

## AI Disclosure

This project was built with the help of Codex 5.6 Sol for code generation. As with any tool I use for coding, I am fully responsible for the end result and only put forward this project with full confidence that I have reviewed every line. Whether a line of code in this project was fully AI generated, AI assisted, or entirely from my own Muppet fingers on the keyboard, it is held to the same expectations.

## Acknowledgements

This project is inspired by [ThisAssembly](https://github.com/devlooped/ThisAssembly). I wholeheartedly recommend that project if it is more to your liking; I just wanted slightly different ergonomics.
