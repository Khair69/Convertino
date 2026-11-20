# Convertino

Convertino is a small, local-first file converter for Windows. Drop files on the window, choose a target format, and the converted copy is written next to the original. Existing files are never overwritten; collisions become `name (2).ext`.

## Current support

- Images: PNG, JPEG, BMP and TIFF using Windows' built-in codecs
- Audio/video: MP3, M4A, WAV, FLAC, OGG, Opus, MP4, WebM and MKV through `ffmpeg`
- Batch conversion, drag-and-drop, command-line file arguments, progress and stop

### Important: ffmpeg is required for audio/video

The installer does **not** bundle ffmpeg. Install it separately and make sure `ffmpeg.exe` is available on your Windows `PATH`. Convertino will then be able to convert audio and video. Image conversion works without it.

To check whether it is installed, open PowerShell and run:

```powershell
ffmpeg -version
```

If PowerShell says the command is not recognized, install ffmpeg from its official Windows builds and add its `bin` folder (the folder containing `ffmpeg.exe`) to your user or system `PATH`. Restart Convertino after changing PATH.

No account, upload, telemetry or background service is used.

## Run from source

```powershell
dotnet run
```

To pass files from Explorer or another launcher:

```powershell
dotnet run -- "C:\Users\you\Videos\clip.mov"
```

## Publish a portable app

```powershell
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

The app is intentionally a native WPF executable with no third-party UI framework. The project is MIT-licensed. The installer also places this README beside the app as a reminder that ffmpeg is a separate installation.
