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
    public readonly struct ResourceKey : IEquatable<ResourceKey>
    {
        public readonly ushort Unknown;
        public readonly ResourceType Type;
        public readonly byte Id;

        public ResourceKey(ushort unknown, ResourceType type, byte id)
        {
            this.Unknown = unknown;
            this.Type = type;
            this.Id = id;
        }

        public static ResourceKey Read(ReadOnlySpan<byte> span, ref int index, Endian endian)
        {
            var unknown = span.ReadValueU16(ref index, endian);
            var type = (ResourceType)span.ReadValueU8(ref index);
            var id = span.ReadValueU8(ref index);
            return new(unknown, type, id);
        }

        public static void Write(ResourceKey instance, IBufferWriter<byte> writer, Endian endian)
        {
            writer.WriteValueU16(instance.Unknown, endian);
            writer.WriteValueU8((byte)instance.Type);
            writer.WriteValueU8(instance.Id);
        }

        public void Write(IBufferWriter<byte> writer, Endian endian)
        {
            Write(this, writer, endian);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceKey key && this.Equals(key) == true;
        }

        public bool Equals(ResourceKey other)
        {
            return
                this.Unknown == other.Unknown &&
                this.Type == other.Type &&
                this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hashCode = -752309282;
            hashCode = hashCode * -1521134295 + this.Unknown.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Type.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Id.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ResourceKey left, ResourceKey right)
        {
            return left.Equals(right) == true;
        }

        public static bool operator !=(ResourceKey left, ResourceKey right)
        {
            return left.Equals(right) == false;
        }

        public override string ToString()
        {
            return $"{this.Type}:{this.Unknown}#{this.Id}";
        }
    }
}
