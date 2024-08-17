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
using System.IO;
using Gibbed.EFX.FileFormats;
using Gibbed.EFX.FileFormats.Commands;
using UnhandledResource = Gibbed.EFX.FileFormats.Resources.UnhandledResource;

namespace Gibbed.EFX.Import
{
    internal partial class Program
    {
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
            command.Key = key;
            var resource = command.Resource = ResourceFactory.Create(key.Type);
            if (resource is UnhandledResource unhandledResource)
            {
                var dataPath = table["data_path"].AsString.Value;
                dataPath = Path.Combine(inputBasePath, dataPath);
                unhandledResource.Data = File.ReadAllBytes(dataPath);
                return true;
            }
            if (ImportResource(table["resource"].AsTable, resource) == false)
            {
                return false;
            }
            if (ImportBytes(table["padding"], out var paddingBytes) == true)
            {
                command.Padding = paddingBytes;
            }
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
    }
}
