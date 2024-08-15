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
using System.Text;
using Gibbed.Memory;

namespace Gibbed.EFX.FileFormats
{
    public class EffectFile
    {
        private readonly List<ICommand> _Commands;

        public EffectFile()
        {
            this._Commands = new();
        }

        public Endian Endian { get; set; }
        public Target Target { get; set; }
        public float Unknown { get; set; }
        public List<ICommand> Commands => this._Commands;

        public void Serialize(IBufferWriter<byte> writer)
        {
            var endian = this.Endian;

            string versionString = this.Target.Version switch
            {
                8 => "EFX0008",
                9 => "EFX0009",
                10 => "EFX0010",
                11 => "EFX0011",
                _ => throw new NotSupportedException("unsupported effect version"),
            };
            writer.WriteString(versionString, 8, Encoding.ASCII);

            writer.WriteValueF32(this.Unknown, endian);

            // TODO(gibbed): finish me
            throw new NotImplementedException();
        }

        public void Deserialize(ReadOnlySpan<byte> span)
        {
            var endian = this.Endian;

            int index = 0;

            var magic = span.ReadString(ref index, 8, Encoding.ASCII, true);
            byte version = magic switch
            {
                "EFX0008" => 8, // FFXII
                "EFX0009" => 9, // FFXII
                "EFX0010" => 10, // FFXII
                "EFX0011" => 11, // TO PSP, TO Reborn
                _ => throw new FormatException("unsupported effect version"),
            };

            var unknown = span.ReadValueF32(ref index, endian);

            var totalSize = span.ReadValueS32(ref index, endian);
            if (totalSize < 0 || totalSize > span.Length)
            {
                throw new FormatException();
            }

            index = totalSize;

            var dataSpan = span.Slice(16);

            var game = DetectGame(version, dataSpan, endian);

            Target target = new(game, version);

            List<ICommand> commands = new();
            ReadCommands(dataSpan, target, endian, commands);

            this.Target = target;
            this.Unknown = unknown;
            this.Commands.Clear();
            this.Commands.AddRange(commands);
        }

        private static void ReadCommands(ReadOnlySpan<byte> span, Target target, Endian endian, List<ICommand> commands)
        {
            int index = 0;
            int spanLength = span.Length;
            while (index < spanLength)
            {
                var chunkStart = index;
                var chunkSize = span.ReadValueS32(ref index, endian);
                if (chunkSize < 8 || chunkStart + chunkSize > spanLength)
                {
                    throw new FormatException();
                }

                var chunkSpan = span.Slice(chunkStart, chunkSize);
                index = chunkStart + chunkSize;

                commands.Add(ReadCommand(chunkSpan, target, endian));
            }
        }

        private static ICommand ReadCommand(ReadOnlySpan<byte> span, Target target, Endian endian)
        {
            int index = 4; // skip command size

            var version = span.ReadValueU16(ref index, endian);
            if (version != 0x100)
            {
                throw new FormatException();
            }

            var opcode = (CommandOpcode)span.ReadValueU16(ref index, endian);

            var headerSize = opcode.GetHeaderSize();
            var dataSize = span.ReadValueS32(ref index, endian);

            var dataSpan = span.Slice(index, headerSize + dataSize);

            var read = opcode.GetRead();

            ICommand command;
            if (read != null)
            {
                command = read(dataSpan, target, endian);
            }
            else
            {
                command = new Commands.UnhandledCommand(opcode)
                {
                    Data = dataSpan.ToArray(),
                };
            }
            return command;
        }

        private Game DetectGame(byte version, ReadOnlySpan<byte> span, Endian endian)
        {
            int? schedulerMetaSize = null;
            int index = 0;
            int spanLength = span.Length;
            while (index < spanLength)
            {
                var chunkStart = index;
                var chunkSize = span.ReadValueS32(ref index, endian);
                if (chunkSize < 8 || chunkStart + chunkSize > spanLength)
                {
                    throw new FormatException();
                }

                var commandMaybeVersion = span.ReadValueU16(ref index, endian);
                if (commandMaybeVersion != 0x100)
                {
                    throw new FormatException();
                }

                var commandOpcode = (CommandOpcode)span.ReadValueU16(ref index, endian);
                var commandDataSize = span.ReadValueS32(ref index, endian);

                if (commandOpcode == CommandOpcode.SchedulerMetaAdd)
                {
                    if (schedulerMetaSize != null &&
                        schedulerMetaSize != commandDataSize)
                    {
                        throw new InvalidOperationException();
                    }
                    schedulerMetaSize = commandDataSize;
                }

                index = chunkStart + chunkSize;
            }
            // Commands.SchedulerMetaAdd has a static size which varies per game/version/platform.
            // sizes are typically aligned to 16 bytes
            return schedulerMetaSize switch
            {
                160 /*152*/ => version switch
                {
                    >= 8 and <= 10 => Game.FinalFantasyXII,
                    _ => throw new NotSupportedException(),
                },
                208 /*200*/ => version switch
                {
                    11 => Game.TacticsOgrePSP,
                    _ => throw new NotSupportedException(),
                },
                320 /*312*/ => version switch
                {
                    11 => Game.TacticsOgreReborn,
                    _ => throw new NotSupportedException(),
                },
                _ => throw new NotSupportedException(),
            };
        }
    }
}
