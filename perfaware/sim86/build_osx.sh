#!/usr/bin/env pwsh

Set-StrictMode -Version Latest

if (-not $IsMacOs)
{
  Write-Error "Not Linux";
  Exit 1;
}

$previousDir = Get-Location

# Every command will be relative to this build folder
$directoryName = "build"
New-Item -ItemType Directory -Path $directoryName -Force | Out-Null
Set-Location $directoryName

$compiler = "clang"
$lib = "-dynamiclib"
$libExt = ".dylib"
$libExt = ".so"
$libPrefix = "lib"
$debugFlags = "-g", "-fno-inline", "-O0"
$warningFlags = "-Wno-unused-function", "-Wall", "-Werror"
$std = "-std=c++17"

if (-not (Get-Command $compiler)) {
    Write-Error "${compiler} not found in PATH. Exiting script."
    Exit 1
}

&$compiler -P -E ../sim86_lib.h | clang-format --style="Microsoft" | Out-File -FilePath "../shared/sim86_shared.h"
&$compiler $std $lib @($warningFlags) -install_name ${libPrefix}sim86_shared_release$libExt -o ../shared/${libPrefix}sim86_shared_release${libExt} ../sim86_lib.cpp 
&$compiler $std $lib @($warningFlags) @($debugFlags) -install_name ${libPrefix}sim86_shared_debug$libExt -o ../shared/${libPrefix}sim86_shared_debug${libExt} ../sim86_lib.cpp 
otool -L ../shared/libsim86_shared_debug.dylib
nm ../shared/libsim86_shared_debug.dylib

$languages = @("csharp", "python", "nodejs", "go", "odin", "zig")

foreach ($lang in $languages) {
    Copy-Item -Path "../shared/sim86_shared.h" -Destination "../shared/contrib_${lang}/sim86_shared.h"
    Copy-Item -Path "../shared/libsim86_shared_debug$libExt" -Destination "../shared/contrib_${lang}/libsim86_shared_debug$libExt"
}
Set-Location $previousDir
