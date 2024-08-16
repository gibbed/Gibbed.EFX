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
using System.Diagnostics;
using System.IO;
using Gibbed.Buffers;
using Gibbed.EFX.FileFormats;
using Gibbed.Memory;
using NDesk.Options;

namespace Gibbed.EFX.Test
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

                PooledArrayBufferWriter<byte> writer = new();
                effect.Serialize(writer);
                var writtenSpan = writer.WrittenSpan;
                if (writtenSpan.SequenceEqual(inputBytes) == false)
                {
                    Console.WriteLine($"mismatch: {inputPath}");

                    if (Debugger.IsAttached == true)
                    {
                        File.WriteAllBytes("mismatch.bin", writtenSpan.ToArray());
                        throw new InvalidOperationException();
                    }
                }
                writer.Clear();
            }
        }
    }
}
