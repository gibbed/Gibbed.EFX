// https://gist.github.com/ladeak/71b0b4c59bdd4eb548535dc641729682#file-pooledarraybufferwriter-cs

using System;
using System.Buffers;

namespace Gibbed.Buffers
{
    public sealed class PooledArrayBufferWriter<T> : IBufferWriter<T>
    {
        private const int DefaultInitialBufferSize = 4096 * 2;

        private T[] _Buffer;
        private int _Index;

        public PooledArrayBufferWriter()
        {
            this._Buffer = ArrayPool<T>.Shared.Rent(DefaultInitialBufferSize);
            this._Index = 0;
        }

        public ReadOnlyMemory<T> WrittenMemory => this._Buffer.AsMemory(0, this._Index);

        public ReadOnlySpan<T> WrittenSpan => this._Buffer.AsSpan(0, this._Index);

        public int WrittenCount => this._Index;

        public int Capacity => this._Buffer.Length;

        public int FreeCapacity => this._Buffer.Length - this._Index;

        public void Clear()
        {
            ArrayPool<T>.Shared.Return(this._Buffer);
            this._Buffer = ArrayPool<T>.Shared.Rent(DefaultInitialBufferSize);
            this._Index = 0;
        }

        public void Advance(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (this._Index > this._Buffer.Length - count)
            {
                ThrowInvalidOperationException_AdvancedTooFar();
            }
            this._Index += count;
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return this._Buffer.AsMemory(this._Index);
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return this._Buffer.AsSpan(this._Index);
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            if (sizeHint == 0)
            {
                sizeHint = 1;
            }

            if (sizeHint > this.FreeCapacity)
            {
                int currentLength = this._Buffer.Length;

                int growBy = Math.Max(sizeHint, currentLength);

                int newSize = currentLength + growBy;

                var temp = ArrayPool<T>.Shared.Rent(newSize);
                Array.Copy(this._Buffer, temp, this._Index);
                ArrayPool<T>.Shared.Return(this._Buffer);
                this._Buffer = temp;
            }
        }

        private static void ThrowInvalidOperationException_AdvancedTooFar() => throw new InvalidOperationException();
    }
}
