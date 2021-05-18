# USAGE: install.ps1 <DEBUG/RELEASE> <UUID>
# Example: install.ps1 RELEASE com.barraider.spotify

Param(
    [Parameter(Mandatory=$true)]
    $target,
    [Parameter(Mandatory=$true)]
    $uuid
)

Push-Location -Path "$target/"

# *** MAKE SURE THE FOLLOWING VARIABLES ARE CORRECT ***
# (Distribution tool be downloaded from: https://developer.elgato.com/documentation/stream-deck/sdk/exporting-your-plugin/ )
$output_dir = "C:/temp"
$distribution_tool = "C:/tools/DistributionTool.exe"

Stop-Process -Name "StreamDeck" -Force
Stop-Process -Name "$uuid" -Force

Start-Sleep -Seconds 2

Remove-Item -Path "$output_dir/$uuid.streamDeckPlugin" -Recurse -Force
Start-Process "$distribution_tool" -ArgumentList "-b -i $uuid.sdPlugin -o $output_dir" -NoNewWindow -Wait
Remove-Item -Path "$env:APPDATA/Elgato/StreamDeck/Plugins/$uuid.sdPlugin" -Recurse -Force

Invoke-Item -Path "$output_dir/$uuid.streamDeckPlugin"

Pop-Location