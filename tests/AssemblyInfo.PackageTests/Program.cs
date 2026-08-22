namespace SlusserLabs.AssemblyInfo.PackageTests;

public static class Program
{
    public static int Main(string[] args)
    {
        // Generated constants prove that the analyzer loaded and processed the consumer assembly
        var result = Validate(PackageAssemblyInfo.AssemblyTitle, PackageAssemblyInfo.BuildCommit);
        if (result != 0)
        {
            return result;
        }

        // The marker attribute is a compile-only reference and must not become a runtime dependency
        const string attributesAssemblyName = "SlusserLabs.AssemblyInfo.Attributes";
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, $"{attributesAssemblyName}.dll")))
        {
            return 3;
        }

        if (typeof(Program).Assembly.GetReferencedAssemblies().Any(assembly => string.Equals(assembly.Name, attributesAssemblyName, StringComparison.Ordinal)))
        {
            return 4;
        }

        // CI supplies both archives to validate metadata that is not available in the built consumer
        if (args.Length == 2)
        {
            PackageValidator.Validate(args[0], args[1]);
        }

        return 0;
    }

    private static int Validate(string assemblyTitle, string buildCommit)
    {
        if (assemblyTitle != "AssemblyInfo.PackageTests")
        {
            return 1;
        }

        if (buildCommit != "abc123")
        {
            return 2;
        }

        return 0;
    }
}
