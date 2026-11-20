using Convertino.Models;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace Convertino.Services;

public sealed class ConversionService
{
    public async Task<string> ConvertAsync(SourceFile source, string target, IProgress<double> progress, CancellationToken token)
    {
        var output = NextOutput(source.Path, target);
        if (source.Kind is FileKind.Image && target is not "webp")
        {
            await Task.Run(() => ConvertImage(source.Path, output, target), token);
            progress.Report(1); return output;
        }
        if (source.Kind is FileKind.Audio or FileKind.Video || target is "webp")
        {
            await RunFfmpeg(source.Path, output, target, progress, token);
            return output;
        }
        if (target == "txt")
        {
            await Task.Run(() => File.Copy(source.Path, output), token);
            progress.Report(1); return output;
        }
        throw new InvalidOperationException("This file type needs a supported conversion tool.");
    }

    private static string NextOutput(string source, string target)
    {
        var dir = Path.GetDirectoryName(source)!;
        var stem = Path.Combine(dir, Path.GetFileNameWithoutExtension(source));
        var result = stem + "." + target;
        var number = 2;
        while (File.Exists(result)) result = $"{stem} ({number++}).{target}";
        return result;
    }

    private static void ConvertImage(string input, string output, string target)
    {
        var decoder = BitmapDecoder.Create(new Uri(input), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        BitmapEncoder encoder = target switch { "jpg" => new JpegBitmapEncoder(), "png" => new PngBitmapEncoder(), "bmp" => new BmpBitmapEncoder(), "tiff" => new TiffBitmapEncoder(), _ => new PngBitmapEncoder() };
        encoder.Frames.Add(decoder.Frames[0]);
        using var stream = File.Create(output);
        encoder.Save(stream);
    }

    private static async Task RunFfmpeg(string input, string output, string target, IProgress<double> progress, CancellationToken token)
    {
        var args = target switch { "mp3" => "-vn -b:a 192k", "m4a" => "-vn -c:a aac -b:a 192k", "wav" => "-vn -acodec pcm_s16le", "flac" => "-vn -c:a flac", "ogg" => "-vn -c:a libvorbis -q:a 5", "opus" => "-vn -c:a libopus -b:a 128k", "webm" => "-c:v libvpx-vp9 -crf 32 -c:a libopus", "mkv" => "-c copy", _ => "-c:v libx264 -crf 20 -preset medium -c:a aac -movflags +faststart" };
        var psi = new ProcessStartInfo("ffmpeg", $"-y -i \"{input}\" {args} \"{output}\"") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };
        using var process = Process.Start(psi) ?? throw new InvalidOperationException("ffmpeg could not be started.");
        using var registration = token.Register(() => { try { if (!process.HasExited) process.Kill(true); } catch { } });
        var progressValue = 0d;
        while (!token.IsCancellationRequested && await process.StandardError.ReadLineAsync(token) is not null) { progressValue = Math.Min(.95, progressValue + .01); progress.Report(progressValue); }
        await process.WaitForExitAsync(token);
        if (token.IsCancellationRequested) { TryDelete(output); throw new OperationCanceledException(token); }
        if (process.ExitCode != 0) { TryDelete(output); throw new InvalidOperationException("ffmpeg could not convert this file. Check that ffmpeg is installed."); }
        progress.Report(1);
    }
    private static void TryDelete(string path) { if (File.Exists(path)) File.Delete(path); }
}
