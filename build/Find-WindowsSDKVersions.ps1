$script:ns = 'http://schemas.microsoft.com/developer/msbuild/2003'

$ErrorActionPreference = 'Stop'

# Unique set of Windows SDK versions referenced in files
$versions = New-Object System.Collections.Generic.HashSet[System.String]

function Get-Nodes {
    param(
        [parameter(ValueFromPipeline = $true)]
        [xml] $xml,
        [parameter(Mandatory = $true)]
        [string] $nodeName)

    # Try the old style csproj. Also format required for .targets and .props files
    $n = Select-Xml -Xml $xml.Project -Namespace @{d = $ns } -XPath "//d:$nodeName"

    # Try the SDK-style files
    if (!$n) {
        $r = Select-Xml -Xml $xml.Project -XPath "//$nodeName"
    }

    return $r
}

function Get-NodeValue {
    param(
        [parameter(ValueFromPipeline = $true)]
        [xml] $xml,
        [string] $nodeName)

    $node = get-nodes $xml $nodeName

    if ($node) {
        if ($node.Node) {
            return [string]$node.Node.'#text'
        }
    }

    return [string]""
}

function Get-SdkVersion {
    param(
        [Parameter(ValueFromPipeline = $true)] $file)

    [xml] $xml = Get-Content $file

    # If you want a complete set of SDKs that are required, uncomment the following
    # $version = Get-NodeValue $xml 'PropertyGroup/TargetPlatformMinVersion'
    # $versions.Add($version) | Out-Null

    $version = Get-NodeValue $xml 'PropertyGroup/TargetPlatformVersion'
    $versions.Add($version) | Out-Null

    # Versions may also be specified without the 10.0.xxxxx.0 format in the
    # PropertyGroup/TargetPlatformRevision and PropertyGroup/TargetPlatformMinRevision

    # If you want a complete set of SDKs that are required, uncomment the following
    # $version = Get-NodeValue $xml 'PropertyGroup/TargetPlatformMinRevision'
    # $versions.Add("10.0." + $version + ".0") | Out-Null

    $version = Get-NodeValue $xml 'PropertyGroup/TargetPlatformRevision'
    $versions.Add("10.0." + $version + ".0") | Out-Null
}

function Test-RegistryPathAndValue {
    param (
        [parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $path,
        [parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $value)

    try {
        if (Test-Path $path) {
            Get-ItemProperty -Path $path | Select-Object -ExpandProperty $value -ErrorAction Stop | Out-Null
            return $true
        }
    }
    catch {
    }

    return $false
}

function Test-InstallWindowsSdk([string] $WindowsSDKVersion) {
    $retval = $true

    $WindowsSDKRegPath = "HKLM:\Software\Microsoft\Windows Kits\Installed Roots"
    $WindowsSDKRegRootKey = "KitsRoot10"
    $WindowsSDKOptions = @("OptionId.UWPCpp")

    $WindowsSDKInstalledRegPath = "$WindowsSDKRegPath\$WindowsSDKVersion\Installed Options"

    if (Test-RegistryPathAndValue -Path $WindowsSDKRegPath -Value $WindowsSDKRegRootKey) {
        # A Windows SDK is installed
        # Is an SDK of our version installed with the options we need?
        if (Test-RegistryPathAndValue -Path $WindowsSDKInstalledRegPath -Value "$WindowsSDKOptions") {
            # It appears we have what we need. Double check the disk
            $sdkRoot = Get-ItemProperty -Path $WindowsSDKRegPath | Select-Object -ExpandProperty $WindowsSDKRegRootKey
            if ($sdkRoot) {
                if (Test-Path $sdkRoot) {
                    $refPath = Join-Path $sdkRoot "References\$WindowsSDKVersion"
                    if (Test-Path $refPath) {
                        $umdPath = Join-Path $sdkRoot "UnionMetadata\$WindowsSDKVersion"
                        if (Test-Path $umdPath) {
                            # Pretty sure we have what we need
                            $retval = $false
                        }
                    }
                }
            }
        }
    }

    return $retval
}

if (!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

Write-Host -NoNewline "Locating referenced Windows SDK versions..."

Get-ChildItem *.csproj -Recurse | ForEach-Object { get-sdkversion $_ }
Get-ChildItem *.targets -Recurse | ForEach-Object { get-sdkversion $_ }
Get-ChildItem *.props -Recurse | ForEach-Object { get-sdkversion $_ }

Write-Host "Done"
Write-Host

$anyInstallRequired = $false;

foreach ($version in $versions) {
    if ($version -match "10\.0\.\d{5}\.0") {
        $installRequired = Test-InstallWindowsSDK $version
        Write-Host "Windows SDK '$version' install required: $installRequired"
        if ($installRequired) {
            # Automatically invoke Install-WindowsSDKIso.ps1 ?
            $anyInstallRequired = $true
        }
    }
}

Write-Host
if ($anyInstallRequired) {
    throw "At least one Windows SDK is missing from this machine"
}
else {
    Write-Host "All referenced Windows SDKs are installed!"
}