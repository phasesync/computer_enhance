using System.Buffers;
using System.Diagnostics;
using System.Text;
using Sim86;

namespace sim86;

internal static class Program
{
    static void Main(string[] args)
    {
        //Trace.Listeners.Add(new ConsoleTraceListener());
        //Trace.AutoFlush = true;

        if (args.Length < 1)
        {
            Console.WriteLine($"USAGE: sim86 [8086 machine code file]");
            return;
        }

#if Linux
        Debug.WriteLine("Running on Linux!");
#elif OSX
        Debug.WriteLine("Running on macOS!");
#elif Windows
        Debug.WriteLine("Runnung on Windows!");
#else
        Debug.WriteLine("Running on Unknown!");
#endif


        using var decoder = new InstructionDecoder();
        Debug.WriteLine($"; Native decoder version: {decoder.GetVersion()}");

        var instructionTable = decoder.Get8086InstructionTable();
        Debug.WriteLine($"; 8086 Instruction Instruction Encoding Count: {instructionTable.MaxInstructionByteCount}");

        var filename = args[0];
        var output = new StringBuilder();
        output.AppendLine($"; Filename: {filename}");

        using var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var length = (int)fileStream.Length;
        using var owner = MemoryPool<byte>.Shared.Rent(length); // borrowchecker thingy?
        var input = owner.Memory[..length];
        var bytesRead = fileStream.Read(input.Span);
        if (bytesRead != length)
        {
            throw new FileLoadException();
        }
        Debug.WriteLine($"; File length: {input.Length}");
        output.AppendLine("bits 16");
        output.AppendLine();

        foreach (var instruction in decoder.Decode(input))
        {
            InstructionWriter.PrintInstruction(instruction, output, decoder);
            output.AppendLine();
        }

        Console.Write(output.ToString());
    }
}