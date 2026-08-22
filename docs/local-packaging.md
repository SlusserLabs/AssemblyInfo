# Building packages locally

Use `dotnet pack` to create a local NuGet package. `dotnet publish` deploys applications and is not used for package creation. Run the commands below from the repository root.

## Create the package

Assign a unique prerelease version to each local build. NuGet caches packages by ID and version, so rebuilding the same version can cause a test project to use stale files.

```powershell
$version = '1.0.0-local.1'
dotnet pack src/AssemblyInfo/AssemblyInfo.csproj --configuration Release --output artifacts/package -p:Version=$version
Get-ChildItem artifacts/package/SlusserLabs.AssemblyInfo.$version.*
```

The command creates both files:

```text
SlusserLabs.AssemblyInfo.1.0.0-local.1.nupkg
SlusserLabs.AssemblyInfo.1.0.0-local.1.snupkg
```

Leave off `-p:Version` to use `VersionPrefix` and `VersionSuffix` from `Directory.Build.props`. Increment `local.N` before rebuilding a package that has already been restored by a test project.

## Inspect the package

Keep the `.nupkg` and `.snupkg` together in the same directory. Open the `.nupkg` in NuGet Package Explorer and confirm that it contains:

```text
analyzers/dotnet/cs/netstandard2.0/SlusserLabs.AssemblyInfo.dll
analyzers/dotnet/cs/netstandard2.0/SlusserLabs.AssemblyInfo.Attributes.dll
ref/netstandard2.0/SlusserLabs.AssemblyInfo.Attributes.dll
ref/netstandard2.0/SlusserLabs.AssemblyInfo.Attributes.xml
```

The package should not contain a `lib` or `runtimes` DLL. Package Explorer reports green "No files found to validate" health results because it does not inspect assemblies in `analyzers` or `ref`.

Open the `.snupkg` separately if you want to inspect its contents. It should contain a matching PDB under `analyzers/dotnet/cs/netstandard2.0` for each analyzer DLL.

## Test the package in another project

Use the package output directory as a temporary source. Replace the project path and increment the local version as needed.

```powershell
$version = '1.0.0-local.1'
$packageSource = (Resolve-Path artifacts/package).Path
$consumerProject = 'C:\path\to\Consumer.csproj'

dotnet add $consumerProject package SlusserLabs.AssemblyInfo --version $version --source $packageSource
dotnet build $consumerProject
```

Confirm that the generator runs and produces the expected constants. `SlusserLabs.AssemblyInfo.Attributes.dll` should not appear in the consumer's output directory or `.deps.json` file.

## Configure Visual Studio

Open **Tools > NuGet Package Manager > Package Manager Settings > Package Sources**. Add a source named `AssemblyInfo.Local` that points to the absolute `artifacts/package` directory. In **Manage NuGet Packages**, select that source, enable prerelease packages, and install the local version.

Increment the `local.N` suffix every time you rebuild. If Visual Studio still shows an older package, close and reopen the package manager after creating the new version.
