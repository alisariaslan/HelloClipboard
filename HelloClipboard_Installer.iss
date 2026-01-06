; HelloClipboard Simple Installer Script

[Setup]
AppName=HelloClipboard
AppVersion=1.1.0.8
DefaultDirName={commonpf}\HelloClipboard
DefaultGroupName=HelloClipboard
OutputBaseFilename=HelloClipboard_Installer
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
DisableDirPage=yes

[Files]
; Main executables and config
Source: ".\HelloClipboard\bin\Release\HelloClipboard.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\HelloClipboard\bin\Release\HelloClipboard.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\HelloClipboard\bin\Release\HelloClipboard.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion

; DLLs and support files
Source: ".\HelloClipboard\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion restartreplace
Source: ".\HelloClipboard\bin\Release\*.pdb"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\HelloClipboard"; Filename: "{app}\HelloClipboard.exe"
Name: "{commondesktop}\HelloClipboard"; Filename: "{app}\HelloClipboard.exe"

[Run]
Filename: "{app}\HelloClipboard.exe"; Description: "Launch HelloClipboard"; Flags: nowait postinstall skipifsilent
