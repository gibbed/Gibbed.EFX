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
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public Color(uint rgba)
        {
            this.R = (byte)(rgba >> 0 & 0xFF);
            this.G = (byte)(rgba >> 8 & 0xFF);
            this.B = (byte)(rgba >> 16 & 0xFF);
            this.A = (byte)(rgba >> 24 & 0xFF);
        }

        public uint AsRGBA
        {
            get
            {
                // TODO(gibbed): less dumb
                uint value = 0;
                value |= this.A;
                value <<= 8;
                value |= this.B;
                value <<= 8;
                value |= this.G;
                value <<= 8;
                value |= this.R;
                return value;
            }
        }

        public static Color Read(ReadOnlySpan<byte> span, ref int index, Endian endian)
        {
            Color instance;
            instance.R = span.ReadValueU8(ref index);
            instance.G = span.ReadValueU8(ref index);
            instance.B = span.ReadValueU8(ref index);
            instance.A = span.ReadValueU8(ref index);
            return instance;
        }

        public static void Write(Color instance, IBufferWriter<byte> writer, Endian endian)
        {
            writer.WriteValueU8(instance.R);
            writer.WriteValueU8(instance.G);
            writer.WriteValueU8(instance.B);
            writer.WriteValueU8(instance.A);
        }

        public void Write(IBufferWriter<byte> writer, Endian endian)
        {
            Write(this, writer, endian);
        }

        public override string ToString()
        {
            return $"#{this.AsRGBA:X8})";
        }
    }
}
