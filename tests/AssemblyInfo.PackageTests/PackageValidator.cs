using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

namespace SlusserLabs.AssemblyInfo.PackageTests;

internal static class PackageValidator
{
    private const string _analyzerPath = "analyzers/dotnet/cs/netstandard2.0";

    // Roslyn portable PDB custom debug information identifiers
    private static readonly Guid _compilerFlagsId = new("B5FEEC05-8CD0-4A83-96DA-466284BB4BD8");
    private static readonly Guid _metadataReferencesId = new("7E4D4708-096E-4C5C-AEDA-CB10BA6A740D");
    private static readonly Guid _sourceLinkId = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");

    public static void Validate(string packagePath, string symbolsPackagePath)
    {
        using var package = ZipFile.OpenRead(packagePath);
        using var symbolsPackage = ZipFile.OpenRead(symbolsPackagePath);

        // Source Link URLs must identify the same commit recorded in the package metadata
        var nuspec = package.Entries.Single(entry => entry.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
        using var nuspecStream = nuspec.Open();
        var document = XDocument.Load(nuspecStream);
        var repositoryCommit = document.Descendants().Single(element => element.Name.LocalName == "repository").Attribute("commit")?.Value;
        if (repositoryCommit is null)
        {
            throw new InvalidDataException("The package repository commit is missing.");
        }

        // Every analyzer assembly must have a matching portable PDB with reproducible build metadata
        ValidateAssembly(package, symbolsPackage, $"{_analyzerPath}/SlusserLabs.AssemblyInfo.dll", repositoryCommit);
        ValidateAssembly(package, symbolsPackage, $"{_analyzerPath}/SlusserLabs.AssemblyInfo.Attributes.dll", repositoryCommit);
    }

    private static void ValidateAssembly(ZipArchive package, ZipArchive symbolsPackage, string assemblyPath, string repositoryCommit)
    {
        var pdbPath = Path.ChangeExtension(assemblyPath, ".pdb").Replace('\\', '/');
        var assemblyBytes = ReadEntry(package, assemblyPath);
        var pdbBytes = ReadEntry(symbolsPackage, pdbPath);

        using var peReader = new PEReader(new MemoryStream(assemblyBytes));
        var debugEntries = peReader.ReadDebugDirectory();

        if (!debugEntries.Any(entry => entry.Type == DebugDirectoryEntryType.Reproducible))
        {
            throw new InvalidDataException($"{assemblyPath} is not marked reproducible.");
        }

        var codeViewEntry = debugEntries.Single(entry => entry.Type == DebugDirectoryEntryType.CodeView);
        var codeView = peReader.ReadCodeViewDebugDirectoryData(codeViewEntry);
        var checksumEntry = debugEntries.Single(entry => entry.Type == DebugDirectoryEntryType.PdbChecksum);
        var checksum = peReader.ReadPdbChecksumDebugDirectoryData(checksumEntry);

        using var provider = MetadataReaderProvider.FromPortablePdbStream(new MemoryStream(pdbBytes));
        var reader = provider.GetMetadataReader();
        var debugMetadataHeader = reader.DebugMetadataHeader;
        if (debugMetadataHeader is null)
        {
            throw new InvalidDataException($"{pdbPath} has no portable PDB identifier.");
        }

        var pdbId = debugMetadataHeader.Id;
        var pdbGuid = new Guid(pdbId.Take(16).ToArray());

        // The portable PDB checksum treats its 20-byte content ID as zeroes
        using var hash = IncrementalHash.CreateHash(new HashAlgorithmName(checksum.AlgorithmName));
        var idOffset = debugMetadataHeader.IdStartOffset;
        hash.AppendData(pdbBytes, 0, idOffset);
        hash.AppendData(new byte[20]);
        hash.AppendData(pdbBytes, idOffset + 20, pdbBytes.Length - idOffset - 20);

        if (!hash.GetHashAndReset().SequenceEqual(checksum.Checksum))
        {
            throw new InvalidDataException($"{pdbPath} does not match {assemblyPath}.");
        }

        if (pdbGuid != codeView.Guid)
        {
            throw new InvalidDataException($"{pdbPath} has a different identifier than {assemblyPath}.");
        }

        var customDebugInformation = reader.CustomDebugInformation
            .Select(reader.GetCustomDebugInformation)
            .Where(information => information.Parent.Kind == HandleKind.ModuleDefinition)
            .ToDictionary(information => reader.GetGuid(information.Kind), information => reader.GetBlobBytes(information.Value));

        if (!customDebugInformation.ContainsKey(_compilerFlagsId) || !customDebugInformation.ContainsKey(_metadataReferencesId))
        {
            throw new InvalidDataException($"{pdbPath} does not contain compiler reproducibility metadata.");
        }

        if (!customDebugInformation.TryGetValue(_sourceLinkId, out var sourceLinkBytes))
        {
            throw new InvalidDataException($"{pdbPath} does not contain Source Link metadata.");
        }

        using var sourceLink = JsonDocument.Parse(sourceLinkBytes);
        if (!sourceLink.RootElement.TryGetProperty("documents", out var documents)
            || !documents.EnumerateObject().Any()
            || documents.EnumerateObject().Any(document => !document.Value.GetString()!.Contains($"/{repositoryCommit}/", StringComparison.Ordinal)))
        {
            throw new InvalidDataException($"{pdbPath} contains invalid Source Link metadata.");
        }
    }

    private static byte[] ReadEntry(ZipArchive archive, string path)
    {
        var entry = archive.GetEntry(path);
        if (entry is null)
        {
            throw new InvalidDataException($"{path} is missing from the package.");
        }

        using var stream = entry.Open();
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);

        return buffer.ToArray();
    }
}
