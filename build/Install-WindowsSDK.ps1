
$WinSdkTempDir = "C:\WinSdkTemp\"
$WinSdkSetupExe = "C:\WinSdkTemp\" + "WinSdkSetup.exe"

mkdir $WinSdkTempDir

$client = [System.Net.WebClient]::new()
$client.DownloadFile("https://go.microsoft.com/fwlink/p/?linkid=870807", $WinSdkSetupExe)

Start-Process -Wait $WinSdkSetupExe "/features OptionId.UWPCpp /q"