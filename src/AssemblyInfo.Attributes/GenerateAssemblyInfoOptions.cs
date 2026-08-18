using System.Reflection;

namespace SlusserLabs.AssemblyInfo;

/// <summary>
/// Specifies the information included in generated output.
/// </summary>
[Flags]
public enum GenerateAssemblyInfoOptions
{
    /// <summary>
    /// Generates no assembly information.
    /// </summary>
    None = 0,

    /// <summary>
    /// Generates the assembly configuration.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyConfigurationAttribute" /> and <c>Configuration</c> MSBuild property.</remarks>
    AssemblyConfiguration = 1 << 0,

    /// <summary>
    /// Generates the assembly company.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyCompanyAttribute" /> and <c>Company</c> MSBuild property.</remarks>
    AssemblyCompany = 1 << 1,

    /// <summary>
    /// Generates the assembly title.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyTitleAttribute" /> and <c>AssemblyTitle</c> MSBuild property.</remarks>
    AssemblyTitle = 1 << 2,

    /// <summary>
    /// Generates the assembly description.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyDescriptionAttribute" /> and <c>Description</c> MSBuild property.</remarks>
    AssemblyDescription = 1 << 3,

    /// <summary>
    /// Generates the assembly product.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyProductAttribute" /> and <c>Product</c> MSBuild property.</remarks>
    AssemblyProduct = 1 << 4,

    /// <summary>
    /// Generates the assembly copyright.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyCopyrightAttribute" /> and <c>Copyright</c> MSBuild property.</remarks>
    AssemblyCopyright = 1 << 5,

    /// <summary>
    /// Generates the assembly version.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyVersionAttribute" /> and <c>AssemblyVersion</c> MSBuild property.</remarks>
    AssemblyVersion = 1 << 6,

    /// <summary>
    /// Generates the informational version.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyInformationalVersionAttribute" /> and <c>InformationalVersion</c> MSBuild property.</remarks>
    AssemblyInformationalVersion = 1 << 7,

    /// <summary>
    /// Generates the file version.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyFileVersionAttribute" /> and <c>FileVersion</c> MSBuild property.</remarks>
    AssemblyFileVersion = 1 << 8,

    /// <summary>
    /// Generates custom assembly metadata for each metadata property.
    /// </summary>
    /// <remarks>This corresponds to the <see cref="AssemblyMetadataAttribute" /> and <c>AssemblyAttribute</c> MSBuild item.</remarks>
    AssemblyMetadata = 1 << 9,

    /// <summary>
    /// Generates all standard assembly attributes.
    /// </summary>
    AllAssemblyAttributes = AssemblyConfiguration
         | AssemblyCompany
         | AssemblyTitle
         | AssemblyDescription
         | AssemblyProduct
         | AssemblyCopyright
         | AssemblyVersion
         | AssemblyInformationalVersion
         | AssemblyFileVersion,

    /// <summary>
    /// Generates all available assembly information.
    /// </summary>
    All = AllAssemblyAttributes | AssemblyMetadata,
}
