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

namespace Gibbed.EFX.FileFormats.Resources
{
    public struct Unknown51Entry
    {
        public byte[] Unknown00;
        public byte Unknown08;
        public byte Unknown09;
        public byte[] Unknown0A;

        public static Unknown51Entry Read(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            Unknown51Entry instance;
            instance.Unknown00 = span.Slice(index, 8).ToArray();
            index += 8;
            instance.Unknown08 = span.ReadValueU8(ref index);
            instance.Unknown09 = span.ReadValueU8(ref index);
            instance.Unknown0A = span.Slice(index, 6).ToArray();
            index += 6;
            return instance;
        }

        public static void Write(Unknown51Entry instance, IBufferWriter<byte> writer, Target target, Endian endian)
        {
            writer.WriteBytes(instance.Unknown00, 0, 8);
            writer.WriteValueU8(instance.Unknown08);
            writer.WriteValueU8(instance.Unknown09);
            writer.WriteBytes(instance.Unknown0A, 0, 6);
        }

        public void Write(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            Write(this, writer, target, endian);
        }
    }
}
