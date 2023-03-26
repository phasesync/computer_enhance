#!/usr/bin/env pwsh

Set-StrictMode -Version Latest

$previousDir = Get-Location

# Every command will be relative to this build folder
$directoryName = "build"
New-Item -ItemType Directory -Path $directoryName -Force
Set-Location $directoryName

$compiler = "clang"
$lib = "-shared"
$libExt = ".so"
$libPrefix = "lib"
$debugFlags = "-g", "-fno-inline", "-O0"
$warningFlags = "-Wno-unused-function", "-Wall", "-Werror"
$std = "-std=c++17"

if ($IsWindows) {
    $compiler = "clang.exe"
    $libExt = ".dll"
}
if ($IsMacOS) {
    $lib = "-dynamiclib"
    $libExt = ".dylib"
}

if (-not (Get-Command $compiler)) {
    Write-Error "${compiler} not found in PATH. Exiting script."
    Exit 1
}

# Library shared header file
&$compiler -P -E ../sim86_lib.h | clang-format --style="Microsoft" | Out-File -FilePath (Convert-Path "../shared/sim86_shared.h")
# Pre-processor file to see all the macros expanded
&$compiler $std @($warningFlags) -E ../sim86_lib.cpp -o ../shared/sim86_lib_pre.c 
# Library Release
&$compiler $std $lib @($warningFlags) -o ../shared/${libPrefix}sim86_shared_release${libExt} ../sim86_lib.cpp 
# Library Debug
&$compiler $std $lib @($warningFlags) @($debugFlags) -o ../shared/${libPrefix}sim86_shared_debug${libExt} ../sim86_lib.cpp 

# Program Release
&$compiler $std @($warningFlags) -o sim86_release ../sim86.cpp 
# Program Debug
&$compiler $std @($warningFlags) @($debugFlags) -o sim86_debug ../sim86.cpp 

# Shared library test program (NOTE: the library must be prefixed with lib otherwise it can't find it)
&$compiler $std @($debugFlags) -o shared_library_test ../shared_library_test.cpp -L../shared -lsim86_shared_debug

# Copy shared debug library to contrib_csharp 
Copy-Item -Path "../shared/sim86_shared.h" -Destination "../shared/contrib_csharp/sim86_shared.h"
Copy-Item -Path "../shared/libsim86_shared_debug$libExt" -Destination "../shared/contrib_csharp/sim86_shared_debug$libExt"
# Copy shared debug library to contrib_python
Copy-Item -Path "../shared/libsim86_shared_debug$libExt" -Destination "../shared/contrib_python/sim86_shared_debug$libExt"
Set-Location $previousDir
