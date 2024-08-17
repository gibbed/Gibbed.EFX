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

namespace Gibbed.EFX.FileFormats.Resources
{
    public class Unknown51Resource : BaseResource
    {
        public override ResourceType Type => ResourceType.Unknown51;

        private readonly List<Unknown51Entry> _Entries;

        public Unknown51Resource()
        {
            this._Entries = new();
        }

        public List<Unknown51Entry> Entries => this._Entries;

        public override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            if (this.Entries.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(Entries)} has too many items");
            }

            writer.WriteValueU16((ushort)this.Entries.Count, endian);

            foreach (var entry in this.Entries)
            {
                entry.Write(writer, target, endian);
            }
        }

        public override void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            var entryCount = span.ReadValueU16(ref index, endian);

            this._Entries.Clear();
            for (int i = 0; i < entryCount; i++)
            {
                this._Entries.Add(Unknown51Entry.Read(span, ref index, target, endian));
            }
        }
    }
}
