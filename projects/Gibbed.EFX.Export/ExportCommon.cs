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

using Gibbed.EFX.FileFormats;

namespace Gibbed.EFX.Export
{
    internal partial class Program
    {
        private static Tommy.TomlTable Export(ResourceKey key)
        {
            Tommy.TomlTable table = new();
            table.IsInline = true;
            table["unknown"] = key.Unknown;
            table["type"] = key.Type.ToString();
            table["id"] = key.Id;
            return table;
        }

        private static Tommy.TomlInteger Export(Color color)
        {
            return new()
            {
                IntegerBase = Tommy.TomlInteger.Base.Hexadecimal,
                Value = color.AsRGBA,
            };
        }

        private static Tommy.TomlArray Export(Vector4 vector)
        {
            return new()
            {
                vector.X,
                vector.Y,
                vector.Z,
                vector.W,
            };
        }

        private static Tommy.TomlArray Export(UV uv)
        {
            return new()
            {
                uv.U,
                uv.V,
            };
        }

        private static Tommy.TomlNode Export(byte[] bytes)
        {
            Tommy.TomlArray array = new();
            foreach (var value in bytes)
            {
                array.Add(new Tommy.TomlInteger()
                {
                    IntegerBase = value != 0
                        ? Tommy.TomlInteger.Base.Hexadecimal
                        : Tommy.TomlInteger.Base.Decimal,
                    Value = value,
                });
            }
            return array;
        }
    }
}
