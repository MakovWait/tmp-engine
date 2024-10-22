namespace Tmp.IO;

public readonly record struct FilePath(string UncheckedValue)
{
    // TODO
    private static string AppName => "Tmp";
    private static readonly string _userRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        AppName
    );

    public string Value
    {
        get
        {
            if (!Exists)
            {
                throw new FileNotFoundException($"The path {UncheckedValue} is not valid.");
            }
            return UncheckedValue;
        }
    }

    public string Extension => Path.GetExtension(Value)[1..];
    
    public bool Exists => File.Exists(UncheckedValue);

    public static implicit operator FilePath(string path) => FromStringPath(path);

    public static implicit operator string(FilePath self) => self.Value;

    public static FilePath FromStringPath(string path)
    {
        if (path.StartsWith("assets://"))
        {
            path = Path.Join(Environment.CurrentDirectory, "assets", path[9..]);
        }
        else if (path.StartsWith("user://"))
        {
            path = Path.Join(_userRoot, path[7..]);
        }

        return new FilePath(path);
    }
}