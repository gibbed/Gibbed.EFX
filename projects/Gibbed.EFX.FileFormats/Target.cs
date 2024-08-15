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

namespace Gibbed.EFX.FileFormats
{
    public readonly struct Target : IEquatable<Target>
    {
        public readonly Game Game;
        public readonly byte Version;

        public Target(Game game, byte version)
        {
            this.Game = game;
            this.Version = version;
        }

        public bool Is64Bit => this.Game == Game.TacticsOgreReborn;

        public override bool Equals(object obj)
        {
            return obj is Target version && this.Equals(version) == true;
        }

        public bool Equals(Target other)
        {
            return this.Game == other.Game &&
                   this.Version == other.Version;
        }

        public override int GetHashCode()
        {
            int hashCode = -1957140106;
            hashCode = hashCode * -1521134295 + this.Game.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Version.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Target left, Target right)
        {
            return left.Equals(right) == true;
        }

        public static bool operator !=(Target left, Target right)
        {
            return left.Equals(right) == false;
        }
    }
}
