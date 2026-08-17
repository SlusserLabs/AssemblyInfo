namespace SlusserLabs.AssemblyInfo;

internal readonly record struct AssemblyMetadataEntry
{
    internal AssemblyMetadataEntry(string key, string value)
    {
        Key = key;
        Value = value;
    }

    internal string Key { get; }

    internal string Value { get; }

}
