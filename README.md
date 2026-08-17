# AssemblyInfo

AssemblyInfo is an incremental `C#` source generator that exposes assembly attributes and custom assembly metadata through generated constants. The values are captured at compile time and require no runtime reflection.

The project is inspired by [ThisAssembly](https://github.com/devlooped/ThisAssembly).

## Installation

Add the package to the project whose assembly information you want to expose:

```shell
dotnet add package SlusserLabs.AssemblyInfo
```

## Usage

Mark a partial `class` or record class as the destination for generated metadata:

```csharp
using SlusserLabs.AssemblyInfo;

[GeneratedAssemblyInfo]
public static partial class ThisAssembly;
```

The default generates every supported value. You can select only the values your application needs:

```csharp
using SlusserLabs.AssemblyInfo;

[GeneratedAssemblyInfo(AssemblyInfoOptions.AssemblyTitle | AssemblyInfoOptions.AssemblyVersion)]
public static partial class ThisAssembly;
```

The generated constants use the corresponding .NET SDK property names:

```csharp
Console.WriteLine(ThisAssembly.AssemblyTitle);
Console.WriteLine(ThisAssembly.AssemblyVersion);
```

Custom `AssemblyMetadataAttribute` values are generated directly on the target type. For example, this project configuration:

```xml
<ItemGroup>
  <AssemblyAttribute Include="System.Reflection.AssemblyMetadataAttribute">
    <_Parameter1>BuildDate</_Parameter1>
    <_Parameter2>2026-08-17</_Parameter2>
  </AssemblyAttribute>
</ItemGroup>
```

generates a `ThisAssembly.BuildDate` constant. Metadata keys are converted to valid `C#` identifiers. The generator reports an error if a converted name conflicts with another metadata key or a standard generated constant.

Attributed targets and all containing types must be partial. Top-level, nested, generic, and non-generic classes and record classes are supported.
