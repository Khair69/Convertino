#define MyAppName "Convertino"
#define MyAppVersion "0.1.0"
#define MyAppPublisher "Convertino"
#define MyAppExeName "Convertino.exe"

[Setup]
AppId={{D8A6B8F5-6C3F-4B5E-A786-4D0E9F4F6A01}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\Convertino
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=..\dist
OutputBaseFilename=Convertino-Setup-{#MyAppVersion}-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}
LicenseFile=..\LICENSE.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "..\bin\Release\net10.0-windows\win-x64\publish\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\README.md"; DestDir: "{app}"; DestName: "README.md"; Flags: ignoreversion

[Icons]
Name: "{group}\Convertino"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Convertino README"; Filename: "{app}\README.md"
Name: "{autodesktop}\Convertino"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\*\shell\Convertino"; ValueType: string; ValueName: "MUIVerb"; ValueData: "Convert with Convertino"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\*\shell\Convertino"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\{#MyAppExeName}"
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\*\shell\Convertino\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch Convertino"; Flags: nowait postinstall skipifsilent
