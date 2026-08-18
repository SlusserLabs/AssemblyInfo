using System.Runtime.CompilerServices;

namespace SlusserLabs.AssemblyInfo.Tests.Infrastructure;

public static class ModuleInitializer
{
    // Verify's pattern for initialization.
    // See: https://github.com/VerifyTests/Verify.SourceGenerators/tree/main#initialize
    [ModuleInitializer]
    public static void Init()
    {
        UseProjectRelativeDirectory("Snapshots");
        VerifySourceGenerators.Initialize();
    }
}
