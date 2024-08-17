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

namespace Gibbed.EFX.FileFormats.Schedulers
{
    public struct Unknown2Entry
    {
        public byte Type;
        public ushort TimelineStart;
        public byte[] Payload;

        private static int GetPayloadSize(Target target)
        {
            return target.Version < 11 ? 12 : 16;
        }

        public static Unknown2Entry Read(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            var payloadSize = GetPayloadSize(target);
            Unknown2Entry instance;
            instance.Type = span.ReadValueU8(ref index);
            span.SkipPadding(ref index, 1);
            instance.TimelineStart = span.ReadValueU16(ref index, endian);
            instance.Payload = span.Slice(index, payloadSize).ToArray();
            index += payloadSize;
            return instance;
        }

        public static void Write(Unknown2Entry instance, IBufferWriter<byte> writer, Target target, Endian endian)
        {
            var payloadSize = GetPayloadSize(target);
            if (instance.Payload?.Length != payloadSize)
            {
                throw new InvalidOperationException($"{nameof(Payload)} is not {payloadSize} bytes");
            }
            writer.WriteValueU8(instance.Type);
            writer.SkipPadding(1);
            writer.WriteValueU16(instance.TimelineStart, endian);
            writer.WriteBytes(instance.Payload);
        }

        public void Write(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            Write(this, writer, target, endian);
        }

        internal static void Skip(ReadOnlySpan<byte> span, ref int index, Target target, int count, int allocatedCount)
        {
            var payloadSize = GetPayloadSize(target);
            var entrySize = 1 + 1 + payloadSize;
            var totalSize = (allocatedCount - count) * entrySize;
            span.SkipPadding(ref index, totalSize);
        }

        internal static void Skip(IBufferWriter<byte> writer, Target target, int count, int allocatedCount)
        {
            var payloadSize = GetPayloadSize(target);
            var entrySize = 1 + 1 + payloadSize;
            var totalSize = (allocatedCount - count) * entrySize;
            writer.SkipPadding(totalSize);
        }
    }
}
