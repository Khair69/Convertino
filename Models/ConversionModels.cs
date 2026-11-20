namespace Convertino.Models;

public enum FileKind { Unknown, Image, Audio, Video, Document }

public sealed record SourceFile(string Path)
{
    public string Name => System.IO.Path.GetFileName(Path);
    public FileKind Kind => FileClassifier.Classify(Path);
}

public static class FileClassifier
{
    private static readonly HashSet<string> Images = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".webp"];
    private static readonly HashSet<string> Audio = [".mp3", ".wav", ".flac", ".m4a", ".aac", ".ogg", ".opus", ".wma"];
    private static readonly HashSet<string> Video = [".mp4", ".mov", ".mkv", ".webm", ".avi", ".wmv", ".m4v"];
    private static readonly HashSet<string> Documents = [".txt", ".md", ".html", ".htm", ".rtf", ".docx", ".pdf"];
    public static FileKind Classify(string path)
    {
        var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
        if (Images.Contains(ext)) return FileKind.Image;
        if (Audio.Contains(ext)) return FileKind.Audio;
        if (Video.Contains(ext)) return FileKind.Video;
        if (Documents.Contains(ext)) return FileKind.Document;
        return FileKind.Unknown;
    }
    public static IReadOnlyList<string> Targets(IEnumerable<SourceFile> files)
    {
        var kinds = files.Select(x => x.Kind).ToHashSet();
        if (kinds.Contains(FileKind.Video)) return ["mp4", "webm", "mkv", "mp3", "wav"];
        if (kinds.Contains(FileKind.Audio)) return ["mp3", "m4a", "wav", "flac", "ogg", "opus"];
        if (kinds.Contains(FileKind.Image)) return ["png", "jpg", "webp", "bmp", "tiff"];
        if (kinds.Contains(FileKind.Document)) return ["txt", "html"];
        return [];
    }
}
