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
using System.Buffers;
using Gibbed.Memory;

namespace Gibbed.EFX.FileFormats
{
    public abstract class SchedulerBase
    {
        public abstract SchedulerType Type { get; }

        public byte Id { get; set; }
        public byte Unknown2 { get; set; }
        public byte Unknown5 { get; set; }
        public int TimelineStart { get; set; }
        public int TimelineEnd { get; set; }

        public virtual void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            writer.WriteValueU8(this.Id);
            writer.WriteValueU8((byte)this.Type);
            writer.WriteValueU8(this.Unknown2);
            writer.SkipPadding(2);
            writer.WriteValueU8(this.Unknown5);
            writer.SkipPadding(2);
            writer.WriteValueS32(this.TimelineStart, endian);
            writer.WriteValueS32(this.TimelineEnd, endian);
        }

        public virtual void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            this.Id = span.ReadValueU8(ref index);
            var type = (SchedulerType)span.ReadValueU8(ref index);
            this.Unknown2 = span.ReadValueU8(ref index);
            span.SkipPadding(ref index, 2);
            this.Unknown5 = span.ReadValueU8(ref index);
            span.SkipPadding(ref index, 2);
            this.TimelineStart = span.ReadValueS32(ref index, endian);
            this.TimelineEnd = span.ReadValueS32(ref index, endian);

            if ((this.Unknown2 != 0 && this.Unknown2 != 1) ||
                (this.Unknown5 != 0 && this.Unknown5 != 1 && this.Unknown5 != 3))
            {
                throw new FormatException();
            }
        }
    }
}
