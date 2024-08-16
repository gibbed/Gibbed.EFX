/* Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;

namespace Gibbed.EFX.FileFormats
{
    public enum CommandOpcode : ushort
    {
        Invalid = 0,

        [Obsolete] Mode = 0x400, // EFFCMD_MODE
        [Obsolete] ViewMatrix = 0x401, // [F] EFFCMD_VIEWMATRIX
        [Obsolete] TexView = 0x402, // EFFCMD_TEXVIEW
        [Obsolete] ScreenData = 0x403, // EFFCMD_SCREENDATA

        SchedulerManagerReset = 0x500, // [PRF] EFFCMD_SCHEDULERMANAGER
        AttachManagerReset = 0x510, // [PRF] EFFCMD_ATTACHMANAGER
        GeneratorManagerReset = 0x520, // [PRF] EFFCMD_GENERATORMANAGER
        ElementManagerReset = 0x530, // [PRF] EFFCMD_ELEMENTMANAGER
        
        ResourceDelete = 0x540, // [PRF] EFFCMD_RESOURCEMANAGER

        SchedulerMetaAdd = 0x600, // [PRF] EFFCMD_SCHEMETA
        SchedulerMetaDelete = 0x601, // [PRF] EFFCMD_SCHEMETADELETE

        SchedulerPageAdd = 0x610, // [PRF] EFFCMD_SCHEPAGE
        SchedulerPageDelete = 0x611, // [PRF] EFFCMD_SCHEPAGEDELETE

        SchedulerAdd = 0x620, // [PRF] EFFCMD_SCHEDULER

        AttachAdd = 0x700, // [PRF] EFFCMD_ATTACH

        GeneratorAdd = 0x800, // [PRF] EFFCMD_GENERATOR

        GeneratorMemoryReset = 0x900, // [PRF] EFFCMD_GENRAM

        ElementAdd = 0xA00, // [PRF] EFFCMD_ELEMENT

        ResourceAdd = 0xB00, // [PRF] EFFCMD_RESOURCE

        EffectCyanSystem = 0x1000, // [PRF] EFFCMD_EFFECCYANSYSYTEM

        [Obsolete] CharacterData = 0x1001, // EFFCMD_CHARADATA
        [Obsolete] First3DPosition = 0x1002, // EFFCMD_FIRST3DPOS
        [Obsolete] ObjectBoneId = 0x1003, // EFFCMD_OBJBONEID

        [Obsolete] Zeek3System = 0x2000, // [F] EFFCMD_ZEEK3SYSTEM

        [Obsolete] Matrix3System = 0x3000, // [F] EFFCMD_MAT3SYSTEM

        [Obsolete] ArrangeSystem = 0x4000, // [F] EFFCMD_ARRANGESYSTEM

        [Obsolete] KHExtension1 = 0x5000, // EFFCMD_KHEXTENSION1
    }
}
