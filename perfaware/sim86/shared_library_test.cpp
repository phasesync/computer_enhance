/* ========================================================================

   (C) Copyright 2023 by Molly Rocket, Inc., All Rights Reserved.
   
   This software is provided 'as-is', without any express or implied
   warranty. In no event will the authors be held liable for any damages
   arising from the use of this software.
   
   Please see https://computerenhance.com for more information
   
   ======================================================================== */

#include <stdio.h>

#include "shared/sim86_shared.h"
//#pragma comment (lib, "sim86_shared.lib")

unsigned char ExampleDisassembly[247] =
{
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
    0xDE, 0xE1, 0xDC, 0xE0, 0xDA, 0xE3, 0xD8
};

int main(void)
{
    u32 Version = Sim86_GetVersion();
    printf("Sim86 Version: %u\n", Version);
    
    instruction_table Table;
    Sim86_Get8086InstructionTable(&Table);
    printf("8086 Instruction Instruction Encoding Count: %u\n", Table.EncodingCount);
    
    u32 Offset = 0;
    while(Offset < sizeof(ExampleDisassembly))
    {
        instruction Decoded;
        Sim86_Decode8086Instruction(sizeof(ExampleDisassembly) - Offset, ExampleDisassembly + Offset, &Decoded);
        if(Decoded.Op)
        {
            Offset += Decoded.Size;
            printf("Size:%u Op:%s Flags:0x%x\n", Decoded.Size, Sim86_MnemonicFromOperationType(Decoded.Op), Decoded.Flags);
        }
        else
        {
            printf("Unrecognized instruction\n");
            break;
        }
    }
    
    return 0;
}
