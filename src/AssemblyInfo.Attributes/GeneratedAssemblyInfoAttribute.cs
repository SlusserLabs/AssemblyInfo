using System.Diagnostics;

namespace SlusserLabs.AssemblyInfo;

// NOTE: The use of SLUSSERLABS_ASSEMBLYINFO_USAGES is trick to make the attribute
// available during compilation but omit it from the runtime dependencies.
// See: https://andrewlock.net/creating-a-source-generator-part-7-solving-the-source-generator-marker-attribute-problem-part1/

/// <summary>
/// Marks a <c>class</c> or <c>record class</c> as a destination for generated assembly information.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[Conditional("SLUSSERLABS_ASSEMBLYINFO_USAGES")]
public sealed class GeneratedAssemblyInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedAssemblyInfoAttribute" /> class.
    /// </summary>
    /// <param name="options">The assembly information to generate. The default is <c>All</c>.</param>
    public GeneratedAssemblyInfoAttribute(AssemblyInfoOptions options = AssemblyInfoOptions.All)
    {
        Options = options;
    }

    /// <summary>
    /// Gets the assembly information to generate.
    /// </summary>
    public AssemblyInfoOptions Options { get; }
}
