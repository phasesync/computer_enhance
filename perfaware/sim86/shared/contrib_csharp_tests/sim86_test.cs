// place "sim86_shared_debug.dll" next to all of these three files (sim86.cs, sim86_test.cs, sim86.csproj)
// then run "dotnet run"

using Xunit;
using Sim86;

public class Sim86Tests
{
    byte[] ExampleDisassembly = {
        0x03, 0x18, 0x03, 0x5E, 0x00, 0x83, 0xC6, 0x02, 0x83, 0xC5, 0x02, 0x83, 0xC1, 0x08, 0x03, 0x5E,
        0x00, 0x03, 0x4F, 0x02, 0x02, 0x7A, 0x04, 0x03, 0x7B, 0x06, 0x01, 0x18, 0x01, 0x5E, 0x00, 0x01,
        0x5E, 0x00, 0x01, 0x4F, 0x02, 0x00, 0x7A, 0x04, 0x01, 0x7B, 0x06, 0x80, 0x07, 0x22, 0x83, 0x82,
        0xE8, 0x03, 0x1D, 0x03, 0x46, 0x00, 0x02, 0x00, 0x01, 0xD8, 0x00, 0xE0, 0x05, 0xE8, 0x03, 0x04,
        0xE2, 0x04, 0x09, 0x2B, 0x18, 0x2B, 0x5E, 0x00, 0x83, 0xEE, 0x02, 0x83, 0xED, 0x02, 0x83, 0xE9,
        0x08, 0x2B, 0x5E, 0x00, 0x2B, 0x4F, 0x02, 0x2A, 0x7A, 0x04, 0x2B, 0x7B, 0x06, 0x29, 0x18, 0x29,
        0x5E, 0x00, 0x29, 0x5E, 0x00, 0x29, 0x4F, 0x02, 0x28, 0x7A, 0x04, 0x29, 0x7B, 0x06, 0x80, 0x2F,
        0x22, 0x83, 0x29, 0x1D, 0x2B, 0x46, 0x00, 0x2A, 0x00, 0x29, 0xD8, 0x28, 0xE0, 0x2D, 0xE8, 0x03,
        0x2C, 0xE2, 0x2C, 0x09, 0x3B, 0x18, 0x3B, 0x5E, 0x00, 0x83, 0xFE, 0x02, 0x83, 0xFD, 0x02, 0x83,
        0xF9, 0x08, 0x3B, 0x5E, 0x00, 0x3B, 0x4F, 0x02, 0x3A, 0x7A, 0x04, 0x3B, 0x7B, 0x06, 0x39, 0x18,
        0x39, 0x5E, 0x00, 0x39, 0x5E, 0x00, 0x39, 0x4F, 0x02, 0x38, 0x7A, 0x04, 0x39, 0x7B, 0x06, 0x80,
        0x3F, 0x22, 0x83, 0x3E, 0xE2, 0x12, 0x1D, 0x3B, 0x46, 0x00, 0x3A, 0x00, 0x39, 0xD8, 0x38, 0xE0,
        0x3D, 0xE8, 0x03, 0x3C, 0xE2, 0x3C, 0x09, 0x75, 0x02, 0x75, 0xFC, 0x75, 0xFA, 0x75, 0xFC, 0x74,
        0xFE, 0x7C, 0xFC, 0x7E, 0xFA, 0x72, 0xF8, 0x76, 0xF6, 0x7A, 0xF4, 0x70, 0xF2, 0x78, 0xF0, 0x75,
        0xEE, 0x7D, 0xEC, 0x7F, 0xEA, 0x73, 0xE8, 0x77, 0xE6, 0x7B, 0xE4, 0x71, 0xE2, 0x79, 0xE0, 0xE2,
        0xDE, 0xE1, 0xDC, 0xE0, 0xDA, 0xE3, 0xD8,
    };

    [Fact]
    public void sim86_version()
    {
        Assert.Equal(3u, InstructionDecoder.GetVersion());
    }

    [Fact]
    public void sim86_test()
    {
        var Offset = 0;
        while (Offset < ExampleDisassembly.Length)
        {
            var Decoded = InstructionDecoder.Decode8086Instruction(ExampleDisassembly.AsSpan().Slice(Offset));
            if (Decoded.Op != OperationType.None)
            {
                Offset += (int)Decoded.Size;
                var Op = InstructionDecoder.MnemonicFromOperationType(Decoded.Op);
                Console.Write($"Size:{Decoded.Size} Op:{Op}");
                for (int i = 0; i < Decoded.Operands.Length; i++)
                {
                    var operand = Decoded.Operands[i];
                    Console.Write($" {operand.Type}");
                    switch (operand.Type)
                    {
                        case OperandType.None:
                            break;
                        case OperandType.Register:
                            break;
                        case OperandType.Memory:
                            break;
                        case OperandType.Immediate:
                            Console.Write($" {InstructionDecoder.RegisterNameFromOperand(operand.Register)}");
                            break;
                    }
                }
                Console.WriteLine($" Flags:0x{Decoded.Flags:x}");
            }
            else
            {
                Assert.Fail("unrecognized instruction");
                break;
            }
        }
    }

    [Fact]
    public void span_byte_length()
    {
        Span<byte> buffer = stackalloc byte[6];
        Assert.Equal(6, buffer.Length);
    }

    [Fact]
    public void span_byte_while_loop()
    {
        Span<byte> buffer = stackalloc byte[256];
        Console.WriteLine(buffer.Length);
        byte i = 0;
        do
        {
            buffer[i] = i;
            Console.Write($"{buffer[i]:X2} ");
            i++;
        }
        while (i != 0);
        Console.WriteLine("");
        Assert.Equal(0xFF, buffer[255]);
    }

    [Fact]
    public void span_byte_for_loop()
    {
        Span<byte> buffer = stackalloc byte[256];
        Console.WriteLine(buffer.Length);
        for (ushort i = 0; i < 256; i++)
        {
            buffer[i] = (byte)i;
            Console.Write($"{buffer[i]:X2} ");
        }
        Console.WriteLine("");
        Assert.Equal(0xFF, buffer[255]);
    }

    [Fact]
    public void effective_address_expression_size()
    {
        //Console.WriteLine(Marshal.SizeOf(typeof(EffectiveAddressTerm)));
        //Console.WriteLine(Marshal.SizeOf(typeof(uint)));
        //Console.WriteLine(Marshal.SizeOf(typeof(int)));
        ////Console.WriteLine(Marshal.SizeOf(typeof(EffectiveAddressFlag)));
        //Console.WriteLine(Marshal.OffsetOf(typeof(InstructionOperand), nameof(InstructionOperand.Type)));
        //Console.WriteLine(Marshal.OffsetOf(typeof(InstructionOperand), nameof(InstructionOperand.Address)));
        //Console.WriteLine(Marshal.OffsetOf(typeof(InstructionOperand), nameof(InstructionOperand.Register)));
        //Console.WriteLine(Marshal.OffsetOf(typeof(InstructionOperand), nameof(InstructionOperand.Immediate)));

        InstructionOperand operand = new InstructionOperand();
        operand.Type = OperandType.Memory;
        operand.Address = new EffectiveAddressExpression();
        //operand.Address.Term0

        InstructionOperand operand1 = new InstructionOperand();
        operand1.Type = OperandType.Register;
        operand1.Register = new RegisterAccess();

        InstructionOperand operand2 = new InstructionOperand();
        operand2.Type = OperandType.Register;
        operand2.Immediate = new Immediate();

    }
}
