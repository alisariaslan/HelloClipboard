; HelloClipboard Simple Installer Script

[Setup]
AppName=HelloClipboard
AppVersion=1.2.4.15
DefaultDirName={commonpf}\HelloClipboard
DefaultGroupName=HelloClipboard
OutputBaseFilename=HelloClipboard_Installer
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
DisableDirPage=yes

[Files]
; Main exe
Source: ".\HelloClipboard\bin\Release\net10.0-windows\publish\HelloClipboard.exe"; DestDir: "{app}"; Flags: ignoreversion

; Managed & native dll's
Source: ".\HelloClipboard\bin\Release\net10.0-windows\publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion restartreplace

; Framework JSON's (Required)
Source: ".\HelloClipboard\bin\Release\net10.0-windows\publish\HelloClipboard.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\HelloClipboard\bin\Release\net10.0-windows\publish\HelloClipboard.deps.json"; DestDir: "{app}"; Flags: ignoreversion


[Icons]
Name: "{group}\HelloClipboard"; Filename: "{app}\HelloClipboard.exe"
Name: "{commondesktop}\HelloClipboard"; Filename: "{app}\HelloClipboard.exe"

[Run]
Filename: "{app}\HelloClipboard.exe"; Description: "Launch HelloClipboard"; Flags: nowait postinstall skipifsilent
