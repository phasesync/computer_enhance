using System.Runtime.InteropServices;

namespace Sim86;

// https://learn.microsoft.com/en-us/dotnet/standard/native-interop/custom-marshalling-source-generation?source=recommendations
// https://github.com/dotnet/runtime/blob/main/docs/design/libraries/LibraryImportGenerator/UserTypeMarshallingV2.md#value-marshaller-shapes
// https://github.com/dotnet/runtime/blob/main/docs/design/libraries/LibraryImportGenerator/StructMarshalling.md

static partial class InstructionDecoder
{
    internal static partial class Native
    {

#if OSX || Linux
	const string LIB_PATH = "./sim86_shared_debug.dylib";
#elif Windows
	const string LIB_PATH = "./libsim86_shared_debug";
#endif

        //[DllImport(LIB_PATH)]
        //public static extern uint Sim86_GetVersion();
        [LibraryImport(LIB_PATH)]
	internal static partial uint Sim86_GetVersion();

        //[DllImport(LIB_PATH)]
        //public static extern void Sim86_Decode8086Instruction(uint SourceSize, [In] ref byte Source, out Instruction Dest);
        [LibraryImport(LIB_PATH)]	
	internal static partial void Sim86_Decode8086Instruction(uint SourceSize, in byte Source, out Instruction Dest);

        //[DllImport(LIB_PATH)]
        //public static extern IntPtr Sim86_RegisterNameFromOperand([In] ref RegisterAccess RegAccess);
        [LibraryImport(LIB_PATH)]	
        internal static partial IntPtr Sim86_RegisterNameFromOperand(in RegisterAccess RegAccess);
        
	//[DllImport(LIB_PATH)]
        //public static extern IntPtr Sim86_MnemonicFromOperationType(OperationType Type);
        [LibraryImport(LIB_PATH)]	
        internal static partial IntPtr Sim86_MnemonicFromOperationType(OperationType Type);
        
	//[DllImport(LIB_PATH)]
        //public static extern void Sim86_Get8086InstructionTable(out InstructionTable Dest);
	[LibraryImport(LIB_PATH)]
        internal static partial void Sim86_Get8086InstructionTable(out InstructionTable Dest);
    }
}

