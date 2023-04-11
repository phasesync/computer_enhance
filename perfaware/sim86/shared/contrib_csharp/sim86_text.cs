namespace Sim86;

static partial class InstructionWriter
{
    public static void PrintInstructions(string filename, StreamWriter stdout)
    {
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        int fileLength = (int)fs.Length;

        Console.WriteLine($"; Filename: {filename}");
        Console.WriteLine($"; File Length: {fileLength}");

        Console.WriteLine("bits 16\r\n");

        int possibleInstructionLength = 6;
        Span<byte> buffer = stackalloc byte[2 * possibleInstructionLength]; // multiple of possibleInstructionLength 
        var bufferLength = buffer.Length;
        var bufferIndex = 0;
        var fileIndex = 0;

        var bytesRead = fs.Read(buffer.Slice(0, Math.Min(bufferLength, fileLength)));

        var bytesLeft = fileLength;
        while (bytesLeft > 0)
        {
            var instruction = InstructionDecoder.Decode8086Instruction(buffer.Slice(bufferIndex, Math.Min(possibleInstructionLength, bytesLeft)));
            var realInstructionLength = (int)instruction.Size;

            InstructionWriter.PrintInstruction(instruction, stdout);
            //stdout.Write($" ; fileindex: {fileIndex} bufferindex: {bufferIndex} bytesread: {bytesRead} bytesleft: {bytesLeft} realinstlength: {realInstructionLength} buffer: {BitConverter.ToString(buffer.ToArray())}");
            stdout.WriteLine();

            fileIndex += realInstructionLength;
            bytesLeft = fileLength - fileIndex;

            bufferIndex += realInstructionLength;
            var bufferRemaining = bufferLength - bufferIndex;
            if (bufferRemaining < possibleInstructionLength)
            {
                buffer.Slice(bufferIndex, bufferRemaining).CopyTo(buffer); // copy remaining bytes to beginning of buffer
                bytesRead = fs.Read(buffer.Slice(bufferRemaining, Math.Min(bufferLength - bufferRemaining, bytesLeft)));
                bufferIndex = 0;
            }
            else
            {
                bytesRead = 0;
            }
        }
    }

    public static void PrintInstruction(Instruction instruction, StreamWriter dest)
    {
        var flags = instruction.Flags;
        bool W = flags.HasFlag(InstructionFlag.Wide);

        if (flags.HasFlag(InstructionFlag.Lock))
        {
            if (instruction.Op == OperationType.xchg)
            {
                var temp = instruction.Operands[0];
                instruction.Operands[0] = instruction.Operands[1];
                instruction.Operands[1] = temp;
            }
            dest.Write($"lock ");
        }

        var mnemonicsuffix = "";
        if (flags.HasFlag(InstructionFlag.Rep))
        {
            dest.Write("rep ");
            mnemonicsuffix = W ? "w" : "b";
        }
        dest.Write($"{InstructionDecoder.MnemonicFromOperationType(instruction.Op)}{mnemonicsuffix} ");

        var separator = "";
        for (uint operandIndex = 0; operandIndex < instruction.Operands.Length; operandIndex++)
        {
            var operand = instruction.Operands[operandIndex];
            if (operand.Type == OperandType.None)
            {
                continue;
            }

            dest.Write($"{separator}");
            separator = ", ";

            switch (operand.Type)
            {
                case OperandType.None: { } break;
                case OperandType.Register:
                    {
                        var register = InstructionDecoder.RegisterNameFromOperand(instruction.Operands[operandIndex].Register);
                        dest.Write($"{register}");
                    }
                    break;
                case OperandType.Memory:
                    {
                        var address = operand.Address;
                        if (flags.HasFlag(InstructionFlag.Far))
                        {
                            dest.Write("far ");
                        }

                        if (address.Flags.HasFlag(EffectiveAddressFlag.ExplicitSegment))
                        {
                            dest.Write($"{address.ExplicitSegment:d}:{address.Displacement:d}");
                        }
                        else
                        {
                            if (instruction.Operands[0].Type != OperandType.Register)
                            {
                                dest.Write(W ? "word " : "byte ");
                            }

                            if (flags.HasFlag(InstructionFlag.Segment))
                            {
                                var a = InstructionDecoder.RegisterNameFromOperand(new RegisterAccess
                                {
                                    Index = instruction.SegmentOverride,
                                    Offset = 0,
                                    Count = 2
                                });
                                dest.Write($"{a}:");
                            }

                            dest.Write("[");
                            PrintEffectiveAddressExpression(address, dest);
                            dest.Write("]");
                        }
                    }
                    break;
                case OperandType.Immediate:
                    var immediate = operand.Immediate;
                    if (immediate.Flags.HasFlag(ImmediateFlag.RelativeJumpDisplacement))
                    {
                        dest.Write($"${immediate.Value + instruction.Size:+#;-#;+0}");
                    }
                    else
                    {
                        dest.Write($"{immediate.Value}");
                    }
                    break;
            }
        }
    }

    static void PrintEffectiveAddressExpression(EffectiveAddressExpression address, StreamWriter dest)
    {
        var terms = new EffectiveAddressTerm[] { address.Term0, address.Term1 };

        var separator = "";
        for (uint Index = 0; Index < terms.Length; Index++)
        {
            var term = terms[Index];
            var reg = term.Register;

            if (reg.Index > 0)
            {
                dest.Write(separator);
                if (term.Scale != 1)
                {
                    dest.Write($"{term.Scale}*");
                }
                dest.Write($"{InstructionDecoder.RegisterNameFromOperand(reg)}");
                separator = "+";
            }
        }

        if (address.Displacement != 0)
        {
            dest.Write($"+{address.Displacement:d}");
        }
    }
}

