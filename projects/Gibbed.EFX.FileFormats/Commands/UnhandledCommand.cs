﻿/* Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
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
    public class UnhandledCommand : ICommand
    {
        private CommandOpcode _CommandOpcode;

        public UnhandledCommand(CommandOpcode opcode)
        {
            this._CommandOpcode = opcode;
        }

        public CommandOpcode Opcode => this._CommandOpcode;
        public int DataOffset { get; set; }
        public byte[] Data { get; set; }

        public void Serialize(IBufferWriter<byte> writer, Target target, Endian endian, out int dataOffset)
        {
            dataOffset = this.DataOffset;
            writer.Write(this.Data);
        }

        public void Deserialize(ReadOnlySpan<byte> span, int dataOffset, Target target, Endian endian)
        {
            this.DataOffset = dataOffset;
            this.Data = span.ToArray();
        }
    }
}
