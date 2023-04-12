using System.Runtime.InteropServices;

namespace Sim86;

public enum OperationType : uint
{
    None,
    mov,
    push,
    pop,
    xchg,
    @in,
    @out,
    xlat,
    lea,
    lds,
    les,
    lahf,
    sahf,
    pushf,
    popf,
    add,
    adc,
    inc,
    aaa,
    daa,
    sub,
    sbb,
    dec,
    neg,
    cmp,
    aas,
    das,
    mul,
    imul,
    aam,
    div,
    idiv,
    aad,
    cbw,
    cwd,
    not,
    shl,
    shr,
    sar,
    rol,
    ror,
    rcl,
    rcr,
    and,
    test,
    or,
    xor,
    rep,
    movs,
    cmps,
    scas,
    lods,
    stos,
    call,
    jmp,
    ret,
    retf,
    je,
    jl,
    jle,
    jb,
    jbe,
    jp,
    jo,
    js,
    jne,
    jnl,
    jg,
    jnb,
    ja,
    jnp,
    jno,
    jns,
    loop,
    loopz,
    loopnz,
    jcxz,
    @int,
    int3,
    into,
    iret,
    clc,
    cmc,
    stc,
    cld,
    std,
    cli,
    sti,
    hlt,
    wait,
    esc,
    @lock,
    segment,
}

[StructLayout(LayoutKind.Sequential)]
public struct Instruction
{
    public uint Address;
    public uint Size;
    public OperationType Op;
    public InstructionFlag Flags;
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public InstructionOperand Operand0;
    public InstructionOperand Operand1;
    public uint SegmentOverride;
};

[Flags]
public enum InstructionFlag : uint
{
    Lock = 0x1,
    Rep = 0x2,
    Segment = 0x4,
    Wide = 0x8,
    Far = 0x10,
};

[StructLayout(LayoutKind.Sequential)]
public struct RegisterAccess
{
    public uint Index;
    public uint Offset;
    public uint Count;
}

[StructLayout(LayoutKind.Explicit)]
public struct InstructionOperand
{
    [FieldOffset(0)]
    public OperandType Type;

    [FieldOffset(4)]
    public EffectiveAddressExpression Address;

    [FieldOffset(4)]
    public RegisterAccess Register;

    [FieldOffset(4)]
    public Immediate Immediate;
};

[StructLayout(LayoutKind.Sequential)]
public struct Immediate
{
    public int Value;
    public ImmediateFlag Flags;
};

[Flags]
public enum ImmediateFlag : uint
{
    RelativeJumpDisplacement = 0x1,
};

public enum OperandType : uint
{
    None,
    Register,
    Memory,
    Immediate,
};
[StructLayout(LayoutKind.Sequential)]
public struct InstructionEncoding
{
    public OperationType Op;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public InstructionBits[] Bits;
};

public enum InstructionBitsUsage : byte
{
    End,
    Literal,
    D,
    S,
    W,
    V,
    Z,
    MOD,
    REG,
    RM,
    SR,
    Disp,
    Data,
    DispAlwaysW,
    WMakesDataW,
    RMRegAlwaysW,
    RelJMPDisp,
    Far,
    Count,
};

[StructLayout(LayoutKind.Sequential)]
public struct InstructionBits
{
    public InstructionBitsUsage Usage;
    public byte BitCount;
    public byte Shift;
    public byte Value;
};

[StructLayout(LayoutKind.Sequential)]
public struct EffectiveAddressExpression
{
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //public EffectiveAddressTerm[] Terms; // TODO Could not load type 'Sim86.InstructionOperand' from assembly 'sim86, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' because it contains an object field at offset 4 that is incorrectly aligned or overlapped by a non-object field.
    public EffectiveAddressTerm Term0;
    public EffectiveAddressTerm Term1;
    public uint ExplicitSegment;
    public int Displacement;
    public EffectiveAddressFlag Flags;
};

[StructLayout(LayoutKind.Sequential)]
public struct EffectiveAddressTerm
{
    public RegisterAccess Register;
    public int Scale;
};

[Flags]
public enum EffectiveAddressFlag : uint
{
    ExplicitSegment = 0x1,
};

[StructLayout(LayoutKind.Sequential)]
public struct InstructionTable
{
    public IntPtr Encodings;
    public int EncodingCount;
    public uint MaxInstructionByteCount;
}

