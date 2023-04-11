using System.Runtime.InteropServices;

namespace Sim86;

static partial class InstructionDecoder
{
    internal static class Native
    {
        const string lib = "sim86_shared_debug";

        [DllImport(lib)]
        public static extern uint Sim86_GetVersion();

        [DllImport(lib)]
        public static extern void Sim86_Decode8086Instruction(uint SourceSize, [In] ref byte Source, out Instruction Dest);

        [DllImport(lib)]
        public static extern IntPtr Sim86_RegisterNameFromOperand([In] ref RegisterAccess RegAccess);

        [DllImport(lib)]
        public static extern IntPtr Sim86_MnemonicFromOperationType(OperationType Type);

        [DllImport(lib)]
        public static extern void Sim86_Get8086InstructionTable(out InstructionTable Dest);
    }
}

