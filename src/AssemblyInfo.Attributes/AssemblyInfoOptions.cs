using System.Reflection;

namespace SlusserLabs.AssemblyInfo;

/// <summary>
/// Specifies the assembly information included in generated output. Each singular option occupies one bit so values can be combined without overlap.
/// </summary>
[Flags]
public enum AssemblyInfoOptions
{
    /// <summary>
    /// Generates no assembly information.
    /// </summary>
    None = 0,

    /// <summary>
    /// Generates the assembly configuration.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyConfigurationAttribute" /> and the <c>Configuration</c> MSBuild property.</remarks>
    AssemblyConfiguration = 1 << 0,

    /// <summary>
    /// Generates the assembly company.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyCompanyAttribute" /> and the <c>Company</c> MSBuild property.</remarks>
    AssemblyCompany = 1 << 1,

    /// <summary>
    /// Generates the assembly title.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyTitleAttribute" /> and the <c>AssemblyTitle</c> MSBuild property.</remarks>
    AssemblyTitle = 1 << 2,

    /// <summary>
    /// Generates the assembly description.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyDescriptionAttribute" /> and the <c>Description</c> MSBuild property.</remarks>
    AssemblyDescription = 1 << 3,

    /// <summary>
    /// Generates the assembly product.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyProductAttribute" /> and the <c>Product</c> MSBuild property.</remarks>
    AssemblyProduct = 1 << 4,

    /// <summary>
    /// Generates the assembly copyright.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyCopyrightAttribute" /> and the <c>Copyright</c> MSBuild property.</remarks>
    AssemblyCopyright = 1 << 5,

    /// <summary>
    /// Generates the assembly version.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyVersionAttribute" /> and the <c>AssemblyVersion</c> MSBuild property.</remarks>
    AssemblyVersion = 1 << 6,

    /// <summary>
    /// Generates the informational version.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyInformationalVersionAttribute" /> and the <c>InformationalVersion</c> MSBuild property.</remarks>
    AssemblyInformationalVersion = 1 << 7,

    /// <summary>
    /// Generates the file version.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyFileVersionAttribute" /> and the <c>FileVersion</c> MSBuild property.</remarks>
    AssemblyFileVersion = 1 << 8,

    /// <summary>
    /// Generates custom assembly metadata.
    /// </summary>
    /// <remarks>This corresponds to <see cref="AssemblyMetadataAttribute" /> and the <c>AssemblyAttribute</c> MSBuild item.</remarks>
    AssemblyMetadata = 1 << 9,

    /// <summary>
    /// Generates all standard assembly attributes.
    /// </summary>
    AllAssemblyAttributes =
        AssemblyConfiguration |
        AssemblyCompany |
        AssemblyTitle |
        AssemblyDescription |
        AssemblyProduct |
        AssemblyCopyright |
        AssemblyVersion |
        AssemblyInformationalVersion |
        AssemblyFileVersion,

    /// <summary>
    /// Generates all available assembly information.
    /// </summary>
    All = AllAssemblyAttributes | AssemblyMetadata,
}
