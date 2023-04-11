using Sim86;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine($"USAGE: sim86 [8086 machine code file]");
            return;
        }

        var Table = InstructionDecoder.Get8086InstructionTable();
        Console.WriteLine($"; 8086 Instruction Instruction Encoding Count: {Table.MaxInstructionByteCount}");

        var stdout = new StreamWriter(Console.OpenStandardOutput());
        stdout.AutoFlush = true;
        Console.SetOut(stdout);

        InstructionWriter.PrintInstructions(args[0], stdout);
    }
}
