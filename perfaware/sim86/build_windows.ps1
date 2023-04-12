# pwsh build_windows.ps1

Set-StrictMode -Version Latest

if (-not $IsWindows) {
    Write-Error "Not Windows"
    Exit 1
}

$previousDir = Get-Location

# Every command will be relative to this build folder
$directoryName = "build"
New-Item -ItemType Directory -Path $directoryName -Force
Set-Location $directoryName

$compiler = "clang.exe"
$lib = "-shared"
$libExt = ".dll"
$libPrefix = "lib"
$debugFlags = "-g", "-fno-inline", "-O0", "-Wl,/DEBUG", "-Wl,/PDB:${libPrefix}sim86_shared_debug.pdb"
$warningFlags = "-Wno-unused-function", "-Wall", "-Werror"
$std = "-std=c++17"

if (-not (Get-Command $compiler)) {
    Write-Error "${compiler} not found in PATH. Exiting script."
    Exit 1
}

# Library shared header file
&$compiler -P -E ../sim86_lib.h | clang-format --style="Microsoft" | Out-File -FilePath "../shared/sim86_shared.h"

# Pre-processor file to see all the macros expanded
&$compiler $std @($warningFlags) -E ../sim86_lib.cpp -o ../shared/sim86_lib.i

# Shared libary
&$compiler $std $lib @($warningFlags) @($debugFlags) -o ../shared/${libPrefix}sim86_shared_debug${libExt} ../sim86_lib.cpp
&$compiler $std $lib @($warningFlags) -o ../shared/${libPrefix}sim86_shared_release${libExt} ../sim86_lib.cpp
# otool -L libsim86_shared_debug.dylib

# Program
&$compiler $std @($warningFlags) -o sim86_release ../sim86.cpp 
&$compiler $std @($warningFlags) @($debugFlags) -o sim86_debug ../sim86.cpp 
&$compiler $std @($debugFlags) -o shared_library_test ../shared_library_test.cpp -L../shared -lsim86_shared_debug

$languages = @("csharp", "python", "nodejs", "go", "odin", "zig")

foreach ($lang in $languages) {
    Copy-Item -Path "../shared/sim86_shared.h" -Destination "../shared/contrib_${lang}/sim86_shared.h"
    Copy-Item -Path "../shared/${libPrefix}sim86_shared_debug$libExt" -Destination "../shared/contrib_${lang}/${libPrefix}sim86_shared_debug$libExt"
}

Set-Location $previousDir
