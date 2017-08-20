![Logo](favicon.ico)
# Puppy.Cleaner
> Project Created by [**Top Nguyen**](http://topnguyen.net)

This tool help Cleanup C# project

- Remove Visual Studio `MEF Cache` (Component Model Cache)

- Remove Website Cache: `%LocalAppData%\Microsoft\WebsiteCache`

- Remove Temp ASP.NET Files: `{Root}\Windows\Microsoft.NET\Framework\{Version}\Temporary ASP.NET Files`

- Remove Team Foundation Server Cache: `%LocalAppData%\Microsoft\Team Foundation\{Version}\Cache`

- Remove All thing in Temporary Folder: `%Temp%` including `%Temp%\VWDWebCache`.

- Default Remove by Reg Pattern:
    -  ".vs"
    - "packages"
    - "bin"
    - "obj"
    - "bld"
    - "Backup"
    - "_UpgradeReport_Files"
    - "ipch"
    - "Debug"
    - "Release"
    - "*.suo",
    - "*.user"

- Support Add Extra Pattern on Runtime.

*Require .Net Framework 4.5.2*