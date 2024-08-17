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
    public struct Unknown50ResourceEntry
    {
        public Vector4 Unknown00;
        public Vector4 Unknown10;
        public Vector4 Unknown20;
        public Vector4 Unknown30;
        public Vector4 Unknown40;
        public Vector4 Unknown50;
        public Color Unknown60;
        public byte[] Unknown64;
        public byte[] Unknown70;

        public static Unknown50ResourceEntry Read(ReadOnlySpan<byte> span, ref int index, Target target, Endian endian)
        {
            Unknown50ResourceEntry instance;
            instance.Unknown00 = Vector4.Read(span, ref index, endian);
            instance.Unknown10 = Vector4.Read(span, ref index, endian);
            instance.Unknown20 = Vector4.Read(span, ref index, endian);
            instance.Unknown30 = Vector4.Read(span, ref index, endian);
            instance.Unknown40 = Vector4.Read(span, ref index, endian);
            instance.Unknown50 = Vector4.Read(span, ref index, endian);
            instance.Unknown60 = Color.Read(span, ref index, endian);
            instance.Unknown64 = span.Slice(index, 12).ToArray();
            index += 12;
            if (target.Game == Game.TacticsOgreReborn)
            {
                instance.Unknown70 = span.Slice(index, 16).ToArray();
                index += 16;
            }
            else
            {
                instance.Unknown70 = null;
            }
            return instance;
        }

        public static void Write(Unknown50ResourceEntry instance, IBufferWriter<byte> writer, Target target, Endian endian)
        {
            instance.Unknown00.Write(writer, endian);
            instance.Unknown10.Write(writer, endian);
            instance.Unknown20.Write(writer, endian);
            instance.Unknown30.Write(writer, endian);
            instance.Unknown40.Write(writer, endian);
            instance.Unknown50.Write(writer, endian);
            instance.Unknown60.Write(writer, endian);
            writer.WriteBytes(instance.Unknown64, 0, 12);
            if (target.Game == Game.TacticsOgreReborn)
            {
                writer.WriteBytes(instance.Unknown70, 0, 16);
            }
        }

        public void Write(IBufferWriter<byte> writer, Target target, Endian endian)
        {
            Write(this, writer, target, endian);
        }
    }
}
