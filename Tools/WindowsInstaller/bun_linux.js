#!bun
import {$} from "bun";

const SimEdVersion = "0.0.5"

async function buildProgramOnPlatform(platform) {
    await $`rm -rf ${platform}`
    await $`dotnet publish  ..\\..\\SimEd\\SimEd.csproj -r ${platform} -c Release -o ${platform} -p:PublishDir=.\\${platform}`
    var zipFileName = `simed-${SimEdVersion}-${platform}.zip`
    await $`rm -f ${zipFileName}`
    await $`zip -r ${zipFileName} ${platform}`
    await $`rm -rf ${platform}`
}

await buildProgramOnPlatform("linux-x64");
