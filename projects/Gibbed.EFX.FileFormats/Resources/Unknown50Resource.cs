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
    public class Unknown50Resource : BaseResource
    {
        public override ResourceType Type => ResourceType.Unknown50;

        private readonly List<Unknown50ResourceEntry> _Entries;
        private readonly List<byte> _TextureIds;
        private readonly List<byte> _Resource53Ids;

        public Unknown50Resource()
        {
            this._Entries = new();
            this._TextureIds = new();
            this._Resource53Ids = new();
        }

        public int Unknown04 { get; set; }
        public List<Unknown50ResourceEntry> Entries => this._Entries;
        public List<byte> TextureIds => this._TextureIds;
        public List<byte> Resource53Ids => this._Resource53Ids;

        public override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            if (this.Entries.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(Entries)} has too many items");
            }

            if (this.TextureIds.Count > byte.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(TextureIds)} has too many items");
            }

            if (this.Resource53Ids.Count > byte.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(Resource53Ids)} has too many items");
            }

            writer.WriteValueU16((ushort)this.Entries.Count, endian);
            writer.WriteValueU8((byte)this.TextureIds.Count);
            writer.WriteValueU8((byte)this.Resource53Ids.Count);
            writer.WriteValueS32(this.Unknown04, endian);

            foreach (var entry in this.Entries)
            {
                entry.Write(writer, target, endian);
            }

            foreach (var textureId in this.TextureIds)
            {
                writer.WriteValueU8(textureId);
            }

            foreach (var resourceId in this.Resource53Ids)
            {
                writer.WriteValueU8(resourceId);
            }
        }

        public override void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            var entryCount = span.ReadValueU16(ref index, endian);
            var textureIdCount = span.ReadValueU8(ref index);
            var resource53IdCount = span.ReadValueU8(ref index);
            this.Unknown04 = span.ReadValueS32(ref index, endian);

            this._Entries.Clear();
            for (int i = 0; i < entryCount; i++)
            {
                this._Entries.Add(Unknown50ResourceEntry.Read(span, ref index, target, endian));
            }

            this._TextureIds.Clear();
            for (int i = 0; i < textureIdCount; i++)
            {
                this._TextureIds.Add(span.ReadValueU8(ref index));
            }

            this._Resource53Ids.Clear();
            for (int i = 0; i < resource53IdCount; i++)
            {
                this._Resource53Ids.Add(span.ReadValueU8(ref index));
            }
        }
    }
}
