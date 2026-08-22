# Project overview

AssemblyInfo is an incremental Roslyn source generator. It turns assembly attributes and custom `AssemblyMetadataAttribute` values into compile-time constants, without using runtime reflection. A consumer chooses the destination by adding `GenerateAssemblyInfoAttribute` to a partial `class` or `record`.

## Projects

- `src/AssemblyInfo.Attributes` targets `netstandard2.0`. It defines `GenerateAssemblyInfoAttribute` and `GenerateAssemblyInfoOptions`. The project is not packed on its own; the main package includes its assembly for the compiler and consumer.
- `src/AssemblyInfo` targets `netstandard2.0` and contains the generator, diagnostics, metadata model, and source templates. This is the only packable project. It produces `SlusserLabs.AssemblyInfo`.
- `tests/AssemblyInfo.Tests` targets `net10.0` and uses TUnit with Verify source-generator snapshots. It belongs to `AssemblyInfo.slnx` and runs through the normal `dotnet test` command.
- `tests/AssemblyInfo.PackageTests` targets `net10.0` and is a clean consumer application. Its local `NuGet.config` restores the packed package, then the project compiles and checks the generated constants. This project stays outside the solution's unit-test run because the GitHub workflow runs it after packing.

## Testing and snapshots

Run the normal test suite from the repository root:

```powershell
dotnet test AssemblyInfo.slnx
```

`GeneratorTestHelper` compiles an in-memory consumer, runs `AssemblyInfoGenerator`, checks any compilation errors, and passes the result to Verify. Verify keeps the accepted output under `tests/AssemblyInfo.Tests/Snapshots`:

```text
*.g.verified.cs   Generated C# source
*.verified.txt    Diagnostics and generator results
```

When generator output changes on purpose, run the affected test and inspect the received and verified files. Accept the snapshot after checking each generated line and diagnostic. `.gitattributes` forces verified snapshots to LF line endings, which keeps them consistent on Windows and Linux.

The other TUnit tests cover metadata extraction, equality, option flags, and identifier sanitization. The package consumer test catches packaging problems that project-reference tests cannot because it uses the actual `.nupkg`.

To reproduce that integration test locally after packing version `1.0.0-local.1`:

```powershell
dotnet restore tests/AssemblyInfo.PackageTests/AssemblyInfo.PackageTests.csproj --configfile tests/AssemblyInfo.PackageTests/NuGet.config -p:AssemblyInfoPackageVersion=1.0.0-local.1
dotnet run --project tests/AssemblyInfo.PackageTests/AssemblyInfo.PackageTests.csproj --configuration Release --no-restore -p:AssemblyInfoPackageVersion=1.0.0-local.1
```
