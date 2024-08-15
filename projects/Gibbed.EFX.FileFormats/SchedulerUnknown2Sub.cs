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
    public struct SchedulerUnknown2Sub
    {
        public byte Type;
        public byte[] Unknown;

        private static int GetActionSize(Target target)
        {
            return target.Version < 11 ? 16 : 20;
        }

        public static SchedulerUnknown2Sub Read(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            var actionSize = GetActionSize(target);
            SchedulerUnknown2Sub instance;
            instance.Type = span.ReadValueU8(ref index);
            instance.Unknown = span.Slice(index, actionSize - 1).ToArray();
            index += actionSize - 1;
            return instance;
        }

        public static void Write(SchedulerUnknown2Sub instance, IBufferWriter<byte> writer, Target target, Endian endian)
        {
            var actionSize = GetActionSize(target);
            if (instance.Unknown?.Length != actionSize - 1)
            {
                throw new InvalidOperationException($"{nameof(Unknown)} is not {actionSize - 1} bytes");
            }
            writer.WriteValueU8(instance.Type);
            writer.WriteBytes(instance.Unknown);
        }

        public void Write(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            Write(this, writer, target, endian);
        }

        internal static void Skip(ReadOnlySpan<byte> span, ref int index, Target target, int count, int allocatedCount)
        {
            var actionSize = GetActionSize(target);
            var extraSize = (allocatedCount - count) * actionSize;
            span.SkipPadding(ref index, extraSize);
        }

        internal static void Skip(IBufferWriter<byte> writer, Target target, int count, int allocatedCount)
        {
            var actionSize = GetActionSize(target);
            var extraSize = (allocatedCount - count) * actionSize;
            writer.SkipPadding(extraSize);
        }
    }
}
