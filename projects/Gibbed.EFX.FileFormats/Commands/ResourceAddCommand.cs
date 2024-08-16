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

namespace Gibbed.EFX.FileFormats.Commands
{
    public class ResourceAddCommand : BaseCommand
    {
        public override CommandOpcode Opcode => CommandOpcode.ResourceAdd;
        protected override int DataOffsetDelta => 4;

        public ResourceKey Key { get; set; }
        public byte[] Data { get; set; }

        protected override bool GetDataOffset(Target target, out int dataOffset)
        {
            if (this.Key.Type != ResourceType.Texture ||
                target.Game != Game.TacticsOgreReborn)
            {
                dataOffset = default;
                return false;
            }

            // workaround for when texture has trailing junk data
            // header size + data size from the texture header, aligned to 16 bytes
            dataOffset = (0x34 + BitConverter.ToInt32(this.Data, 0x10)).Align(16);
            return true;
        }

        protected override void Serialize(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            this.Key.Write(writer, endian);
            writer.Write(this.Data);
        }

        public override void Deserialize(ReadOnlySpan<byte> span, int dataOffset, Target target, Endian endian)
        {
            int index = 0;
            this.Key = ResourceKey.Read(span, ref index, endian);
            this.Data = span.Slice(index).ToArray();
        }
    }
}
