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
using Gibbed.Memory;
using Gibbed.EFX.FileFormats.Commands;

namespace Gibbed.EFX.FileFormats
{
    internal static class CommandHelpers
    {
        public static int GetHeaderSize(this CommandOpcode opcode) => opcode switch
        {
            CommandOpcode.SchedulerAdd => 4,
            _ => 0, //throw new NotSupportedException(),
        };

        public delegate ICommand ReadCommandDelegate(ReadOnlySpan<byte> span, Target gameVersion, Endian endian);

        public static ReadCommandDelegate GetRead(this CommandOpcode opcode) => opcode switch
        {
            CommandOpcode.ResourceAdd => ResourceAddCommand.Read,
            CommandOpcode.SchedulerAdd => SchedulerAddCommand.Read,
            _ => null,
        };

    }
}
