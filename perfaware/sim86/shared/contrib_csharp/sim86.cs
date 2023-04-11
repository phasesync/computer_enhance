using System.Runtime.InteropServices;

namespace Sim86;

public static partial class InstructionDecoder
{
    public const int Version = 3;

    public static uint GetVersion()
    {
        return Native.Sim86_GetVersion();
    }

    public static Instruction Decode8086Instruction(Span<byte> Source)
    {
        Instruction instruction;
        Native.Sim86_Decode8086Instruction((uint)Source.Length, ref MemoryMarshal.AsRef<byte>(Source), out instruction);
        return instruction;
    }

    public static string RegisterNameFromOperand(RegisterAccess RegAccess)
    {
        return Marshal.PtrToStringAnsi(Native.Sim86_RegisterNameFromOperand(ref RegAccess))!;
    }

    public static string MnemonicFromOperationType(OperationType Type)
    {
        return Marshal.PtrToStringAnsi(Native.Sim86_MnemonicFromOperationType(Type))!;
    }

    public static InstructionTable Get8086InstructionTable()
    {
        InstructionTable NativeTable;
        Native.Sim86_Get8086InstructionTable(out NativeTable);
        return NativeTable;
    }
}
