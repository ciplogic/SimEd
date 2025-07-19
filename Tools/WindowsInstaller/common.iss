
[Setup]
AppName=Sim(ple) Ed(itor)
AppVersion=0.0.5
WizardStyle=modern
DefaultDirName={autopf}\SimpleEditor
DefaultGroupName=Simple Editor
UninstallDisplayIcon={app}\SimEd.exe
Compression=lzma
SolidCompression=yes

[Tasks]
Name: StartAfterInstall; Description: Run application after install

[Files]
Source: "SimEd.exe"; DestDir: "{app}"; Tasks: StartAfterInstall
Source: "*.dll"; DestDir: "{app}"

[Icons]
Name: "{group}\Sim(ple) Ed(itor)"; Filename: "{app}\SimEd.exe"


