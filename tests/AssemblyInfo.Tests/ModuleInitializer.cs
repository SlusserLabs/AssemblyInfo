using System.Runtime.CompilerServices;
using VerifyTests;

namespace SlusserLabs.AssemblyInfo.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifySourceGenerators.Initialize();
    }
}
