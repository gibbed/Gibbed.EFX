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
    public struct Vector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static Vector4 Read(ReadOnlySpan<byte> span, ref int index, Endian endian)
        {
            Vector4 instance;
            instance.X = span.ReadValueF32(ref index, endian);
            instance.Y = span.ReadValueF32(ref index, endian);
            instance.Z = span.ReadValueF32(ref index, endian);
            instance.W = span.ReadValueF32(ref index, endian);
            return instance;
        }

        public static void Write(Vector4 instance, IBufferWriter<byte> writer, Endian endian)
        {
            writer.WriteValueF32(instance.X, endian);
            writer.WriteValueF32(instance.Y, endian);
            writer.WriteValueF32(instance.Z, endian);
            writer.WriteValueF32(instance.W, endian);
        }

        public void Write(IBufferWriter<byte> writer, Endian endian)
        {
            Write(this, writer, endian);
        }

        public override string ToString()
        {
            return $"({this.X}, {this.Y}, {this.Z}, {this.W})";
        }
    }
}
