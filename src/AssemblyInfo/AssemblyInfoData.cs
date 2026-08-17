using System.Reflection;

using Microsoft.CodeAnalysis;

namespace SlusserLabs.AssemblyInfo;

internal sealed record AssemblyInfoData
{
    private const string _assemblyConfigurationAttributeName = "System.Reflection." + nameof(AssemblyConfigurationAttribute);
    private const string _assemblyCompanyAttributeName = "System.Reflection." + nameof(AssemblyCompanyAttribute);
    private const string _assemblyTitleAttributeName = "System.Reflection." + nameof(AssemblyTitleAttribute);
    private const string _assemblyDescriptionAttributeName = "System.Reflection." + nameof(AssemblyDescriptionAttribute);
    private const string _assemblyProductAttributeName = "System.Reflection." + nameof(AssemblyProductAttribute);
    private const string _assemblyCopyrightAttributeName = "System.Reflection." + nameof(AssemblyCopyrightAttribute);
    private const string _assemblyVersionAttributeName = "System.Reflection." + nameof(AssemblyVersionAttribute);
    private const string _assemblyInformationalVersionAttributeName = "System.Reflection." + nameof(AssemblyInformationalVersionAttribute);
    private const string _assemblyFileVersionAttributeName = "System.Reflection." + nameof(AssemblyFileVersionAttribute);
    private const string _assemblyMetadataAttributeName = "System.Reflection." + nameof(AssemblyMetadataAttribute);

    private AssemblyInfoData(
        string? configuration,
        string? company,
        string? title,
        string? description,
        string? product,
        string? copyright,
        string? version,
        string? informationalVersion,
        string? fileVersion,
        EquatableArray<AssemblyMetadataEntry> metadata)
    {
        Configuration = configuration;
        Company = company;
        Title = title;
        Description = description;
        Product = product;
        Copyright = copyright;
        Version = version;
        InformationalVersion = informationalVersion;
        FileVersion = fileVersion;
        Metadata = metadata;
    }

    internal string? Configuration { get; }

    internal string? Company { get; }

    internal string? Title { get; }

    internal string? Description { get; }

    internal string? Product { get; }

    internal string? Copyright { get; }

    internal string? Version { get; }

    internal string? InformationalVersion { get; }

    internal string? FileVersion { get; }

    internal EquatableArray<AssemblyMetadataEntry> Metadata { get; }

    internal static AssemblyInfoData Create(Compilation compilation)
    {
        string? configuration = default;
        string? company = default;
        string? title = default;
        string? description = default;
        string? product = default;
        string? copyright = default;
        string? version = default;
        string? informationalVersion = default;
        string? fileVersion = default;
        var metadata = new List<AssemblyMetadataEntry>();

        // SDK-generated assembly information is already available on the compilation
        foreach (var attribute in compilation.Assembly.GetAttributes())
        {
            switch (attribute.AttributeClass?.ToDisplayString())
            {
                case _assemblyConfigurationAttributeName:
                    configuration = GetString(attribute, 0);
                    break;
                case _assemblyCompanyAttributeName:
                    company = GetString(attribute, 0);
                    break;
                case _assemblyTitleAttributeName:
                    title = GetString(attribute, 0);
                    break;
                case _assemblyDescriptionAttributeName:
                    description = GetString(attribute, 0);
                    break;
                case _assemblyProductAttributeName:
                    product = GetString(attribute, 0);
                    break;
                case _assemblyCopyrightAttributeName:
                    copyright = GetString(attribute, 0);
                    break;
                case _assemblyVersionAttributeName:
                    version = GetString(attribute, 0);
                    break;
                case _assemblyInformationalVersionAttributeName:
                    informationalVersion = GetString(attribute, 0);
                    break;
                case _assemblyFileVersionAttributeName:
                    fileVersion = GetString(attribute, 0);
                    break;
                case _assemblyMetadataAttributeName:
                    AddMetadata(attribute, metadata);
                    break;
            }
        }

        return new AssemblyInfoData(
            configuration,
            company,
            title,
            description,
            product,
            copyright,
            version,
            informationalVersion,
            fileVersion,
            new EquatableArray<AssemblyMetadataEntry>(metadata.ToArray()));
    }

    private static void AddMetadata(AttributeData attribute, List<AssemblyMetadataEntry> metadata)
    {
        var key = GetString(attribute, 0);
        var value = GetString(attribute, 1);

        if (key is not null && value is not null)
        {
            metadata.Add(new AssemblyMetadataEntry(key, value));
        }
    }

    private static string? GetString(AttributeData attribute, int index)
    {
        return attribute.ConstructorArguments.Length > index ? attribute.ConstructorArguments[index].Value as string : default;
    }

}
