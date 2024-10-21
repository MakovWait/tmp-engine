using Tmp.IO;

namespace Tmp.Asset;

public readonly record struct AssetPath(string RawPath, string Label)
{
    public FilePath FilePath => FilePath.FromStringPath(RawPath);

    public bool IsMem => RawPath.StartsWith("mem://");

    public bool HasLabel => !Label.Equals("");

    public string Extension => FilePath.Extension;

    public static implicit operator AssetPath(string path) => FromPath(path);

    public static AssetPath FromPath(string path)
    {
        var pathAndLabel = path.Split('#');
        return pathAndLabel.Length switch
        {
            1 => new AssetPath(pathAndLabel[0], ""),
            2 => new AssetPath(pathAndLabel[0], pathAndLabel[1]),
            _ => throw new ArgumentException($"Invalid path: {path}")
        };
    }
}