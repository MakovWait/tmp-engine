using Tmp.IO;

namespace Tmp.Resource;

public readonly record struct ResourcePath(FilePath FilePath, string Label)
{
    public string Extension => FilePath.Extension;

    public static implicit operator ResourcePath(string path) => FromPath(path);

    public static ResourcePath FromPath(string path)
    {
        var pathAndLabel = path.Split('#');
        return pathAndLabel.Length switch
        {
            1 => new ResourcePath(pathAndLabel[0], ""),
            2 => new ResourcePath(pathAndLabel[0], pathAndLabel[1]),
            _ => throw new ArgumentException($"Invalid path: {path}")
        };
    }
}