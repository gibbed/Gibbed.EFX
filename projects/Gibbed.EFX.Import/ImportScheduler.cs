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
using Gibbed.EFX.FileFormats;
using Gibbed.EFX.FileFormats.Schedulers;

namespace Gibbed.EFX.Import
{
    internal partial class Program
    {
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
    }
}
