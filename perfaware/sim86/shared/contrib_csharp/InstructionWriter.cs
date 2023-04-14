using System.Text;

namespace Sim86;

public static class InstructionWriter
{
    public static void PrintInstruction(Instruction instruction, StringBuilder writer, InstructionDecoder decoder)
    {
        var flags = instruction.Flags;
        var wideFlag = flags.HasFlag(InstructionFlag.Wide);

        var operands = new[] { instruction.Operand0, instruction.Operand1 };

        if (flags.HasFlag(InstructionFlag.Lock))
        {
            if (instruction.Op == OperationType.xchg)
            {
                (operands[0], operands[1]) = (operands[1], operands[0]);
            }
            writer.Append($"lock ");
        }

        var mnemonicsuffix = "";
        if (flags.HasFlag(InstructionFlag.Rep))
        {
            writer.Append("rep ");
            mnemonicsuffix = wideFlag ? "w" : "b";
        }
        writer.Append($"{decoder.MnemonicFromOperationType(instruction.Op)}{mnemonicsuffix} ");

        var separator = "";
        for (uint operandIndex = 0; operandIndex < operands.Length; operandIndex++)
        {
            var operand = operands[operandIndex];
            if (operand.Type == OperandType.None)
            {
                continue;
            }

            writer.Append($"{separator}");
            separator = ", ";

            switch (operand.Type)
            {
                case OperandType.None: { } break;
                case OperandType.Register:
                    {
                        var register = decoder.RegisterNameFromOperand(operands[operandIndex].Register);
                        writer.Append($"{register}");
                    }
                    break;
                case OperandType.Memory:
                    {
                        var address = operand.Address;
                        if (flags.HasFlag(InstructionFlag.Far))
                        {
                            writer.Append("far ");
                        }

                        if (address.Flags.HasFlag(EffectiveAddressFlag.ExplicitSegment))
                        {
                            writer.Append($"{address.ExplicitSegment:d}:{address.Displacement:d}");
                        }
                        else
                        {
                            if (operands[0].Type != OperandType.Register)
                            {
                                writer.Append(wideFlag ? "word " : "byte ");
                            }

                            if (flags.HasFlag(InstructionFlag.Segment))
                            {
                                var a = decoder.RegisterNameFromOperand(new RegisterAccess
                                {
                                    Index = instruction.SegmentOverride,
                                    Offset = 0,
                                    Count = 2
                                });
                                writer.Append($"{a}:");
                            }

                            writer.Append('[');
                            PrintEffectiveAddressExpression(address, writer, decoder);
                            writer.Append(']');
                        }
                    }
                    break;
                case OperandType.Immediate:
                    var immediate = operand.Immediate;
                    if (immediate.Flags.HasFlag(ImmediateFlag.RelativeJumpDisplacement))
                    {
                        writer.Append($"${immediate.Value + instruction.Size:+#;-#;+0}");
                    }
                    else
                    {
                        writer.Append($"{immediate.Value}");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static void PrintEffectiveAddressExpression(EffectiveAddressExpression address, StringBuilder writer, InstructionDecoder decoder)
    {
        var terms = new[] { address.Term0, address.Term1 };
        var separator = "";
        for (uint index = 0; index < terms.Length; index++)
        {
            var term = terms[index];
            var registerAccess = term.Register;

            if (registerAccess.Index <= 0) continue;
            
            writer.Append(separator);
            
            if (term.Scale != 1)
            {
                writer.Append($"{term.Scale}*");
            }
            
            writer.Append($"{decoder.RegisterNameFromOperand(registerAccess)}");
            
            separator = "+";
        }

        if (address.Displacement != 0)
        {
            writer.Append($"+{address.Displacement:d}");
        }
    }
}

