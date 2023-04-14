using System.Runtime.InteropServices;

namespace Sim86;

public class InstructionDecoder : IDisposable
{
    public const int Version = 3;
    public const int minimalInstructionSize = 6;

    public IEnumerable<Instruction> Decode(ReadOnlyMemory<byte> buffer)
    {
        var bufferLength = buffer.Length;
        var offset = 0;
        while (offset < bufferLength)
        {
            var instruction = Decode8086Instruction(buffer.Span.Slice(offset, Math.Min(minimalInstructionSize, bufferLength - offset)));
            if (instruction.Op == OperationType.None)
            {
                break;
            }
            //Debug.Write($" ; offset: {offset} : {BitConverter.ToString(buffer.Slice(offset, Math.Min(6, bufferLength - offset)).ToArray())}");
            yield return instruction;
            offset += (int)instruction.Size;
        } 
    }
    
    public int GetVersion()
    {
        return Sim86Native.GetVersion();
    }

    public Instruction Decode8086Instruction(ReadOnlySpan<byte> source)
    {
        return Sim86Native.Decode8086Instruction((uint)source.Length, in MemoryMarshal.AsRef<byte>(source));
    }

    public string? RegisterNameFromOperand(RegisterAccess registerAccess)
    {
        return Sim86Native.RegisterNameFromOperand(registerAccess);
    }

    public string? MnemonicFromOperationType(OperationType operationType)
    {
        return Sim86Native.MnemonicFromOperationType(operationType);
    }

    public InstructionTable Get8086InstructionTable()
    {
        return Sim86Native.Get8086InstructionTable();
    }

    private void ReleaseUnmanagedResources()
    {
       Sim86Native.Dispose();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~InstructionDecoder()
    {
        ReleaseUnmanagedResources();
    }
}
