import {$} from "bun";

const projectPath = 'C:\\oss\\SimEd'

const InnoSetupCompiler = `C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe`

async function buildProgramOnPlatform(platform) {
    await $`dotnet publish  ..\\..\\SimEd\\SimEd.csproj -r ${platform} -c Release -o ${platform} -p:PublishDir=.\\${platform}`;
    await $`${InnoSetupCompiler} installer-${platform}.iss`
    await $`mv ..\\tools\\${platform}\\mysetup.exe setup-simaed-${platform}.exe`
}

await buildProgramOnPlatform("win-x64");
await buildProgramOnPlatform("win-x86");
await buildProgramOnPlatform("win-arm64");
