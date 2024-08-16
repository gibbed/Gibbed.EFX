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
using System.Text;
using Gibbed.EFX.FileFormats;
using Gibbed.EFX.FileFormats.Commands;
using Gibbed.EFX.FileFormats.Schedulers;
using Gibbed.Memory;
using NDesk.Options;

namespace Gibbed.EFX.Export
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

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extra.Count < 1 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_efx+", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            List<string> inputPaths = new();
            foreach (var inputPath in extra)
            {
                if (Directory.Exists(inputPath) == true)
                {
                    inputPaths.AddRange(Directory.EnumerateFiles(inputPath, "*.efx", SearchOption.AllDirectories));
                }
                else
                {
                    inputPaths.Add(inputPath);
                }
            }

            foreach (var inputPath in inputPaths)
            {
                string outputBasePath = Path.ChangeExtension(inputPath, null) + "_unpack";

                if (verbose == true)
                {
                    Console.WriteLine(inputPath);
                }

                var inputBytes = File.ReadAllBytes(inputPath);

                EffectFile effect = new()
                {
                    Endian = Endian.Little,
                };
                effect.Deserialize(inputBytes);

                Export(effect, outputBasePath);
            }
        }

        private static void Export(EffectFile effect, string outputBasePath)
        {
            Tommy.TomlArray commandsArray = new()
            {
                IsTableArray = true,
            };

            for (int i = 0; i < effect.Commands.Count; i++)
            {
                var command = effect.Commands[i];
                var opcode = command.Opcode;

                Tommy.TomlTable commandTable = new();
                commandTable["command"] = command.Opcode.ToString();

                if (command is ResourceAddCommand resourceAddCommand)
                {
                    var key = resourceAddCommand.Key;
                    var outputName = $"cmd_{i}_{opcode}_{key.Type}_{key.Unknown}_{key.Id}.bin";
                    var outputPath = Path.Combine(outputBasePath, outputName);
                    CreatePath(outputPath);
                    File.WriteAllBytes(outputPath, resourceAddCommand.Data);

                    commandTable["key"] = Export(resourceAddCommand.Key);
                    commandTable["data_path"] = PathHelper.GetRelativePath(outputBasePath, outputPath) ?? throw new InvalidOperationException();
                }
                else if (command is SchedulerAddCommand schedulerAddCommand)
                {
                    Export(schedulerAddCommand, commandTable);
                }
                else if (command is UnhandledCommand unhandledCommand)
                {
                    var outputName = $"cmd_{i}_{opcode}.bin";
                    var outputPath = Path.Combine(outputBasePath, outputName);
                    CreatePath(outputPath);
                    File.WriteAllBytes(outputPath, unhandledCommand.Data);

                    commandTable["data_offset"] = unhandledCommand.DataOffset;
                    commandTable["data_path"] = PathHelper.GetRelativePath(outputBasePath, outputPath) ?? throw new InvalidOperationException();
                }
                else
                {
                    throw new NotSupportedException($"unhandled command type {command.GetType()}");
                }

                commandsArray.Add(commandTable);
            }

            Tommy.TomlTable rootTable = new();

            rootTable["game"] = effect.Target.Game.ToString();
            rootTable["version"] = effect.Target.Version;

            rootTable["commands"] = commandsArray;

            StringBuilder sb = new();
            using (StringWriter writer = new(sb))
            {
                rootTable.WriteTo(writer);
                writer.Flush();
            }

            var commandsOutputPath = Path.Combine(outputBasePath, "@efx.toml");
            CreatePath(commandsOutputPath);
            File.WriteAllText(commandsOutputPath, sb.ToString(), Encoding.UTF8);
        }

        private static Tommy.TomlTable Export(ResourceKey key)
        {
            Tommy.TomlTable table = new();
            table.IsInline = true;
            table["unknown"] = key.Unknown;
            table["type"] = key.Type.ToString();
            table["id"] = key.Id;
            return table;
        }

        private static void Export(SchedulerAddCommand command, Tommy.TomlTable table)
        {
            table["meta_id"] = command.MetaId;
            table["page_id"] = command.PageId;
            table["scheduler_id"] = command.SchedulerId;

            Tommy.TomlTable schedulerTable = new();
            schedulerTable.IsInline = true;

            schedulerTable["type"] = command.Scheduler.Type.ToString();

            if (command.Scheduler is Unknown0Scheduler scheduler0)
            {
                Export(scheduler0, schedulerTable);
            }
            else if (command.Scheduler is Unknown1Scheduler scheduler1)
            {
                Export(scheduler1, schedulerTable);
            }
            else if (command.Scheduler is Unknown2Scheduler scheduler2)
            {
                Export(scheduler2, schedulerTable);
            }
            else if (command.Scheduler is Unknown3Scheduler scheduler3)
            {
                Export(scheduler3, schedulerTable);
            }
            else
            {
                throw new NotSupportedException();
            }

            table["scheduler"] = schedulerTable;
        }

        private static void Export(BaseScheduler scheduler, Tommy.TomlTable table)
        {
            table["u2"] = scheduler.Unknown2;
            table["u5"] = scheduler.Unknown5;
            table["timeline_start"] = scheduler.TimelineStart;
            table["timeline_end"] = scheduler.TimelineEnd;
        }

        private static void Export(Unknown0Scheduler scheduler, Tommy.TomlTable table)
        {
            Export((BaseScheduler)scheduler, table);
        }

        private static void Export(Unknown1Scheduler scheduler, Tommy.TomlTable table)
        {
            Export((BaseScheduler)scheduler, table);
            table["generator_id"] = scheduler.GeneratorId;
            table["element_id"] = scheduler.ElementId;
            table["u14"] = scheduler.Unknown14;
        }

        private static void Export(Unknown2Scheduler scheduler, Tommy.TomlTable table)
        {
            Export((BaseScheduler)scheduler, table);

            if (scheduler.EntryAllocatedCount != scheduler.Entries.Count)
            {
                table["entry_allocated_count"] = scheduler.EntryAllocatedCount;
            }

            if (scheduler.Entries.Count > 0)
            {
                table.IsInline = false;

                Tommy.TomlArray actionsArray = new()
                {
                    //IsTableArray = true,
                    IsMultiline = true,
                };

                foreach (var entry in scheduler.Entries)
                {
                    Tommy.TomlTable actionTable = new();
                    Export(entry, actionTable);
                    actionsArray.Add(actionTable);
                }

                table["actions"] = actionsArray;
            }
        }

        private static void Export(Unknown2Sub action, Tommy.TomlTable table)
        {
            table["type"] = action.Type;
            table["u1"] = BitConverter.ToString(action.Unknown).ToUpperInvariant().Replace("-", " ");
        }

        private static void Export(Unknown3Scheduler scheduler, Tommy.TomlTable table)
        {
            Export((BaseScheduler)scheduler, table);
            table["u10"] = scheduler.Unknown10;
            table["u11"] = scheduler.Unknown11;
            table["attach_id"] = scheduler.AttachId;
            table["u1C"] = scheduler.Unknown1C;
        }

        private static void CreatePath(string path)
        {
            var parentPath = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(parentPath) == false)
            {
                Directory.CreateDirectory(parentPath);
            }
        }
    }
}
