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
using System.Linq;
using Gibbed.Memory;

namespace Gibbed.EFX.FileFormats.Commands
{
    public class SchedulerAddCommand : ICommand
    {
        public CommandOpcode Opcode => CommandOpcode.SchedulerAdd;

        public ushort MetaId { get; set; }
        public byte PageId { get; set; }
        public byte SchedulerId { get; set; }
        public SchedulerBase Scheduler { get; set; }

        public static SchedulerAddCommand Read(ReadOnlySpan<byte> span, Target gameVersion, Endian endian)
        {
            int index = 0;
            var metaId = gameVersion.Version < 11
                ? span.ReadValueU8(ref index)
                : span.ReadValueU16(ref index, endian);
            var pageId = span.ReadValueU8(ref index);
            var schedulerId = span.ReadValueU8(ref index);
            var schedulerType = (SchedulerType)span.ReadValueU8(ref index);

            var paddingSize = gameVersion.Version < 11
                ? 12
                : 11;
            var padding = span.Slice(index, paddingSize);
            foreach (var b in padding)
            {
                if (b != 0)
                {
                    throw new FormatException();
                }
            }
            index += paddingSize;

            SchedulerBase scheduler = schedulerType switch
            {
                SchedulerType.Unknown0 => new SchedulerUnknown0(),
                SchedulerType.Unknown1 => new SchedulerUnknown1(),
                SchedulerType.Unknown2 => new SchedulerUnknown2(),
                SchedulerType.Unknown3 => new SchedulerUnknown3(),
                _ => throw new NotSupportedException(),
            };
            //scheduler.Id = schedulerId;
            scheduler.Deserialize(span, ref index, gameVersion, endian);
            return new()
            {
                MetaId = metaId,
                PageId = pageId,
                SchedulerId = schedulerId,
                Scheduler = scheduler,
            };
        }
    }
}
