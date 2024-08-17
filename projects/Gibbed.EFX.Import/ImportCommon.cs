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
using Gibbed.EFX.FileFormats;

namespace Gibbed.EFX.Import
{
    internal partial class Program
    {
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

        private static float ImportFloat(Tommy.TomlNode node)
        {
            var asInteger = node.AsInteger;
            if (asInteger != null)
            {
                return asInteger.Value;
            }
            var asFloat = node.AsFloat;
            if (asFloat != null)
            {
                return (float)asFloat.Value;
            }
            throw new InvalidOperationException();
        }

        private static double ImportDouble(Tommy.TomlNode node)
        {
            var asInteger = node.AsInteger;
            if (asInteger != null)
            {
                return asInteger.Value;
            }
            var asFloat = node.AsFloat;
            if (asFloat != null)
            {
                return asFloat.Value;
            }
            throw new InvalidOperationException();
        }

        private static bool ImportColor(Tommy.TomlNode node, out Color color)
        {
            if (node is Tommy.TomlInteger integer)
            {
                var rgba = (uint)integer.Value;
                color = new(rgba);
                return true;
            }

            if (node is not Tommy.TomlArray array)
            {
                color = default;
                return false;
            }

            List<byte> list = new();
            foreach (var child in array.Children)
            {
                list.Add((byte)child.AsInteger.Value);
            }
            if (list.Count != 4)
            {
                throw new InvalidOperationException();
            }
            color = new(list[0], list[1], list[2], list[3]);
            return true;
        }

        private static bool ImportVector(Tommy.TomlNode node, out Vector4 vector)
        {
            if (node is not Tommy.TomlArray array)
            {
                vector = default;
                return false;
            }
            List<float> list = new();
            foreach (var child in array.Children)
            {
                list.Add(ImportFloat(child));
            }
            if (list.Count != 4)
            {
                throw new InvalidOperationException();
            }
            vector = default;
            vector.X = list[0];
            vector.Y = list[1];
            vector.Z = list[2];
            vector.W = list[3];
            return true;
        }

        private static bool ImportUV(Tommy.TomlNode node, out UV value)
        {
            if (node is not Tommy.TomlArray array)
            {
                value = default;
                return false;
            }
            List<float> list = new();
            foreach (var child in array.Children)
            {
                list.Add(ImportFloat(child));
            }
            if (list.Count != 2)
            {
                throw new InvalidOperationException();
            }
            value = default;
            value.U = list[0];
            value.V = list[1];
            return true;
        }

        private static bool ImportBytes(Tommy.TomlNode node, out byte[] values)
        {
            if (node == null)
            {
                values = default;
                return true;
            }
            if (node is not Tommy.TomlArray array)
            {
                values = default;
                return false;
            }
            List<byte> list = new();
            foreach (var child in array.Children)
            {
                list.Add((byte)child.AsInteger.Value);
            }
            values = list.ToArray();
            return true;
        }

        private static bool ImportShorts(Tommy.TomlNode node, out short[] values)
        {
            if (node == null)
            {
                values = default;
                return true;
            }
            if (node is not Tommy.TomlArray array)
            {
                values = default;
                return false;
            }
            List<short> list = new();
            foreach (var child in array.Children)
            {
                list.Add((short)child.AsInteger.Value);
            }
            values = list.ToArray();
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
