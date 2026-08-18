namespace SlusserLabs.AssemblyInfo.Infrastructure;

internal readonly record struct StringPair
{
    public StringPair(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }

    public string Value { get; }
}
