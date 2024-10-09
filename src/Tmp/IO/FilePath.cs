namespace Tmp.IO;

public readonly record struct FilePath(string Value)
{
    // TODO
    private static string AppName => "Tmp";
    private static readonly string _userRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        AppName
    );
    
    public string Extension => Path.GetExtension(Value)[1..];

    public static implicit operator FilePath(string path) => FromStringPath(path);

    public static implicit operator string(FilePath self) => self.Value;

    public static FilePath FromStringPath(string path)
    {
        if (path.StartsWith("res://"))
        {
            path = Path.Join(Environment.CurrentDirectory, "res", path[6..]);
        }
        else if (path.StartsWith("user://"))
        {
            path = Path.Join(_userRoot, path[7..]);
        }

        var pathIsValid = File.Exists(path);
        if (!pathIsValid)
        {
            throw new FileNotFoundException($"The path {path} is not valid.");
        }

        return new FilePath(path);
    }
}