#!bun
import {$} from "bun";

const ProjectPath = 'C:\\oss\\SimEd'
const SimEdVersion = "0.0.4"

const InnoSetupCompiler = `C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe`

async function buildProgramOnPlatform(platform) {
    await $`dotnet publish  ..\\..\\SimEd\\SimEd.csproj -r ${platform} -c Release -o ${platform} -p:PublishDir=.\\${platform}`
    await $`${InnoSetupCompiler} installer-${platform}.iss`
    await $`mv ..\\tools\\${platform}\\mysetup.exe setup-simaed-${SimEdVersion}-${platform}.exe`
}

await buildProgramOnPlatform("win-x64");
await buildProgramOnPlatform("win-x86");
await buildProgramOnPlatform("win-arm64");
