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
using Gibbed.EFX.FileFormats.Schedulers;
using Gibbed.Memory;

namespace Gibbed.EFX.FileFormats.Commands
{
    public class SchedulerAddCommand : BaseCommand
    {
        public override CommandOpcode Opcode => CommandOpcode.SchedulerAdd;
        protected override int DataOffsetDelta => 4;

        public ushort MetaId { get; set; }
        public byte PageId { get; set; }
        public byte SchedulerId { get; set; }
        public BaseScheduler Scheduler { get; set; }
        public byte[] Padding { get; set; }

        private static int GetPaddingSize(Target target)
        {
            return target.Version < 11 ? 12 : 11;
        }

        protected override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            if (this.Scheduler == null)
            {
                throw new InvalidOperationException($"{nameof(Scheduler)} is null");
            }

            if (target.Version < 11)
            {
                if (this.MetaId > byte.MaxValue)
                {
                    throw new InvalidOperationException($"{nameof(MetaId)} is too big");
                }
                writer.WriteValueU8((byte)this.MetaId);
            }
            else
            {
                writer.WriteValueU16(this.MetaId, endian);
            }

            writer.WriteValueU8(this.PageId);
            writer.WriteValueU8(this.SchedulerId);
            writer.WriteValueU8((byte)this.Scheduler.Type);
            writer.SkipPadding(GetPaddingSize(target));
            this.Scheduler.Serialize(writer, target, endian);
            if (this.Padding != null)
            {
                writer.Write(this.Padding);
            }
        }

        public override void Deserialize(ReadOnlySpan<byte> span, int dataOffset, Target target, Endian endian)
        {
            int index = 0;
            var paddingSize = GetPaddingSize(target);
            this.MetaId = target.Version < 11
                ? span.ReadValueU8(ref index)
                : span.ReadValueU16(ref index, endian);
            this.PageId = span.ReadValueU8(ref index);
            var schedulerId = this.SchedulerId = span.ReadValueU8(ref index);
            var schedulerType = (SchedulerType)span.ReadValueU8(ref index);
            span.SkipPadding(ref index, paddingSize);
            BaseScheduler scheduler = this.Scheduler = schedulerType switch
            {
                SchedulerType.Unknown0 => new Unknown0Scheduler(),
                SchedulerType.Unknown1 => new Unknown1Scheduler(),
                SchedulerType.Unknown2 => new Unknown2Scheduler(),
                SchedulerType.Unknown3 => new Unknown3Scheduler(),
                _ => throw new NotSupportedException(),
            };
            scheduler.Id = schedulerId;
            scheduler.Deserialize(span, ref index, target, endian);
            this.Padding = index < span.Length ? span.Slice(index).ToArray() : null;
        }
    }
}
