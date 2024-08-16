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
using System.Collections.Generic;
using System.IO;
using Gibbed.Buffers;
using Gibbed.EFX.FileFormats;
using Gibbed.EFX.FileFormats.Commands;
using Gibbed.EFX.FileFormats.Schedulers;
using Gibbed.Memory;
using NDesk.Options;

namespace Gibbed.EFX.Import
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool verbose = false;
            bool showHelp = false;

            OptionSet options = new()
            {
                { "v|verbose", "be verbose (list files)", v => verbose = v != null },
                { "h|help", "show this message and exit", v => showHelp = v != null },
            };

            List<string> extras;
            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_toml|input_dir [output_path]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var inputPath = Path.GetFullPath(extras[0]);
            string inputBasePath;
            if (Directory.Exists(inputPath) == true)
            {
                inputBasePath = inputPath;
                inputPath = Path.Combine(inputPath, "@efx.toml");
            }
            else
            {
                inputBasePath = Path.GetDirectoryName(inputPath);
            }

            string outputPath;
            if (extras.Count > 1)
            {
                outputPath = extras[1];
            }
            else
            {
                outputPath = Path.GetDirectoryName(inputPath) + ".efx";
            }

            Tommy.TomlTable rootTable;
            var inputBytes = File.ReadAllBytes(inputPath);
            using (var input = new MemoryStream(inputBytes, false))
            using (var reader = new StreamReader(input, true))
            {
                rootTable = Tommy.TOML.Parse(reader);
            }

            if (Import(rootTable, inputBasePath,out var effect) == false)
            {
                return;
            }

            PooledArrayBufferWriter<byte> writer = new();
            effect.Serialize(writer);
            var writtenSpan = writer.WrittenSpan;
            File.WriteAllBytes(outputPath, writtenSpan.ToArray());
            writer.Clear();
        }

        private static bool Import(Tommy.TomlTable table, string inputBasePath, out EffectFile effect)
        {
            effect = new();

            if (ImportEnum(table, "endian", Endian.Little, out var endian) == false)
            {
                Console.WriteLine("Invalid endian '{0}' specified.", endian);
                return false;
            }
            effect.Endian = endian;

            var game = Game.TacticsOgreReborn;
            if (ImportEnum(table, "game", out game) == false)
            {
                Console.WriteLine("Invalid game '{0}' specified.", game);
                return false;
            }
            var version = table["version"].AsInteger.Value;

            effect.Target = new(game, (byte)version);
            effect.Unknown = ImportFloat(table["unknown"]);

            foreach (Tommy.TomlTable commandTable in table["commands"])
            {
                if (ImportCommand(commandTable, inputBasePath, out var command) == false)
                {
                    return false;
                }
                effect.Commands.Add(command);
            }

            return true;
        }

        private static float ImportFloat(Tommy.TomlNode node)
        {
            var asInteger = node.AsInteger;
            if (asInteger != null)
            {
                return (float)asInteger.Value;
            }
            var asFloat = node.AsFloat;
            if (asFloat != null)
            {
                return (float)asFloat.Value;
            }
            throw new InvalidOperationException();
        }

        private static bool ImportCommand(Tommy.TomlTable table, string inputBasePath, out ICommand command)
        {
            var opcode = CommandOpcode.Invalid;
            if (ImportEnum(table, "command", out opcode) == false || opcode == CommandOpcode.Invalid)
            {
                Console.WriteLine("Invalid command opcode '{0}' specified.", opcode);
                command = default;
                return false;
            }

            command = CommandFactory.Create(opcode);
            if (command is ResourceAddCommand resourceAddCommand)
            {
                return ImportCommand(table, inputBasePath, resourceAddCommand);
            }
            else if (command is SchedulerAddCommand schedulerAddCommand)
            {
                return ImportCommand(table, schedulerAddCommand);
            }
            else if (command is UnhandledCommand unhandledCommand)
            {
                return ImportCommand(table, inputBasePath, unhandledCommand);
            }

            throw new NotSupportedException($"unhandled command type {command.GetType()}");
        }

        private static bool ImportCommand(Tommy.TomlTable table, string inputBasePath, ResourceAddCommand command)
        {
            if (ImportResourceKey(table["key"].AsTable, out var key) == false)
            {
                return false;
            }
            var dataPath = table["data_path"].AsString.Value;
            dataPath = Path.Combine(inputBasePath, dataPath);
            command.Key = key;
            command.Data = File.ReadAllBytes(dataPath);
            return true;
        }

        private static bool ImportCommand(Tommy.TomlTable table, SchedulerAddCommand command)
        {
            command.MetaId = (ushort)table["meta_id"].AsInteger.Value;
            command.PageId = (byte)table["page_id"].AsInteger.Value;
            command.SchedulerId = (byte)table["scheduler_id"].AsInteger.Value;
            if (ImportScheduler(table["scheduler"].AsTable, out var scheduler) == false)
            {
                return false;
            }
            if (ImportBytes(table["padding"], out var paddingBytes) == true)
            {
                command.Padding = paddingBytes;
            }
            scheduler.Id = command.SchedulerId;
            command.Scheduler = scheduler;
            return true;
        }

        private static bool ImportCommand(Tommy.TomlTable table, string inputBasePath, UnhandledCommand command)
        {
            var dataOffset = table["data_offset"].AsInteger.Value;
            var dataPath = table["data_path"].AsString.Value;
            dataPath = Path.Combine(inputBasePath, dataPath);
            command.DataOffset = (int)dataOffset;
            command.Data = File.ReadAllBytes(dataPath);
            return true;
        }

        private static bool ImportScheduler(Tommy.TomlTable table, out BaseScheduler scheduler)
        {
            var type = SchedulerType.Invalid;
            if (ImportEnum(table, "type", out type) == false || type == SchedulerType.Invalid)
            {
                Console.WriteLine("Invalid scheduler type '{0}' specified.", type);
                scheduler = default;
                return false;
            }

            scheduler = SchedulerFactory.Create(type);
            if (scheduler is Unknown0Scheduler unknown0Scheduler)
            {
                return ImportScheduler(table, unknown0Scheduler);
            }
            else if (scheduler is Unknown1Scheduler unknown1Scheduler)
            {
                return ImportScheduler(table, unknown1Scheduler);
            }
            else if (scheduler is Unknown2Scheduler unknown2Scheduler)
            {
                return ImportScheduler(table, unknown2Scheduler);
            }
            else if (scheduler is Unknown3Scheduler unknown3Scheduler)
            {
                return ImportScheduler(table, unknown3Scheduler);
            }

            throw new NotSupportedException($"unhandled scheduler type {scheduler.GetType()}");
        }

        private static bool ImportScheduler(Tommy.TomlTable table, BaseScheduler scheduler)
        {
            scheduler.Unknown2 = (byte)table["u2"].AsInteger.Value;
            scheduler.Unknown5 = (byte)table["u5"].AsInteger.Value;
            scheduler.TimelineStart = (int)table["timeline_start"].AsInteger.Value;
            scheduler.TimelineEnd = (int)table["timeline_end"].AsInteger.Value;
            return true;
        }

        private static bool ImportScheduler(Tommy.TomlTable table, Unknown0Scheduler scheduler)
        {
            if (ImportScheduler(table, (BaseScheduler)scheduler) == false)
            {
                return false;
            }
            return true;
        }

        private static bool ImportScheduler(Tommy.TomlTable table, Unknown1Scheduler scheduler)
        {
            if (ImportScheduler(table, (BaseScheduler)scheduler) == false)
            {
                return false;
            }
            scheduler.GeneratorId = (ushort)table["generator_id"].AsInteger.Value;
            scheduler.ElementId = (ushort)table["element_id"].AsInteger.Value;
            scheduler.Unknown14 = (byte)table["u14"].AsInteger.Value;
            return true;
        }

        private static bool ImportScheduler(Tommy.TomlTable table, Unknown2Scheduler scheduler)
        {
            if (ImportScheduler(table, (BaseScheduler)scheduler) == false)
            {
                return false;
            }

            foreach (Tommy.TomlTable actionTable in table["actions"])
            {
                if (ImportUnknown2SchedulerEntry(actionTable, out var entry) == false)
                {
                    return false;
                }
                scheduler.Entries.Add(entry);
            }

            var entryAllocatedCountNode = table["entry_allocated_count"].AsInteger;
            scheduler.EntryAllocatedCount = entryAllocatedCountNode != null
                ? (byte)entryAllocatedCountNode.Value
                : (byte)scheduler.Entries.Count;

            return true;
        }

        private static bool ImportUnknown2SchedulerEntry(Tommy.TomlTable table, out Unknown2Sub entry)
        {
            entry = default;
            entry.Type = (byte)table["type"].AsInteger.Value;
            if (ImportBytes(table["u1"], out var unknown) == false)
            {
                Console.WriteLine($"Invalid byte table for {nameof(Unknown2Sub)}.{nameof(Unknown2Sub.Unknown)}.");
                return false;
            }
            entry.Unknown = unknown;
            return true;
        }

        private static bool ImportScheduler(Tommy.TomlTable table, Unknown3Scheduler scheduler)
        {
            if (ImportScheduler(table, (BaseScheduler)scheduler) == false)
            {
                return false;
            }
            scheduler.Unknown10 = (byte)table["u10"].AsInteger.Value;
            scheduler.Unknown11 = (byte)table["u11"].AsInteger.Value;
            scheduler.AttachId = (ushort)table["attach_id"].AsInteger.Value;
            scheduler.Unknown1C = (int)table["u1C"].AsInteger.Value;
            return true;
        }

        private static bool ImportResourceKey(Tommy.TomlTable table, out ResourceKey key)
        {
            var unknown = (ushort)table["unknown"].AsInteger.Value;
            var type = ResourceType.Invalid;
            if (ImportEnum(table, "type", out type) == false || type == ResourceType.Invalid)
            {
                Console.WriteLine("Invalid resource type '{0}' specified.", type);
                key = default;
                return false;
            }
            var id = (byte)table["id"].AsInteger.Value;
            key = new(unknown, type, id);
            return true;
        }

        private static bool ImportBytes(Tommy.TomlNode node, out byte[] bytes)
        {
            if (node is not Tommy.TomlArray array)
            {
                bytes = default;
                return false;
            }
            List<byte> list = new();
            foreach (var child in array.Children)
            {
                list.Add((byte)child.AsInteger.Value);
            }
            bytes = list.ToArray();
            return true;
        }

        private static bool ImportEnum<TEnum>(Tommy.TomlTable table, string key, out TEnum value)
             where TEnum : struct
        {
            if (table[key] is not Tommy.TomlString str)
            {
                Console.WriteLine($"No {nameof(TEnum)} specified for '{key}'.");
                value = default;
                return false;
            }
            if (Enum.TryParse(str, out value) == false)
            {
                Console.WriteLine($"Invalid {nameof(TEnum)} value '{str.Value}' specified for '{key}'.");
                value = default;
                return false;
            }
            return true;
        }

        private static bool ImportEnum<TEnum>(Tommy.TomlTable table, string key, TEnum defaultValue, out TEnum value)
             where TEnum : struct
        {
            if (table[key] is not Tommy.TomlString str)
            {
                value = defaultValue;
                return true;
            }
            if (Enum.TryParse(str, out value) == false)
            {
                Console.WriteLine($"Invalid {nameof(TEnum)} value '{str.Value}' specified for '{key}'.");
                value = default;
                return false;
            }
            return true;
        }
    }
}
