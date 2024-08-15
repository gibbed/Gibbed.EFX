// https://gist.github.com/ladeak/71b0b4c59bdd4eb548535dc641729682#file-chainedarraybufferwriter-cs

using System;

namespace Gibbed.Buffers
{
    internal class MemorySegment<T>
    {
        private T[] _Buffer;
        private int _Index;

        public MemorySegment(T[] buffer)
        {
            this._Buffer = buffer;
            this._Index = 0;
            this.RunningIndex = 0;
            this.NextSegment = null;
        }

        internal int RunningIndex { get; private set; }

        internal T[] Buffer => this._Buffer;

        internal int Index => this._Index;

        internal int FreeCapacity => this._Buffer.Length - this._Index;

        internal Memory<T> WritableMemory => this._Buffer.AsMemory(this._Index);

        internal Span<T> WritableSpan => this._Buffer.AsSpan(this._Index);

        internal ReadOnlySpan<T> Span => this._Buffer.AsSpan(0, this._Index);

        internal MemorySegment<T>? NextSegment { get; private set; }

        public void Advance(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException(null, nameof(count));
            }
            if (this._Index > this._Buffer.Length - count)
            {
                ThrowInvalidOperationException_AdvancedTooFar();
            }
            this._Index += count;
        }

        public MemorySegment<T> Append(T[] nextSegment)
        {
            var segment = new MemorySegment<T>(nextSegment)
            {
                RunningIndex = this.RunningIndex + this._Index
            };
            this.NextSegment = segment;
            return segment;
        }

        private static void ThrowInvalidOperationException_AdvancedTooFar() => throw new InvalidOperationException();
    }
}
