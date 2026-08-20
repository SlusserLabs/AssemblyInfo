namespace SlusserLabs.AssemblyInfo.PackageTests;

public static class Program
{
    public static int Main()
    {
        return Validate(PackageAssemblyInfo.AssemblyTitle, PackageAssemblyInfo.BuildCommit);
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
