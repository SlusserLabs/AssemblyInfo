# Release process

The repository has one package workflow: `.github/workflows/nuget.yml`. It runs the .NET 10 build and tests, checks the package, and handles publishing.

## Workflow overview

| Trigger | Version | Result |
| --- | --- | --- |
| Pull request to `main` | `Directory.Build.props` | Tests and checks the package, then uploads an artifact |
| Push to `main` | `Directory.Build.props` | Same validation as a pull request |
| Manual run | `<VersionPrefix>-ci.<run number>` | Validates and publishes to the NuGet integration feed |
| Published GitHub Release | Release tag without the leading `v` | Validates and, when enabled, publishes to nuget.org and attaches packages to the release |

The `verify` job starts with `dotnet test`, which restores and builds the solution before running the TUnit tests. It packs the NuGet package, opens it, and checks the version, repository metadata, bundled files, assemblies, and symbols. It then restores a clean consumer using only the new package. GitHub stores the `.nupkg` and `.snupkg` files in the `nuget-package` workflow artifact.

A manual run uses the `ci.<run number>` suffix and sends the package to the NuGet integration feed through trusted publishing. Start it from **Actions > Build and publish NuGet package > Run workflow**. The button is available because `workflow_dispatch` is present on the default branch.

The `publish-nuget` job only starts when the repository variable `NUGET_PUBLISH_ENABLED` is `true`. It publishes through the `nuget-release` environment and trusted publishing, then attaches the packages and `SHA256SUMS` to the GitHub Release.

## Cut a release

1. Update the version in `Directory.Build.props`.

   Stable release:

   ```xml
   <VersionPrefix>1.1.0</VersionPrefix>
   <VersionSuffix></VersionSuffix>
   ```

   Prerelease:

   ```xml
   <VersionPrefix>1.1.0</VersionPrefix>
   <VersionSuffix>beta.1</VersionSuffix>
   ```

2. If analyzer diagnostics changed, move their entries from `AnalyzerReleases.Unshipped.md` into a matching release section in `AnalyzerReleases.Shipped.md`.

3. Test and pack the release commit locally. Make sure the version matches the tag you plan to create.

   ```powershell
   dotnet test AssemblyInfo.slnx --configuration Release
   dotnet pack src/AssemblyInfo/AssemblyInfo.csproj --configuration Release --output artifacts/package
   ```

4. Commit and push the version change. Then create and push an annotated tag. Use `v1.2.3` for a stable release or a SemVer prerelease such as `v1.2.3-beta.1`.

   ```powershell
   git add Directory.Build.props src/AssemblyInfo/AnalyzerReleases.*.md
   git commit -m "Prepare 1.1.0 release"
   git push origin main
   git tag -a v1.1.0 -m "v1.1.0"
   git push origin v1.1.0
   ```

5. Create a GitHub Release from the tag, add the release notes, and publish it. Pushing the tag will not start this workflow. The GitHub Release `published` event starts it.

6. Check that `verify` and `publish-nuget` passed. Confirm that the package is on nuget.org and that the GitHub Release has both packages and `SHA256SUMS`.

For release runs, the workflow removes the leading `v` from the tag and passes the result to `dotnet pack` as `-p:Version=<tag version>`. Local builds and normal CI still read the version from `Directory.Build.props`, so the file and tag should agree.
