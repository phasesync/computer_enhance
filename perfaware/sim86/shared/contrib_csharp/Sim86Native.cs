using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sim86;

// https://learn.microsoft.com/en-us/dotnet/framework/interop/marshalling-data-with-platform-invoke?redirectedfrom=MSDN
// https://www.mono-project.com/docs/advanced/pinvoke/
// http://www.swig.org/

internal static partial class Sim86Native
{

#if OSX
	const string LIB_PATH = "./libsim86_shared_debug.dylib";
#elif Linux
	const string LIB_PATH = "./libsim86_shared_debug.so";
#elif Windows
	const string LIB_PATH = "./sim86_shared_debug.dll";
#endif

	private static nint _library = nint.Zero;

	static Sim86Native()
	{
		_library = LoadLibrary.OpenLibrary(LIB_PATH);
	}

	private delegate uint _getVersion();

	private delegate void _getDecode8086Instruction(uint SourceSize, in byte Source, out Instruction Instruction);

	private delegate void _get8086InstructionTable(out InstructionTable InstructionTable);

	private delegate nint _getRegisterNameFromOperand(in RegisterAccess RegisterAccess);

	private delegate nint _getMnemonicFromOperationType(OperationType Type);

	//[DllImport(LIB_PATH)]
	//public static extern uint Sim86_GetVersion();
	//[LibraryImport(LIB_PATH, EntryPoint = "Sim86_GetVersion")]
	//internal static partial uint GetVersion();
	internal static int GetVersion()
	{
		var func = LoadLibrary.GetDelegate<_getVersion>(_library, "Sim86_GetVersion");
		return (int)func.Invoke();
	}

	//[DllImport(LIB_PATH)]
	//public static extern void Sim86_Decode8086Instruction(uint SourceSize, [In] ref byte Source, out Instruction Dest);
	// [LibraryImport(LIB_PATH)]
	// internal static partial void Sim86_Decode8086Instruction(uint SourceSize, in byte Source, out Instruction Dest);
	internal static Instruction Decode8086Instruction(uint SourceSize, in byte Source)
	{
		var func = LoadLibrary.GetDelegate<_getDecode8086Instruction>(_library, "Sim86_Decode8086Instruction");
		func.Invoke(SourceSize, in Source, out var Instruction);
		return Instruction;
	}

	//[DllImport(LIB_PATH)]
	//public static extern IntPtr Sim86_RegisterNameFromOperand([In] ref RegisterAccess RegAccess);
	// [LibraryImport(LIB_PATH)]
	// internal static partial IntPtr Sim86_RegisterNameFromOperand(in RegisterAccess RegAccess);
	internal static string? RegisterNameFromOperand(in RegisterAccess RegAccess)
	{
		var func = LoadLibrary.GetDelegate<_getRegisterNameFromOperand>(_library, "Sim86_RegisterNameFromOperand");
		var ptr2Char = func.Invoke(in RegAccess);
		var registername = Marshal.PtrToStringAnsi(ptr2Char);
		return registername;
	}

	//[DllImport(LIB_PATH)]
	//public static extern IntPtr Sim86_MnemonicFromOperationType(OperationType Type);
	// [LibraryImport(LIB_PATH)]
	// internal static partial IntPtr Sim86_MnemonicFromOperationType(in OperationType Type);
	internal static string? MnemonicFromOperationType(OperationType OperationType)
	{
		var func = LoadLibrary.GetDelegate<_getMnemonicFromOperationType>(_library, "Sim86_MnemonicFromOperationType");
		var ptr2Char = func.Invoke(OperationType);
		var mnemonic = Marshal.PtrToStringAnsi(ptr2Char);
		return mnemonic;
	}

	//[DllImport(LIB_PATH)]
	//public static extern void Sim86_Get8086InstructionTable(out InstructionTable Dest);
	// [LibraryImport(LIB_PATH)]
	// internal static partial void Sim86_Get8086InstructionTable(out InstructionTable Dest);
	internal static InstructionTable Get8086InstructionTable()
	{
		var func = LoadLibrary.GetDelegate<_get8086InstructionTable>(_library, "Sim86_Get8086InstructionTable");
		func.Invoke(out InstructionTable InstructionTable);
		return InstructionTable;
	}

	public static void Dispose()
	{
		Debug.WriteLine("Sim86Native Disposed");
		if (_library == nint.Zero) return;
		LoadLibrary.CloseLibrary(_library);
		_library = nint.Zero;
	}
}