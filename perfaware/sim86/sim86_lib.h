/* ========================================================================

   (C) Copyright 2023 by Molly Rocket, Inc., All Rights Reserved.
   
   This software is provided 'as-is', without any express or implied
   warranty. In no event will the authors be held liable for any damages
   arising from the use of this software.
   
   Please see https://computerenhance.com for more information
   
   ======================================================================== */

#pragma once

#include "sim86.h"
#include "sim86_instruction.h"
#include "sim86_instruction_table.h"

#if defined(__GNUC__)
    #define SIM86_API
#else
    #define SIM86_API __declspec(dllimport)
#endif

extern "C" {
    SIM86_API u32 Sim86_GetVersion(void);
   	SIM86_API void Sim86_Decode8086Instruction(u32 SourceSize, u8 *Source, instruction *Dest);
    SIM86_API char const *Sim86_RegisterNameFromOperand(register_access *RegAccess);
    SIM86_API char const *Sim86_MnemonicFromOperationType(operation_type Type);
    SIM86_API void Sim86_Get8086InstructionTable(instruction_table *Dest);
}
