# C# using the shared 8086 library

### Build

Build the native library:
- Windows (dll)
```shell
../../build_windows.ps1
```
- MacOS (dylib)
```shell
../../build_osx.sh
```
- Linux (so)
```shell
../../build_linux.sh
```

### Run

 See [location](../../../part1/) for the test files.

```shell
dotnet run ../../../part1/listing_0042_completionist_decode

# or

chmod +x run.sh
./run.sh
```
