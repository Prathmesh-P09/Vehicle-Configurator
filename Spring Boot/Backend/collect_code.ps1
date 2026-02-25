$targetDir = "d:\CDAC\Vehicle Configurator\Backend\v-conf\src\main\java\com\example"
$outputFile = "d:\CDAC\Vehicle Configurator\Backend\v_conf_src_code.txt"
if (Test-Path $outputFile) { Remove-Item $outputFile }
$files = Get-ChildItem -Path $targetDir -Recurse -Filter *.java
foreach ($file in $files) {
    " " | Out-File -FilePath $outputFile -Append
    "==========================================" | Out-File -FilePath $outputFile -Append
    "File: $($file.FullName)" | Out-File -FilePath $outputFile -Append
    "==========================================" | Out-File -FilePath $outputFile -Append
    Get-Content $file.FullName | Out-File -FilePath $outputFile -Append
}
Write-Host "Done processing $($files.Count) files."
