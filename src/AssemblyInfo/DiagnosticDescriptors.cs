using Microsoft.CodeAnalysis;

namespace SlusserLabs.AssemblyInfo;

internal static class DiagnosticDescriptors
{
    public const string TargetMustBePartialId = "SLAI001";
    public const string ContainingTypeMustBePartialId = "SLAI002";
    public const string UnsupportedTargetId = "SLAI003";
    public const string MetadataNameCollisionId = "SLAI004";
    public const string UndefinedOptionsId = "SLAI005";

    public static readonly DiagnosticDescriptor TargetMustBePartial = new(
        TargetMustBePartialId,
        "Assembly information target must be partial",
        "Type '{0}' must be partial to receive generated assembly information",
        "SlusserLabs.AssemblyInfo",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContainingTypeMustBePartial = new(
        ContainingTypeMustBePartialId,
        "Containing type must be partial",
        "Containing type '{0}' must be partial to generate assembly information for a nested type",
        "SlusserLabs.AssemblyInfo",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnsupportedTarget = new(
        UnsupportedTargetId,
        "Assembly information target is not supported",
        "Type '{0}' cannot receive generated assembly information",
        "SlusserLabs.AssemblyInfo",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MetadataNameCollision = new(
        MetadataNameCollisionId,
        "Assembly metadata name conflicts with a generated member",
        "Assembly metadata key '{0}' produces the duplicate member name '{1}'",
        "SlusserLabs.AssemblyInfo",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UndefinedOptions = new(
        UndefinedOptionsId,
        "Assembly information options contain undefined flags",
        "Type '{0}' uses undefined AssemblyInfoOptions flags: 0x{1}",
        "SlusserLabs.AssemblyInfo",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
