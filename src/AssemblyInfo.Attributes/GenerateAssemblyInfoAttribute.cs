using System.Diagnostics;

namespace SlusserLabs.AssemblyInfo;

// SLUSSERLABS_ASSEMBLYINFO_USAGES keeps the marker available during compilation while
// preventing its usage from being emitted into consumer assembly metadata.
// See: https://andrewlock.net/creating-a-source-generator-part-7-solving-the-source-generator-marker-attribute-problem-part1/

/// <summary>
/// Marks a <c>class</c> or <c>record class</c> as a destination for generated assembly information.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[Conditional("SLUSSERLABS_ASSEMBLYINFO_USAGES")]
public sealed class GenerateAssemblyInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateAssemblyInfoAttribute" /> class.
    /// </summary>
    /// <param name="options">The assembly information to generate. The default is <c>All</c>.</param>
    public GenerateAssemblyInfoAttribute(GenerateAssemblyInfoOptions options = GenerateAssemblyInfoOptions.All)
    {
        Options = options;
    }

    /// <summary>
    /// Gets the assembly information to generate.
    /// </summary>
    public GenerateAssemblyInfoOptions Options { get; }
}
