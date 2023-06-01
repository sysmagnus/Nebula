; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "nebula"
#define MyAppVersion "1.2.5"
#define MyAppPublisher "rd4Lab"
#define MyAppURL "https://rd4lab.com/"
#define MyAppExeName "abrirBandeja.bat"
#define MyAppDirPkg "C:\nebula.pkg"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{B6D4A4FF-9071-4796-B0F8-90C0BCB86728}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\{#MyAppName}
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=C:\nebula.pkg
OutputBaseFilename={#MyAppName}-v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Code]
var
  UploadsPath: string;
 
function NextButtonClick(CurPageID: Integer): Boolean;
var
  StoragePath: string;
begin
  Result := True;

  // Check if the D:\ drive exists
  if DirExists('D:\') then
    StoragePath := 'D:\storage'
  else
    StoragePath := 'C:\storage';

  // Create the storage folder if it doesn't exist
  if not DirExists(StoragePath) then
    CreateDir(StoragePath);

  // Create the uploads subfolder
  UploadsPath := StoragePath + '\uploads';
  if not DirExists(UploadsPath) then
    CreateDir(UploadsPath);
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  SourceFile: string;
  DestFile: string;
begin
  if CurStep = ssPostInstall then
  begin
    // Copy the image file to the uploads folder
    SourceFile := ExpandConstant('{app}\html\assets\default.jpg');
    DestFile := UploadsPath + '\default.jpg';
    if FileExists(SourceFile) then
      FileCopy(SourceFile, DestFile, False);
  end;
end;

[Files]
Source: "{#MyAppDirPkg}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\conf\*"; DestDir: "{app}\conf"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppDirPkg}\contrib\*"; DestDir: "{app}\contrib"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppDirPkg}\html\*"; DestDir: "{app}\html"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppDirPkg}\logs\*"; DestDir: "{app}\logs"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppDirPkg}\temp\*"; DestDir: "{app}\temp"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppDirPkg}\abrirBandeja.bat"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\appsettings.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\aspnetcorev2_inprocess.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\backupMongoDatabase.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\createSystemScheduledTask.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\e_sqlite3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\mongocrypt.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\Nebula.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\nginx.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppDirPkg}\nssm.exe"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: shellexec postinstall skipifsilent
Filename: "{app}\nssm.exe"; Parameters: "install nebula_nssm {app}\Nebula.exe";
Filename: "{app}\nssm.exe"; Parameters: "install nebula_html {app}\nginx.exe";
Filename: "{app}\nssm.exe"; Parameters: "set nebula_nssm AppDirectory {app}";
Filename: "{app}\nssm.exe"; Parameters: "start nebula_nssm";
Filename: "{app}\nssm.exe"; Parameters: "start nebula_html";

[UninstallRun]
Filename: "{app}\nssm.exe"; Parameters: "stop nebula_nssm";
Filename: "{app}\nssm.exe"; Parameters: "stop nebula_html";
Filename: "{app}\nssm.exe"; Parameters: "remove nebula_nssm";
Filename: "{app}\nssm.exe"; Parameters: "remove nebula_html";
