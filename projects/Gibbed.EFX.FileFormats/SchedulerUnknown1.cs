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
    public class SchedulerUnknown1 : SchedulerBase
    {
        public override SchedulerType Type => SchedulerType.Unknown1;

        public ushort GeneratorId { get; set; }
        public ushort ElementId { get; set; }
        public byte Unknown14 { get; set; }

        public override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            base.Serialize(writer, target, endian);
            writer.WriteValueU16(this.GeneratorId, endian);
            writer.WriteValueU16(this.ElementId, endian);
            writer.WriteValueU8(this.Unknown14);
            writer.SkipPadding(11);
        }

        public override void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            base.Deserialize(span, ref index, target, endian);
            this.GeneratorId = span.ReadValueU16(ref index, endian);
            this.ElementId = span.ReadValueU16(ref index, endian);
            this.Unknown14 = span.ReadValueU8(ref index);
            span.SkipPadding(ref index, 11);
        }
    }
}
