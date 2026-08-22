# Publishing packages locally

Use `dotnet pack` to build a local NuGet package. `dotnet publish` is for application deployment and is not used here. Run these commands from the repository root.

## Create the package

Give each test package a unique prerelease version. NuGet caches packages by ID and version, so reusing a version can load an older build.

```powershell
dotnet test AssemblyInfo.slnx --configuration Release
dotnet pack src/AssemblyInfo/AssemblyInfo.csproj --configuration Release --output artifacts/package -p:Version=1.0.0-local.1
Get-ChildItem artifacts/package
```

Leave off `-p:Version` to use `VersionPrefix` and `VersionSuffix` from `Directory.Build.props`.

## Add a local package source

If only one repository needs the local source, put a `NuGet.config` beside its solution. Change the relative path when the consumer lives outside this repository.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="AssemblyInfo.Local" value="artifacts/package" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

Add and restore the local version:

```powershell
dotnet add path/to/Consumer.csproj package SlusserLabs.AssemblyInfo --version 1.0.0-local.1
dotnet restore path/to/Consumer.csproj --configfile path/to/NuGet.config
```

To make the source available to every repository on the machine, register its absolute path for the current user:

```powershell
dotnet nuget add source (Resolve-Path artifacts/package).Path --name AssemblyInfo.Local
dotnet nuget list source
```

Remove the source when it is no longer needed:

```powershell
dotnet nuget remove source AssemblyInfo.Local
```

## Configure Visual Studio

Open **Tools > NuGet Package Manager > Package Manager Settings > Package Sources**. Add `AssemblyInfo.Local` and browse to the absolute `artifacts/package` folder. In **Manage NuGet Packages**, select that source, enable prerelease packages, and install the local version.

Increment the `local.N` suffix every time the package is rebuilt. If Visual Studio still shows an older package, close and reopen the package manager after packing the new version.
