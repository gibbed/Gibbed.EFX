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
using Gibbed.Memory;
using NDesk.Options;

namespace Gibbed.EFX.Import
{
    internal partial class Program
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
    }
}
