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
using System.Collections.Generic;
using Gibbed.Memory;

namespace Gibbed.EFX.FileFormats
{
    public class SchedulerUnknown2 : SchedulerBase
    {
        public override SchedulerType Type => SchedulerType.Unknown2;

        private readonly List<SchedulerUnknown2Sub> _Entries;

        public SchedulerUnknown2()
        {
            this._Entries = new();
        }

        public byte EntryAllocatedCount { get; set; }
        public List<SchedulerUnknown2Sub> Entries => this._Entries;

        public override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            if (this.Entries.Count > byte.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(Entries)} has too many items");
            }
            if (this.Entries.Count > this.EntryAllocatedCount)
            {
                throw new InvalidOperationException($"{nameof(Entries)}.{nameof(Entries.Count)} is greater than {nameof(EntryAllocatedCount)} ({this.Entries.Count} > {this.EntryAllocatedCount})");
            }
            base.Serialize(writer, target, endian);
            writer.WriteValueU8((byte)this.Entries.Count);
            writer.WriteValueU8(this.EntryAllocatedCount);
            writer.SkipPadding(2);
            foreach (var entry in this.Entries)
            {
                entry.Write(writer, target, endian);
            }
            SchedulerUnknown2Sub.Skip(writer, target, this.Entries.Count, this.EntryAllocatedCount);
        }

        public override void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            base.Deserialize(span, ref index, target, endian);
            var entryCount = span.ReadValueU8(ref index);
            var entryAllocatedCount = span.ReadValueU8(ref index);
            span.SkipPadding(ref index, 2);

            if (entryCount > entryAllocatedCount)
            {
                throw new FormatException($"{nameof(entryCount)} is greater than {nameof(entryAllocatedCount)} ({entryCount} > {entryAllocatedCount})");
            }

            this.EntryAllocatedCount = entryAllocatedCount;
            this._Entries.Clear();
            for (int i = 0; i < entryCount; i++)
            {
                this._Entries.Add(SchedulerUnknown2Sub.Read(span, ref index, target, endian));
            }
            SchedulerUnknown2Sub.Skip(span, ref index, target, entryCount, entryAllocatedCount);
        }
    }
}
