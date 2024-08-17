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
    public class ModelResource : BaseResource
    {
        public override ResourceType Type => ResourceType.Model;

        private readonly List<Vector4> _Vertices;
        private readonly List<Vector4> _UnknownVectors;
        private readonly List<Color> _Colors;
        private readonly List<UV> _UVs;
        private readonly List<ModelUnknown> _UnknownEntries;
        private readonly List<byte> _TextureIds;

        public ModelResource()
        {
            this._Vertices = new();
            this._UnknownVectors = new();
            this._Colors = new();
            this._UVs = new();
            this._UnknownEntries = new();
            this._TextureIds = new();
        }

        public byte Flags { get; set; }
        public ushort Unknown010Count { get; set; }
        public ushort Unknown012Count { get; set; }
        public List<Vector4> Vertices => this._Vertices;
        public List<Vector4> UnknownVectors => this._UnknownVectors;
        public List<Color> Colors => this._Colors;
        public List<UV> UVs => this._UVs;
        public List<ModelUnknown> UnknownEntries => this._UnknownEntries;
        public short[] Unknown170_0 { get; set; }
        public short[] Unknown170_1 { get; set; }
        public short[] Unknown174_0 { get; set; }
        public short[] Unknown174_1 { get; set; }
        public byte[] Unknown188_0 { get; set; }
        public byte[] Unknown188_1 { get; set; }
        public List<byte> TextureIds => this._TextureIds;

        public override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            var unknown10Count = this.Unknown010Count;
            var unknown12Count = this.Unknown012Count;

            if ((this.Flags & 1) != 0)
            {
                if (this.Unknown170_0.Length != unknown10Count * 3)
                {
                    throw new InvalidOperationException();
                }

                if (this.Unknown170_1.Length != unknown12Count * 4)
                {
                    throw new InvalidOperationException();
                }

                if (this.Unknown174_0.Length != unknown10Count * 3)
                {
                    throw new InvalidOperationException();
                }

                if (this.Unknown174_1.Length != unknown12Count * 4)
                {
                    throw new InvalidOperationException();
                }
            }

            if (this.Unknown188_0.Length != unknown10Count)
            {
                throw new InvalidOperationException();
            }

            if (this.Unknown188_1.Length != unknown12Count)
            {
                throw new InvalidOperationException();
            }

            if (this.Vertices.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(Vertices)} has too many items");
            }

            if (this.UnknownVectors.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(UnknownVectors)} has too many items");
            }

            if (this.Colors.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(Colors)} has too many items");
            }

            if (this.UVs.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(UVs)} has too many items");
            }

            if (this.TextureIds.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(TextureIds)} has too many items");
            }

            if (this.UnknownEntries.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException($"{nameof(UnknownEntries)} has too many items");
            }

            writer.WriteValueU8(this.Flags);
            writer.SkipPadding(target.Version < 11 ? 1 : 3);
            writer.WriteValueU16(this.Unknown010Count, endian);
            writer.WriteValueU16(this.Unknown012Count, endian);
            writer.WriteValueU16((ushort)this._Vertices.Count, endian);
            writer.WriteValueU16((ushort)this._UnknownVectors.Count, endian);
            writer.WriteValueU16((ushort)this._Colors.Count, endian);
            writer.WriteValueU16((ushort)this._UVs.Count, endian);
            writer.WriteValueU16((ushort)this._TextureIds.Count, endian);
            writer.WriteValueU16((ushort)this._UnknownEntries.Count, endian);

            foreach (var vertex in this._Vertices)
            {
                vertex.Write(writer, endian);
            }

            foreach (var unknownVector in this._UnknownVectors)
            {
                unknownVector.Write(writer, endian);
            }

            foreach (var color in this._Colors)
            {
                color.Write(writer, endian);
            }

            foreach (var uv in this._UVs)
            {
                uv.Write(writer, endian);
            }

            foreach (var unknownEntry in this._UnknownEntries)
            {
                unknownEntry.Write(writer, target, endian);
            }

            if ((this.Flags & 1) != 0)
            {
                var unknown170_0 = this.Unknown170_0;
                for (int i = 0; i < unknown10Count * 3; i++)
                {
                    writer.WriteValueS16(unknown170_0[i], endian);
                }

                var unknown170_1 = this.Unknown170_1;
                for (int i = 0; i < unknown12Count * 4; i++)
                {
                    writer.WriteValueS16(unknown170_1[i], endian);
                }

                var unknown174_0 = this.Unknown174_0;
                for (int i = 0; i < unknown10Count * 3; i++)
                {
                    writer.WriteValueS16(unknown174_0[i], endian);
                }

                var unknown174_1 = this.Unknown174_1;
                for (int i = 0; i < unknown12Count * 4; i++)
                {
                    writer.WriteValueS16(unknown174_1[i], endian);
                }
            }

            writer.WriteBytes(this.Unknown188_0, 0, this.Unknown010Count);
            writer.WriteBytes(this.Unknown188_1, 0, this.Unknown012Count);

            foreach (var textureId in this._TextureIds)
            {
                writer.WriteValueU8(textureId);
            }
        }

        public override void Deserialize(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            this.Flags = span.ReadValueU8(ref index);
            span.SkipPadding(ref index, target.Version < 11 ? 1 : 3);

            var unknown10Count = this.Unknown010Count = span.ReadValueU16(ref index, endian);
            var unknown12Count = this.Unknown012Count = span.ReadValueU16(ref index, endian);
            var vertexCount = span.ReadValueU16(ref index, endian);
            var unknownVectorCount = span.ReadValueU16(ref index, endian);
            var colorCount = span.ReadValueU16(ref index, endian);
            var uvCount = span.ReadValueU16(ref index, endian);
            var textureCount = span.ReadValueU16(ref index, endian);
            var unknownEntryCount = span.ReadValueU16(ref index, endian);

            this._Vertices.Clear();
            for (int i = 0; i < vertexCount; i++)
            {
                this._Vertices.Add(Vector4.Read(span, ref index, endian));
            }

            this._UnknownVectors.Clear();
            for (int i = 0; i < unknownVectorCount; i++)
            {
                this._UnknownVectors.Add(Vector4.Read(span, ref index, endian));
            }

            this._Colors.Clear();
            for (int i = 0; i < colorCount; i++)
            {
                this._Colors.Add(Color.Read(span, ref index, endian));
            }

            this._UVs.Clear();
            for (int i = 0; i < uvCount; i++)
            {
                this._UVs.Add(UV.Read(span, ref index, endian));
            }

            this._UnknownEntries.Clear();
            for (int i = 0; i < unknownEntryCount; i++)
            {
                this._UnknownEntries.Add(ModelUnknown.Read(span, ref index, target, endian));
            }

            if ((this.Flags & 1) != 0)
            {
                var unknown170_0 = this.Unknown170_0 = new short[unknown10Count * 3];
                for (int i = 0; i < unknown10Count * 3; i++)
                {
                    unknown170_0[i] = span.ReadValueS16(ref index, endian);
                }

                var unknown170_1 = this.Unknown170_1 = new short[unknown12Count * 4];
                for (int i = 0; i < unknown12Count * 4; i++)
                {
                    unknown170_1[i] = span.ReadValueS16(ref index, endian);
                }

                var unknown174_0 = this.Unknown174_0 = new short[unknown10Count * 3];
                for (int i = 0; i < unknown10Count * 3; i++)
                {
                    unknown174_0[i] = span.ReadValueS16(ref index, endian);
                }

                var unknown174_1 = this.Unknown174_1 = new short[unknown12Count * 4];
                for (int i = 0; i < unknown12Count * 4; i++)
                {
                    unknown174_1[i] = span.ReadValueS16(ref index, endian);
                }
            }

            this.Unknown188_0 = span.Slice(index, unknown10Count).ToArray();
            index += unknown10Count;

            this.Unknown188_1 = span.Slice(index, unknown12Count).ToArray();
            index += unknown12Count;

            this._TextureIds.Clear();
            for (int i = 0; i < textureCount; i++)
            {
                this._TextureIds.Add(span.ReadValueU8(ref index));
            }
        }
    }
}
