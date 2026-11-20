# Convertino

Convertino is a lightweight Windows desktop application for converting common image, audio, and video files locally. Files are processed on the computer and the original file is never overwritten.

## Features

- Drag and drop files into the application.
- Open files from Explorer or pass them as command-line arguments.
- Convert multiple files in one batch.
- Write converted files beside the originals.
- Automatically choose a new filename when the target already exists.
- Stop a conversion in progress.
- Add a `Convert with Convertino` entry to the Windows Explorer context menu during installation.
- Convert common image formats without installing any additional software.

## Supported formats

Image conversion is handled by Windows and supports PNG, JPEG, BMP, and TIFF.

Audio and video conversion is handled by `ffmpeg` and supports the target formats exposed by the application, including MP3, M4A, WAV, FLAC, OGG, Opus, MP4, WebM, and MKV.

Document conversion, PDF conversion, font conversion, and animated image conversion are not currently implemented.

## ffmpeg requirement

The Convertino installer does not bundle `ffmpeg`. This keeps the installer smaller and leaves users in control of the separate `ffmpeg` installation and its licensing.

You need `ffmpeg.exe` on the Windows `PATH` to convert audio or video. Image conversion works without it.

To check whether `ffmpeg` is available, open PowerShell and run:

```powershell
ffmpeg -version
```

If PowerShell reports that the command is not recognized, install an official Windows build of `ffmpeg`, add the folder containing `ffmpeg.exe` to the user or system `PATH`, and restart Convertino.

## Run from source

Requirements:

- Windows 10 or later
- .NET 10 SDK
- `ffmpeg` on `PATH` for audio and video conversion

Start the application with:

```powershell
dotnet run
```

Pass files directly from PowerShell or another launcher with:

```powershell
dotnet run -- "C:\Users\you\Videos\clip.mov"
```

## Build a self-contained executable

```powershell
dotnet restore -r win-x64
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

The executable is written to:

```text
bin\Release\net10.0-windows\win-x64\publish\Convertino.exe
```

## Build the installer

Install Inno Setup 7, build the self-contained executable, and then run:

```powershell
& "C:\Program Files\Inno Setup 7\ISCC.exe" Installer\Convertino.iss
```

The installer is written to the `dist` directory. It installs the application, README, shortcuts, uninstaller, and Explorer context-menu integration. It does not install `ffmpeg`.

## Privacy and licensing

Convertino has no account system, upload service, telemetry, or background service. Conversion is performed locally.

The project is licensed under the MIT License. `ffmpeg` is a separate project with its own builds and licensing terms; it is not included in this repository or installer.
