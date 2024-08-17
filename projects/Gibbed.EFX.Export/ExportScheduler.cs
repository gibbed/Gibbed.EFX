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
using Gibbed.EFX.FileFormats.Commands;
using Gibbed.EFX.FileFormats.Schedulers;

namespace Gibbed.EFX.Export
{
    internal partial class Program
    {
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

            if (command.Padding?.Length > 0)
            {
                table["padding"] = Export(command.Padding);
            }
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

                Tommy.TomlArray entriesArray = new()
                {
                    //IsTableArray = true,
                    IsMultiline = true,
                };

                foreach (var entry in scheduler.Entries)
                {
                    Tommy.TomlTable entryTable = new();
                    Export(entry, entryTable);
                    entriesArray.Add(entryTable);
                }

                table["entries"] = entriesArray;
            }
        }

        private static void Export(Unknown2Entry entry, Tommy.TomlTable table)
        {
            table["type"] = entry.Type;
            table["timeline_start"] = entry.TimelineStart;
            table["payload"] = Export(entry.Payload);
        }

        private static void Export(Unknown3Scheduler scheduler, Tommy.TomlTable table)
        {
            Export((BaseScheduler)scheduler, table);
            table["u10"] = scheduler.Unknown10;
            table["u11"] = scheduler.Unknown11;
            table["attach_id"] = scheduler.AttachId;
            table["u1C"] = scheduler.Unknown1C;
        }
    }
}
