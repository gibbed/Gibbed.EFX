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
using Gibbed.EFX.FileFormats.Resources;

namespace Gibbed.EFX.Import
{
    internal partial class Program
    {
        private static bool ImportResource(Tommy.TomlTable table, BaseResource resource)
        {
            if (resource is Unknown50Resource unknown50Resource)
            {
                return ImportResource(table, unknown50Resource);
            }
            else if (resource is ModelResource modelResource)
            {
                return ImportResource(table, modelResource);
            }
            throw new NotSupportedException($"unhandled resource type {resource.GetType()}");
        }

        private static bool ImportResource(Tommy.TomlTable table, Unknown50Resource resource)
        {
            resource.Unknown04 = (int)table["u04"].AsInteger.Value;

            foreach (Tommy.TomlTable entryTable in table["entries"])
            {
                if (ImportUnknown50ResourceEntry(entryTable, out var entry) == false)
                {
                    return false;
                }
                resource.Entries.Add(entry);
            }

            foreach (Tommy.TomlInteger integer in table["texture_ids"])
            {
                resource.TextureIds.Add((byte)integer.Value);
            }

            foreach (Tommy.TomlInteger integer in table["resource_53_ids"])
            {
                resource.Resource53Ids.Add((byte)integer.Value);
            }

            return true;
        }

        private static bool ImportUnknown50ResourceEntry(Tommy.TomlTable table, out Unknown50ResourceEntry entry)
        {
            entry = default;

            if (ImportVector(table["u00"], out var unknown00) == false)
            {
                return false;
            }
            entry.Unknown00 = unknown00;

            if (ImportVector(table["u00"], out var unknown10) == false)
            {
                return false;
            }
            entry.Unknown10 = unknown10;

            if (ImportVector(table["u00"], out var unknown20) == false)
            {
                return false;
            }
            entry.Unknown20 = unknown20;

            if (ImportVector(table["u00"], out var unknown30) == false)
            {
                return false;
            }
            entry.Unknown30 = unknown30;

            if (ImportVector(table["u00"], out var unknown40) == false)
            {
                return false;
            }
            entry.Unknown40 = unknown40;

            if (ImportVector(table["u00"], out var unknown50) == false)
            {
                return false;
            }
            entry.Unknown50 = unknown50;

            if (ImportColor(table["u60"], out var unknown60) == false)
            {
                return false;
            }
            entry.Unknown60 = unknown60;

            if (ImportBytes(table["u64"], out var unknown64Bytes) == false)
            {
                return false;
            }
            entry.Unknown64 = unknown64Bytes;

            if (ImportBytes(table["u70"], out var unknown70) == false)
            {
                return false;
            }
            entry.Unknown70 = unknown70;

            return true;
        }

        private static bool ImportResource(Tommy.TomlTable table, ModelResource resource)
        {
            resource.Flags = (byte)table["flags"].AsInteger.Value;
            resource.Unknown010Count = (byte)table["u010_count"].AsInteger.Value;
            resource.Unknown012Count = (byte)table["u012_count"].AsInteger.Value;

            foreach (Tommy.TomlTable vertexTable in table["vertices"])
            {
                if (ImportVector(vertexTable, out var vertex) == false)
                {
                    return false;
                }
                resource.Vertices.Add(vertex);
            }

            foreach (Tommy.TomlTable vectorTable in table["unknown_vectors"])
            {
                if (ImportVector(vectorTable, out var vertex) == false)
                {
                    return false;
                }
                resource.UnknownVectors.Add(vertex);
            }

            foreach (Tommy.TomlNode colorNode in table["colors"])
            {
                if (ImportColor(colorNode, out var color) == false)
                {
                    return false;
                }
                resource.Colors.Add(color);
            }

            foreach (Tommy.TomlNode uvNode in table["uvs"])
            {
                if (ImportUV(uvNode, out var uv) == false)
                {
                    return false;
                }
                resource.UVs.Add(uv);
            }

            foreach (Tommy.TomlTable entryTable in table["unknown_entries"])
            {
                if (ImportModelUnknown(entryTable, out var entry) == false)
                {
                    return false;
                }
                resource.UnknownEntries.Add(entry);
            }

            if (ImportShorts(table["u170_0"], out var unknown170_0) == false)
            {
                return false;
            }
            resource.Unknown170_0 = unknown170_0;

            if (ImportShorts(table["u170_1"], out var unknown170_1) == false)
            {
                return false;
            }
            resource.Unknown170_1 = unknown170_1;

            if (ImportShorts(table["u174_0"], out var unknown174_0) == false)
            {
                return false;
            }
            resource.Unknown174_0 = unknown174_0;

            if (ImportShorts(table["u174_1"], out var unknown174_1) == false)
            {
                return false;
            }
            resource.Unknown174_1 = unknown174_1;

            if (ImportBytes(table["u188_0"], out var unknown188_0) == false)
            {
                return false;
            }
            resource.Unknown188_0 = unknown188_0;

            if (ImportBytes(table["u188_1"], out var unknown188_1) == false)
            {
                return false;
            }
            resource.Unknown188_1 = unknown188_1;

            foreach (Tommy.TomlInteger integer in table["texture_ids"])
            {
                resource.TextureIds.Add((byte)integer.Value);
            }

            return true;
        }

        private static bool ImportModelUnknown(Tommy.TomlTable table, out ModelUnknown entry)
        {
            throw new NotImplementedException();
        }
    }
}
