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
    public class SchedulerUnknown3 : SchedulerBase
    {
        public override SchedulerType Type => SchedulerType.Unknown3;

        public byte Unknown10 { get; set; }
        public byte Unknown11 { get; set; }
        public ushort AttachId { get; set; }
        public int Unknown1C { get; set; }

        public override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            if (target.Version > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(target), "unsupported target");
            }
            base.Serialize(writer, target, endian);
            writer.WriteValueU8(this.Unknown10);
            writer.WriteValueU8(this.Unknown11);
            writer.WriteValueU16(this.AttachId, endian);
            writer.SkipPadding(8);
            writer.WriteValueS32(this.Unknown1C, endian);
        }

        public override void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            if (target.Version > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(target), "unsupported target");
            }
            base.Deserialize(span, ref index, target, endian);
            this.Unknown10 = span.ReadValueU8(ref index);
            this.Unknown11 = span.ReadValueU8(ref index);
            this.AttachId = span.ReadValueU16(ref index, endian);
            span.SkipPadding(ref index, 8);
            this.Unknown1C = span.ReadValueS32(ref index, endian);
        }
    }
}
