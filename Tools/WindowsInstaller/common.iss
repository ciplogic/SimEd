
[Setup]
AppName=Sim(ple) Ed(itor)
AppVersion=0.0.5
WizardStyle=modern
DefaultDirName={autopf}\SimpleEditor
DefaultGroupName=Simple Editor
UninstallDisplayIcon={app}\SimEd.exe
Compression=lzma2
SolidCompression=yes


[Files]
Source: "SimEd.exe"; DestDir: "{app}"
Source: "*.dll"; DestDir: "{app}"

[Icons]
Name: "{group}\Sim(ple) Ed(itor)"; Filename: "{app}\SimEd.exe"


