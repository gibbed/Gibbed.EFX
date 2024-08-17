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
using Gibbed.EFX.FileFormats.Commands;
using Gibbed.EFX.FileFormats.Resources;

namespace Gibbed.EFX.Export
{
    internal partial class Program
    {
        private static void Export(ResourceAddCommand command, int commandIndex, Tommy.TomlTable table, string outputBasePath)
        {
            table["key"] = Export(command.Key);

            if (command.Resource is UnhandledResource unhandledResource)
            {
                var key = command.Key;
                var outputName = $"cmd_{commandIndex}_{command.Opcode}_{key.Type}_{key.Unknown}_{key.Id}.bin";
                var outputPath = Path.Combine(outputBasePath, outputName);
                CreatePath(outputPath);
                File.WriteAllBytes(outputPath, unhandledResource.Data);

                table["data_path"] = PathHelper.GetRelativePath(outputBasePath, outputPath) ?? throw new InvalidOperationException();
                return;
            }

            Tommy.TomlTable resourceTable = new();
            if (command.Resource is Unknown50Resource unknown50Resource)
            {
                Export(unknown50Resource, resourceTable);
            }
            else if (command.Resource is ModelResource modelResource)
            {
                Export(modelResource, resourceTable);
            }
            else
            {
                throw new NotSupportedException();
            }
            table["resource"] = resourceTable;

            if (command.Padding?.Length > 0)
            {
                table["padding"] = Export(command.Padding);
            }
        }

        private static void Export(Unknown50Resource resource, Tommy.TomlTable table)
        {
            table["u04"] = resource.Unknown04;

            if (resource.Entries.Count > 0)
            {
                table.IsInline = false;

                Tommy.TomlArray entriesArray = new()
                {
                    //IsTableArray = true,
                    IsMultiline = true,
                };

                foreach (var entry in resource.Entries)
                {
                    Tommy.TomlTable entryTable = new();
                    Export(entry, entryTable);
                    entriesArray.Add(entryTable);
                }

                table["entries"] = entriesArray;
            }

            if (resource.TextureIds.Count > 0)
            {
                table.IsInline = false;

                Tommy.TomlArray idsArray = new()
                {
                    //IsTableArray = true,
                    IsMultiline = true,
                };

                foreach (var entry in resource.TextureIds)
                {
                    idsArray.Add(entry);
                }

                table["texture_ids"] = idsArray;
            }

            if (resource.Resource53Ids.Count > 0)
            {
                table.IsInline = false;

                Tommy.TomlArray idsArray = new()
                {
                    //IsTableArray = true,
                    IsMultiline = true,
                };

                foreach (var entry in resource.Resource53Ids)
                {
                    idsArray.Add(entry);
                }

                table["resource_53_ids"] = idsArray;
            }
        }

        private static void Export(Unknown50ResourceEntry entry, Tommy.TomlTable table)
        {
            table["u00"] = Export(entry.Unknown00);
            table["u10"] = Export(entry.Unknown10);
            table["u20"] = Export(entry.Unknown20);
            table["u30"] = Export(entry.Unknown30);
            table["u40"] = Export(entry.Unknown40);
            table["u50"] = Export(entry.Unknown50);
            table["u60"] = Export(entry.Unknown60);
            table["u64"] = Export(entry.Unknown64);
            if (entry.Unknown70 != null)
            {
                table["u70"] = Export(entry.Unknown70);
            }
        }

        private static void Export(ModelResource resource, Tommy.TomlTable table)
        {
            table["flags"] = resource.Flags;
            table["u010_count"] = resource.Unknown010Count;
            table["u012_count"] = resource.Unknown012Count;

            if (resource.Vertices.Count > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray verticesArray = new();
                foreach (var entry in resource.Vertices)
                {
                    verticesArray.Add(Export(entry));
                }
                table["vertices"] = verticesArray;
            }

            if (resource.UnknownVectors.Count > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray vectorsArray = new();
                foreach (var entry in resource.UnknownVectors)
                {
                    vectorsArray.Add(Export(entry));
                }
                table["unknown_vectors"] = vectorsArray;
            }

            if (resource.Colors.Count > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray colorsArray = new();
                foreach (var entry in resource.Colors)
                {
                    colorsArray.Add(Export(entry));
                }
                table["colors"] = colorsArray;
            }

            if (resource.UVs.Count > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray uvsArray = new();
                foreach (var entry in resource.UVs)
                {
                    uvsArray.Add(Export(entry));
                }
                table["uvs"] = uvsArray;
            }

            if (resource.UnknownEntries.Count > 0)
            {
                throw new NotImplementedException();
            }

            if (resource.Unknown170_0?.Length > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray indexArray = new();
                foreach (var index in resource.Unknown170_0)
                {
                    indexArray.Add(index);
                }
                table["u170_0"] = indexArray;
            }

            if (resource.Unknown170_1?.Length > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray indexArray = new();
                foreach (var index in resource.Unknown170_1)
                {
                    indexArray.Add(index);
                }
                table["u170_1"] = indexArray;
            }

            if (resource.Unknown174_0?.Length > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray indexArray = new();
                foreach (var index in resource.Unknown174_0)
                {
                    indexArray.Add(index);
                }
                table["u174_0"] = indexArray;
            }

            if (resource.Unknown174_1?.Length > 0)
            {
                table.IsInline = false;
                Tommy.TomlArray indexArray = new();
                foreach (var index in resource.Unknown174_1)
                {
                    indexArray.Add(index);
                }
                table["u174_1"] = indexArray;
            }

            if (resource.Unknown188_0?.Length > 0)
            {
                table["u188_0"] = Export(resource.Unknown188_0);
            }

            if (resource.Unknown188_1?.Length > 0)
            {
                table["u188_1"] = Export(resource.Unknown188_1);
            }

            if (resource.TextureIds.Count > 0)
            {
                table.IsInline = false;

                Tommy.TomlArray idsArray = new()
                {
                    //IsTableArray = true,
                    IsMultiline = true,
                };

                foreach (var entry in resource.TextureIds)
                {
                    idsArray.Add(entry);
                }

                table["texture_ids"] = idsArray;
            }
        }
    }
}
